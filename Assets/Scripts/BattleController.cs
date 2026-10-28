using System.ComponentModel;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    // Creamos una instancia estática (Singleton) para que cualquier carta u otro script 
    // pueda acceder directamente a este BattleController sin tener que buscarlo en la escena.
    public static BattleController instance;

    // Variables públicas para configurar el maná inicial (4) y el límite máximo (12)
    public int startingMana = 4, maxMana = 12;

    // Variable que almacena la cantidad actual de maná que tiene el jugador
    public int playerMana;
    public int startingCardsAmount = 4;

    // Awake se ejecuta antes que el método Start y antes de que empiece el juego
    private void Awake()
    {
        // Guardamos este objeto exacto como la instancia global oficial
        instance = this;
    }

    // Start se ejecuta una vez al arrancar el juego
    void Start()
    {
        // Al comenzar la partida, rellenamos el maná del jugador con el valor inicial (4)
        playerMana = startingMana;
        // Actualizamos la UI para mostrar el maná inicial
        UIController.instance.SetPlayerManaText(playerMana);
        //Forzar Bug de variable pública, también podemos cambiar el nombre a la varible
        DeckController.instance.DrawMultipleCards(startingCardsAmount);
    }

    // Update se ejecuta en cada fotograma
    void Update()
    {
        // Actualmente vacío porque no necesitamos lógica constante por fotograma aquí
    }

    // Función pública que se llama desde la carta para gastar maná al jugarla
    public void SpendPlayerMana(int amountToSpend)
    {
        // Restamos el coste de la carta al maná actual del jugador
        playerMana = playerMana - amountToSpend;

        // Comprobación de seguridad por si el maná bajara de cero por algún motivo
        if (playerMana < 0)
        {
            // Forzamos a que el maná se quede en cero para evitar mostrar números negativos en pantalla
            playerMana = 0;
        }

        UIController.instance.SetPlayerManaText(playerMana);
    }
}