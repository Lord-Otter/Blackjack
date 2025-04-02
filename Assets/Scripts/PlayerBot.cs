using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBot : MonoBehaviour
{
    private GameController gameController;
    private CardStack player;
    private CardStack dealer;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text totalAmount;
    public Text betAmount;

    public int totalAmountMoney;
    public int betAmountMoney;
    public int bettingAmount;
    [HideInInspector] public bool betHasBeenMade = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        player = GameObject.Find("The Deck").GetComponent<CardStack>();
    }

    // Update is called once per frame
    void Update()
    {
        BetMoney();
        HitTheButtons();
        UpdateMoneyText();
    }

    void UpdateMoneyText()
    {
        totalAmount.text = totalAmountMoney.ToString();;
        betAmount.text = betAmountMoney.ToString();;
    }

    void BetMoney()
    {
        if(betAmountMoney <= 0)
        {
            betHasBeenMade = true;
            totalAmountMoney -= bettingAmount;
            betAmountMoney += bettingAmount;
        }
    }

    void HitTheButtons()
    {
        if(playAgainButton.interactable)
        {
            gameController.PlayAgain();
        }

        if(hitButton.interactable || stickButton.interactable) // It aint workin
        {
            if(player.HandValue() < 21)
            {
                gameController.Stick();
            }
        }

        
    }
}
