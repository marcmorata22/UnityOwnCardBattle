using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeckController : MonoBehaviour
{
    public static DeckController instance;

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;
    public Transform cardSpawnPoint;
    public int drawCardCost = 2;
    public float waitBetweenDrawingCards = .25f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupDeck();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            DrawCardToHand();
        } */
    }

    public void SetupDeck()
    {
        // 1. Limpiamos la baraja activa por si acaso ya tuviera cartas de una partida anterior.
        activeCards.Clear();

        // 2. Creamos una lista temporal ("tempDeck") que funciona como la mesa donde vamos a volcar las cartas.
        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();

        // 3. Copiamos todas las cartas del mazo original ("deckToUse") a esta mesa temporal ("tempDeck").
        tempDeck.AddRange(deckToUse);

        // 4. Mientras queden cartas en la mesa temporal, repetimos el proceso de robar y barajar.
        int iterators = 0;
        while (tempDeck.Count > 0 && iterators < 500) // Agregamos la  condición iterators adicional para evitar bucles infinitos
        {
            // A) Elegimos un número al azar entre 0 y el total de cartas que quedan en la mesa temporal.
            int selected = Random.Range(0, tempDeck.Count);

            // B) Cogemos la carta que está en esa posición al azar y la AÑADIMOS a nuestra baraja de juego ("activeCards").
            activeCards.Add(tempDeck[selected]);

            // C) QUITAMOS esa carta de la mesa temporal para no volver a seleccionarla por error.
            tempDeck.RemoveAt(selected);

            iterators++;
        }
    }

    // Método público que recibe cuántas cartas queremos robar (por ejemplo, 5 al iniciar la batalla)
    public void DrawMultipleCards(int amountToDraw)
    {
        // Iniciamos una "corrutina" (un proceso que puede pausar su ejecución en el tiempo)
        // Le pasamos la función encargada de hacer la pausa y el número de cartas.
        StartCoroutine(DrawMultipleCo(amountToDraw));
    }

    // Corrutina que se encarga de sacar las cartas una a una dejando un espacio de tiempo entre ellas
    IEnumerator DrawMultipleCo(int amountToDraw)
    {
        // Bucle que se repite tantas veces como cartas queramos robar (desde 0 hasta amountToDraw - 1)
        for (int i = 0; i < amountToDraw; i++)
        {
            // 1. Ejecutamos la función normal para sacar una sola carta y ponerla en la mano
            DrawCardToHand();

            // 2. Pausamos este bucle temporalmente durante los segundos que indique la variable 'waitBetweenDrawingCards'
            // (Esto evita que salgan todas de golpe y crea el efecto visual de ir repartiéndolas una a una)
            yield return new WaitForSeconds(waitBetweenDrawingCards);
        }
    }

    public void DrawCardToHand()
    {
        // Si el mazo se vacía, lo recicla
        if (activeCards.Count == 0)
        {
            SetupDeck();
        }

        // Instancia el prefeb de la carta en el mundo
        Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);

        // Asigna los datos de la primera carta activa y la configura
        newCard.cardSO = activeCards[0];
        newCard.SetupCard();

        // La elimina de la lista activa para que no se repita
        activeCards.RemoveAt(0);

        HandController.instance.addCardToHand(newCard);
    }

    // Método que permite robar una carta gastando maná si tenemos lo suficiente
    public void DrawCardForMana()
    {
        // Comprueba si el jugador tiene suficiente maná
        if (BattleController.instance.playerMana >= drawCardCost)
        {
            DrawCardToHand(); // Roba la carta
            BattleController.instance.SpendPlayerMana(drawCardCost); // Descuenta el maná
        }
        else
        {
            UIController.instance.ShowManaWarning(); // Muestra aviso de falta de maná
            UIController.instance.drawCardButton.SetActive(false); // Oculta el botón de robar
        }
    }
}
