using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeckController : MonoBehaviour
{
    public static DeckController instance;

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        SetupDeck();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            DrawCardToHand();
        }
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

    public void DrawCardToHand()
    {
        // Si el mazo se vacía, lo recicla
        if (activeCards.Count == 0)
        {
            SetupDeck();
        }

        // Instancia el prefeb de la carta en el mundo
        Card newCard = Instantiate(cardToSpawn, transform.position, Quaternion.identity);

        // Asigna los datos de la primera carta activa y la configura
        newCard.cardSO = activeCards[0];
        newCard.SetupCard();

        // La elimina de la lista activa para que no se repita
        activeCards.RemoveAt(0);
    }
}
