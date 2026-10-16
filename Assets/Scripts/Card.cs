// Una carta de la escena: muestra sus datos, se mueve suave a un destino y se puede coger/soltar con el puntero.
// Los using NUnit.Framework, Unity.AppUI.UI y static DebugUI los añadió Visual Studio solo y no se usan.
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // Plantilla de datos compartida (.asset): no se modifica en partida.
    public CardScriptableObject cardSO;
    // Copia de trabajo de los valores del asset (SetupCard los sobrescribe al empezar).
    public int currentHealth;
    public int attackPower;
    public int manaCost;
    // Textos del Canvas de la carta, arrastrados en el Inspector (si uno queda vacío, SetupCard falla).
    public TMP_Text healthText, attakText, costText, nameText, actionDescriptionText, loreText;
    // Ilustración y fondo de la carta.
    public Image CharacterArt, bgArt;
    // Destino al que se acerca la carta cada frame (lo fija MoveToPoint).
    private Vector3 targetPoint;
    private Quaternion targetRot;
    // moveSpeed es el factor del Lerp; rotateSpeed son grados por segundo.
    public float moveSpeed = 5f, rotateSpeed = 540f;
    // Solo reaccionan al ratón las cartas en la mano; este script no los rellena (lo hace el HandController).
    public bool inHand;
    public int handPosition;
    private HandController _handController;

    // true mientras llevas la carta cogida.
    private bool isSelected;
    // Collider de esta carta (debe estar en el mismo objeto que este script).
    private Collider theCol;

    // Capas de los rayos: la mesa por donde arrastras y los huecos del tablero donde sueltas.
    public LayerMask whatIsDesktop, whatIsPlacement;
    // Escudo de un frame para que el clic de coger no cuente también como clic de soltar.
    private bool justPressed;

    // Hueco del tablero donde está colocada la carta (null si está en la mano).
    public CardPlacePoint assignedPlace;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupCard();

        // Se busca una sola vez aquí porque FindAnyObjectByType es lento.
        _handController = Object.FindAnyObjectByType<HandController>();
        theCol = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //carta se encarga de actualizar su posición y rotación de manera suave fotograma
        // Lerp avanza una fracción de la distancia restante por frame: arranca rápido y frena al llegar.
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        // Solo la carta que llevas cogida entra aquí.
        if (isSelected)
        {
            // Pointer es la clase base de ratón, dedo y lápiz, así que el mismo código vale en las dos plataformas: en PC lee el ratón y en móvil el punto donde tocas con el dedo.
            // Rayo que sale de la cámara y atraviesa el punto de pantalla donde está el puntero.
            Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
            // Aquí guarda Unity el resultado del choque del rayo.
            RaycastHit hit;

            // Si el rayo toca la mesa, la carta va a ese punto flotando 2 unidades por encima.
            if (Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
            {
                MoveToPoint(hit.point + new Vector3(0, 2f, 0f), Quaternion.identity);
            }

            // Clic derecho: cancelar (solo existe con ratón, en móvil no).
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                ReturnToHand();
            }

            // Clic izquierdo: intentar colocar (justPressed evita que cuente el mismo clic con el que se cogió).
            if (Mouse.current.leftButton.wasPressedThisFrame && justPressed == false)
            {
                // Mismo rayo, pero ahora solo choca con los huecos del tablero.
                if (Physics.Raycast(ray, out hit, 100f, whatIsPlacement))
                {
                    // Si el objeto tocado no tiene CardPlacePoint, esto es null y la línea siguiente falla.
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

                    // Hueco libre y del jugador: se coloca la carta.
                    if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                    {
                        if (BattleController.instance.playerMana >= manaCost)
                        {
                            // El hueco y la carta se guardan mutuamente.
                            selectedPoint.activeCard = this;
                            assignedPlace = selectedPoint;

                            MoveToPoint(selectedPoint.transform.position, Quaternion.identity);
                            inHand = false;
                            isSelected = false;

                            _handController.RemoveCardFromHand(this);

                            BattleController.instance.SpendPlayerMana(manaCost);
                        }
                        else
                        {
                            // No hay maná suficiente.
                            ReturnToHand();
                        }
                    }
                    else
                    {
                        // Hueco ocupado o del rival.
                        ReturnToHand();
                    }
                }
                else
                {
                    // El rayo no tocó ningún hueco.
                    ReturnToHand();
                }
            }
        }

        // Se apaga al final de cada frame: el escudo dura solo un frame.
        justPressed = false;
    }

    // Copia los datos del asset a la carta y los pinta en la UI.
    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;

        // .text solo acepta string, por eso ToString().
        healthText.text = currentHealth.ToString();
        attakText.text = attackPower.ToString();
        costText.text = manaCost.ToString();

        nameText.text = cardSO.cardName;
        actionDescriptionText.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;

        CharacterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;
    }

    // Solo guarda el destino; el movimiento real lo hace Update.
    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;
    }

    // OnMouseOver is called every frame while the mouse is over the GUIElement or Collider
    private void OnMouseOver()
    {
        // Hover: la carta sube y se endereza mientras el puntero está encima.
        if (inHand)
        {
            MoveToPoint(_handController.cardPositions[handPosition] + new Vector3(0, 1f, .5f), Quaternion.identity);
        }
    }

    // OnMouseExit is called when the mouse is no longer over the GUIElement or Collider
    private void OnMouseExit()
    {
        // Al salir el puntero, vuelve a su sitio de la mano.
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
            // Sin collider no salta OnMouseExit al arrastrarla, que la mandaría de vuelta a la mano.
            theCol.enabled = false;

            justPressed = true;
        }
    }

    // Vuelve a la mano y reactiva el collider para poder cogerla otra vez.
    public void ReturnToHand()
    {
        isSelected = false;
        theCol.enabled = true;

        MoveToPoint(_handController.cardPositions[handPosition], _handController.minPos.rotation);
    }
}
