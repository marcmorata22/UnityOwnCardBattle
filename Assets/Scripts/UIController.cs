using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public TMP_Text playerManaText;

    public GameObject manaWarning;
    public float manaWarningTime;
    private float manaWarningCounter;

    public GameObject drawCardButton;

    private void Awake()
    {
        // Pregunta si this e slo mismo que _instance*****************************************************
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(manaWarningCounter > 0)
        {
            manaWarningCounter -= Time.deltaTime;
            if (manaWarningCounter <= 0)
            {
                manaWarning.SetActive(false);
            }
        }
    }

    public void SetPlayerManaText(int manaAmount) 
    {
        playerManaText.text = "Mana: " + manaAmount;
    }

    public void ShowManaWarning()
    {
        manaWarning.SetActive(true);
        manaWarningCounter = manaWarningTime;
    }

    public void DrawCard()
    {
        DeckController.instance.DrawCardForMana();
    }
}
