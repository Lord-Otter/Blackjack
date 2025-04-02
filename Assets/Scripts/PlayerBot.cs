using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBot : MonoBehaviour
{
    private GameController gameController;
    private CardStack player;
    private CardStack dealer;
    public Button hitButton;
    public Button standButton;
    public Button doubleButton;
    public Button playAgainButton;
    public Text totalAmount;
    public Text betAmount;

    public Text playerTotal;
    public Text dealerTotal;

    public int totalAmountMoney;
    public int betAmountMoney;
    public int bettingAmount;

    [Header("Time Scale")]
    public float timeScale;
    [HideInInspector]public float lastTimeScale;
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
        //HitTheButtons();
        UpdateMoneyText();
        TimeScale();
    }

    void TimeScale()
    {
        Time.timeScale = timeScale;

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (timeScale == 0)
            {
                timeScale = lastTimeScale;
                Debug.Log($"Time Scale: {timeScale * 100}% ");
            }
            else
            {
                lastTimeScale = timeScale;
                timeScale = 0;
                Debug.Log("Stopping Time");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            timeScale = 1f;
            Debug.Log($"Time Scale: {timeScale * 100}% ");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            timeScale = 2f;
            Debug.Log($"Time Scale: {timeScale * 100}% ");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            timeScale = 10f;
            Debug.Log($"Time Scale: {timeScale * 100}% ");
        }
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

    public void HitTheButtons()
    {
        if(playAgainButton.interactable)
        {
            gameController.PlayAgain();
        }
                  
        if(hitButton.interactable)
        {       
            if(int.Parse(playerTotal.text) > 18)
            {
                gameController.Stand();
            }
            else
            {
                gameController.Hit();
            }
        }

        if(doubleButton.interactable)
        {
            Debug.Log("HELP ME");
            if(int.Parse(playerTotal.text) < 15)
            {
                gameController.DoubleDown();
                Debug.Log("Double Down");
            }
        }
        
    }
}
