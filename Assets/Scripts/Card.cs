using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupCard();

        _handController = FindObjectOfType<HandController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
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

    private void OnMouseExit()
    {
        if (inHand)
        {
            MoveToPoint(_handController.cardPositions[handPosition], _handController.minPos.rotation);
        }
    }
}
