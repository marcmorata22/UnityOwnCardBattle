using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public static HandController instance;
    // Lista de las cartas que actualmente tenemos en la mano
    public List<Card> heldCards = new List<Card>();

    // Lista de las posiciones espaciales calculadas para repartir las cartas en abanico/línea
    public List<Vector3> cardPositions = new List<Vector3>();

    // Transformaciones que marcan el punto inicial y final donde se extienden las cartas en la mano
    public Transform minPos, maxPos;

    private void Awake()
    {
        instance = this;
    }

    // Start se ejecuta una vez al iniciar el juego
    void Start()
    {
        // Al arrancar, calcula y asigna la posición inicial de las cartas en la mano
        SetCardPositionInHand();
    }

    // Update se ejecuta en cada fotograma
    void Update()
    {
        // Actualmente vacío porque no necesitamos lógica constante aquí
    }

    // Función para calcular y distribuir las posiciones de todas las cartas en la mano
    public void SetCardPositionInHand()
    {
        // Limpiamos la lista de posiciones anteriores para recalcular desde cero
        cardPositions.Clear();

        Vector3 distanceBetweenPoints = Vector3.zero;

        // Si hay más de una carta, calculamos la distancia proporcional que debe haber entre cada una
        if (heldCards.Count > 1)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (heldCards.Count - 1);
        }

        // Recorremos cada carta de la mano para asignarle su sitio
        for (int i = 0; i < heldCards.Count; i++)
        {
            // Añadimos la posición calculada para esta carta específica en la lista
            cardPositions.Add(minPos.position + (distanceBetweenPoints * i));

            // (Líneas antiguas comentadas donde se teletransportaban de golpe)
            //heldCards[i].transform.position = cardPositions[i];
            //heldCards[i].transform.rotation = minPos.rotation;

            // Ordenamos a la carta que se mueva de forma suave hacia su nueva posición y rotación asignada
            heldCards[i].MoveToPoint(cardPositions[i], minPos.rotation);

            // Marcamos que la carta está oficialmente en la mano y guardamos su índice de posición actual
            heldCards[i].inHand = true;
            heldCards[i].handPosition = i;
        }
    }

    // Función para eliminar una carta de la mano cuando la jugamos en el tablero
    public void RemoveCardFromHand(Card cardToRemove)
    {
        // Comprobación de seguridad: ¿La carta en esa posición coincide exactamente con la que queremos borrar?
        if (heldCards[cardToRemove.handPosition] == cardToRemove)
        {
            // Si coincide, la eliminamos de la lista de la mano
            heldCards.RemoveAt(cardToRemove.handPosition);
        }
        else
        {
            // Si hay un desajuste o error de índices, lanzamos un mensaje de error rojo a la consola
            Debug.LogError("Card at position " + cardToRemove.handPosition + " is not the card being removed from hand");
        }

        // Una vez eliminada, recalculamos las posiciones para que las cartas restantes cierren el hueco
        SetCardPositionInHand();
    }

    public void addCardToHand(Card cardToAdd)
    {
        heldCards.Add(cardToAdd);
        SetCardPositionInHand();
    }
}