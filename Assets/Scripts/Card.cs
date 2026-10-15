using NUnit.Framework;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;
    public int currentHealth;
    public int attackPower;
    public int manaCost;
    public TMP_Text healthText, attakText, costText, nameText, actionDescriptionText, loreText;
    public Image CharacterArt, bgArt;
    private Vector3 targetPoint;
    private Quaternion targetRot;
    public float moveSpeed = 5f, rotateSpeed = 540f;
    public bool inHand;
    public int handPosition;
    private HandController _handController;

    private bool isSelected;
    private Collider theCol;

    public LayerMask whatIsDesktop, whatIsPlacement;
    private bool justPressed;

    public CardPlacePoint assignedPlace;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupCard();

        _handController = Object.FindAnyObjectByType<HandController>();
        theCol = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //carta se encarga de actualizar su posición y rotación de manera suave fotograma
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        if(isSelected)
        {
            // Pointer es la clase base de ratón, dedo y lápiz, así que el mismo código vale en las dos plataformas: en PC lee el ratón y en móvil el punto donde tocas con el dedo.
            Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
            {
                MoveToPoint(hit.point + new Vector3(0, 2f, 0f), Quaternion.identity);
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                ReturnToHand();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame && justPressed == false)
            {
                if (Physics.Raycast(ray, out hit, 100f, whatIsPlacement) && justPressed == false)
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

                    if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                    {
                        selectedPoint.activeCard = this;
                        assignedPlace = selectedPoint;

                        MoveToPoint(selectedPoint.transform.position, Quaternion.identity);
                        inHand = false;
                        isSelected = false;
                    }
                    else
                    {
                        ReturnToHand();
                    }
                }
                else
                {
                    ReturnToHand();
                }
            }
        }

        justPressed = false;
    }

    public void SetupCard() 
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;

        healthText.text = currentHealth.ToString();
        attakText.text = attackPower.ToString();
        costText.text = manaCost.ToString();

        nameText.text = cardSO.cardName;
        actionDescriptionText.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;

        CharacterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;
    }

    // OnMouseOver is called every frame while the mouse is over the GUIElement or Collider
    private void OnMouseOver()
    {
        if (inHand)
        {
            MoveToPoint(_handController.cardPositions[handPosition] + new Vector3(0, 1f, .5f), Quaternion.identity);
        }
    }

    // OnMouseExit is called when the mouse is no longer over the GUIElement or Collider
    private void OnMouseExit()
    {
        if (inHand)
        {
            MoveToPoint(_handController.cardPositions[handPosition], _handController.minPos.rotation);
        }
    }

    private void OnMouseDown()
    {
        if (inHand)
        {
            isSelected = true;
            theCol.enabled = false;

            justPressed = true;
        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        theCol.enabled = true;

        MoveToPoint(_handController.cardPositions[handPosition], _handController.minPos.rotation);
    }
}
