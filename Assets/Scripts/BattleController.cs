using UnityEngine;
using UnityEngine.InputSystem;


public class BattleController : MonoBehaviour
{
    // Creamos una instancia estática (Singleton) para que cualquier carta u otro script 
    // pueda acceder directamente a este BattleController sin tener que buscarlo en la escena.
    public static BattleController instance;

    // Variables públicas para configurar el maná inicial (4) y el límite máximo (12)
    public int startingMana = 4, maxMana = 12;

    // Variable que almacena la cantidad actual de maná que tiene el jugador
    public int playerMana;
    private int currentPlayerMaxMana;
    public int startingCardsAmount = 4;
    public int cardsToDrawPerTurn = 1;

    public enum TurnOrder
    {
        playerActive,
        playerCardAttacks,
        enemyActive,
        enemyCardAttacks
    }
    public TurnOrder currentPhase;

    // Awake se ejecuta antes que el método Start y antes de que empiece el juego
    private void Awake()
    {
        // Guardamos este objeto exacto como la instancia global oficial
        instance = this;
    }

    // Start se ejecuta una vez al arrancar el juego
    void Start()
    {
        currentPlayerMaxMana = startingMana;
        FillPlayerMana();
        //Forzar Bug de variable pública, también podemos cambiar el nombre a la varible
        DeckController.instance.DrawMultipleCards(startingCardsAmount);
    }

    // Update se ejecuta en cada fotograma
    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            AdvanceTurn();
        }

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

    public void FillPlayerMana()
    {
        // Al comenzar la partida, rellenamos el maná del jugador con el valor inicial (4)
        playerMana = currentPlayerMaxMana;
        // Actualizamos la UI para mostrar el maná inicial
        UIController.instance.SetPlayerManaText(playerMana);
    }

    public void AdvanceTurn()
    {
        currentPhase++;

        // Comprueba si hemos llegado al final del enum TurnOrder para reiniciar el ciclo
        if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
        {
            currentPhase = 0;
        }

        // Evalúa la fase actual para ejecutar la lógica correspondiente
        switch (currentPhase)
        {
            case TurnOrder.playerActive:

                // Activa los botones de acción del jugador en su interfaz
                UIController.instance.endTurnButton.SetActive(true);
                UIController.instance.drawCardButton.SetActive(true);

                // Aumenta el maná máximo del jugador si no ha llegado al límite
                if (currentPlayerMaxMana < maxMana)
                {
                    currentPlayerMaxMana++;
                }

                // Rellena el maná disponible y roba las cartas del turno
                FillPlayerMana();

                DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn);

                break;

            case TurnOrder.playerCardAttacks:

                Debug.Log("skipping player card attacks");
                AdvanceTurn();

                //CardPointsController.instance.PlayerAttack();

                break;

            case TurnOrder.enemyActive:

                Debug.Log("skipping enemy actions");
                AdvanceTurn();

                //if (currentEnemyMaxMana < maxMana)
                //{
                //    currentEnemyMaxMana++;
                //}

                //FillEnemyMana();

                //EnemyController.instance.StartAction();

                break;

            case TurnOrder.enemyCardAttacks:

                Debug.Log("skipping enemy card attacks");
                AdvanceTurn();

                //CardPointsController.instance.EnemyAttack();

                break;

        }
    }

    public void EndPlayerTurn()
    {
        // Oculta los botones del jugador al terminar su turno
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawCardButton.SetActive(false);

        // Pasa a la siguiente fase o turno
        AdvanceTurn();
    }
}