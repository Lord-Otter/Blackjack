using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

public class PlayerBot : MonoBehaviour
{
    private GameController gameController;
    private GraphController graphController;
    private CardStack player;
    private CardStack dealer;
    public Button hitButton;
    public Button standButton;
    public Button doubleButton;
    public Button playAgainButton;
    public Text differenceText;
    public Text totalAmount;
    public Text betAmount;

    public Text playerTotal;
    public Text dealerTotal;

    public enum BettingStrategy {Flat, LossDoubling, WinDoubling, Random}
    [Header("Betting Strategy")]
    public BettingStrategy betStrat;

    [Header("Betting")]
    public int totalAmountMoney;
    [HideInInspector] public int startingAmountMoney;
    public int betAmountMoney;
    public int bettingAmount;
    public int lastBet = 0;
    public int minBet;
    public int maxBet;
    [HideInInspector] public int difference;
    [HideInInspector] public int actualDifference;

    [Header("Time Scale")]
    public float timeScale;
    [HideInInspector]public float lastTimeScale;
    [HideInInspector] public bool betHasBeenMade = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        graphController = GameObject.Find("GraphController").GetComponent<GraphController>();
        player = GameObject.Find("Player").GetComponent<CardStack>();
        dealer = GameObject.Find("Dealer").GetComponent<CardStack>();
    }

    void Start()
    {
        //StartCoroutine(PlayersTurn());
        startingAmountMoney = totalAmountMoney;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameController.simulationEnd)
        {
            return;
        }

        BetMoney();
        if(gameController.canMakeMove)
        {
            HitTheButtons();
            gameController.canMakeMove = false;
        }
        HitRestart();
        UpdateMoneyText();
        TimeScale();
    }

    /*public IEnumerator PlayersTurn()
    {
        while(true)
        {
            BetMoney();
            yield return new WaitForSeconds(1f);
            // Player
            playerTotal.text = player.HandValue().ToString();

            // Dealear
            dealerTotal.text = dealer.HandValue().ToString();

            yield return new WaitForSeconds(1f);

            HitTheButtons();
        }
    }*/

    void UpdateMoneyText()
    {
        totalAmount.text = totalAmountMoney.ToString();
        betAmount.text = betAmountMoney.ToString();
        difference = totalAmountMoney - startingAmountMoney;
        if(difference > 0)
        {
            differenceText.color = Color.green;
            differenceText.text = "+" + difference.ToString();
        }
        else if(difference < 0)
        {
            differenceText.color = Color.red;
            differenceText.text = difference.ToString();
        }
        else
        {
            differenceText.color = Color.gray;
            differenceText.text = difference.ToString();
        }
        
    }

    public void BetMoney()
    {
        switch (betStrat)
        {
            case BettingStrategy.Flat:
                FlatBetting();
                break;

            case BettingStrategy.LossDoubling:
                LossDoubling();
                break;

            case BettingStrategy.WinDoubling:
                WinDoubling();
                break;

            case BettingStrategy.Random:
                RandomBetting();
                break;
        }
    }

    void FlatBetting()
    {
        
        if(totalAmountMoney > bettingAmount)
        {
            if(betAmountMoney <= 0)
            {
                betHasBeenMade = true;
                totalAmountMoney -= bettingAmount;
                betAmountMoney += bettingAmount;
            }
        }
        else
        {
            betHasBeenMade = true;
            betAmountMoney = totalAmountMoney;
            totalAmountMoney = 0;
        }
        
    }
    
    void LossDoubling()
    {
        if(betAmountMoney <= 0)
        {
            if(gameController.lastHandOutcome == GameController.LastHandOutcome.Win)
            {                
                totalAmountMoney -= minBet;
                betAmountMoney += minBet;
                lastBet = minBet;
                betHasBeenMade = true;
            }
            else if(gameController.lastHandOutcome == GameController.LastHandOutcome.Draw)
            {
                totalAmountMoney -= lastBet;
                betAmountMoney += lastBet;
                betHasBeenMade = true;
            }
            else if(gameController.lastHandOutcome == GameController.LastHandOutcome.Loss)
            {
                if(lastBet <= 0)
                {                    
                    totalAmountMoney -= minBet;
                    betAmountMoney += minBet;
                    lastBet = minBet; 
                    betHasBeenMade = true;                   
                }
                else if(lastBet * 2 < maxBet)
                {                    
                    totalAmountMoney -= lastBet * 2;
                    betAmountMoney += lastBet * 2;
                    lastBet *= 2;
                    betHasBeenMade = true;
                }
                else
                {                    
                    totalAmountMoney -= maxBet;
                    betAmountMoney += maxBet;
                    lastBet = maxBet;
                    betHasBeenMade = true;
                }
            }
            

        }
    }

    void WinDoubling()
    {
        if(betAmountMoney <= 0)
        {
            if(gameController.lastHandOutcome == GameController.LastHandOutcome.Win)
            {
                if(lastBet <= 0)
                {
                    betHasBeenMade = true;
                    totalAmountMoney -= minBet;
                    betAmountMoney += minBet;
                    lastBet = minBet;                    
                }
                else if(lastBet * 2 < maxBet)
                {
                    betHasBeenMade = true;
                    totalAmountMoney -= lastBet * 2;
                    betAmountMoney += lastBet * 2;
                    lastBet *= 2;
                }
                else
                {
                    betHasBeenMade = true;
                    totalAmountMoney -= maxBet;
                    betAmountMoney += maxBet;
                    lastBet = maxBet;
                }
            }
            else if(gameController.lastHandOutcome == GameController.LastHandOutcome.Draw)
            {
                betHasBeenMade = true;
                totalAmountMoney -= lastBet;
                betAmountMoney += lastBet;
            }
            else if(gameController.lastHandOutcome == GameController.LastHandOutcome.Loss)
            {
                betHasBeenMade = true;
                totalAmountMoney -= minBet;
                betAmountMoney += minBet;
                lastBet = minBet;
            }
        }
    }

    void RandomBetting()
    {
        if(betAmountMoney <= 0)
        {
            betHasBeenMade = true;
            int randomBet = UnityEngine.Random.Range(minBet / 10, (maxBet / 10) + 1) * 10;
            totalAmountMoney -= randomBet;
            betAmountMoney += randomBet;
            lastBet = randomBet;
        }
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

    /*IEnumerator PlayersTurn()
    {
        if(gameController.softHand)
        {
            while(player.HandValue() <= 20)
            {

            }
        }
        else
        {
            while(player.HandValue() <= 17)
            {

            }
        }
    }*/

    public void HitRestart()
    {
        if(playAgainButton.interactable)
        {
            gameController.softHand = false;
            gameController.PlayAgain();
        }
    }

    public void HitTheButtons()
    {
        if(player.HandValue() >= 10 && !gameController.hasHit)
        {
            gameController.softHand = false;
        }
        
        if(gameController.softHand)  // Soft Totals
        {
            if(doubleButton.interactable)
            {
                if((player.HandValue() == 19 && dealer.HandValue() == 6)
                || (player.HandValue() == 18 && dealer.HandValue() <= 6)
                || (player.HandValue() == 17 && dealer.HandValue() <= 6 && dealer.HandValue() >= 3)
                || (player.HandValue() == 16 && dealer.HandValue() <= 6 && dealer.HandValue() >= 4)
                || (player.HandValue() == 15 && dealer.HandValue() <= 6 && dealer.HandValue() >= 4)
                || (player.HandValue() == 14 && dealer.HandValue() <= 6 && dealer.HandValue() >= 5)
                || (player.HandValue() == 13 && dealer.HandValue() <= 6 && dealer.HandValue() >= 5))
                {                    
                    gameController.DoubleDown();                   
                }
            }

            if(hitButton.interactable)
            {
                if((player.HandValue() == 18 && dealer.HandValue() >= 9)
                || (player.HandValue() == 17 && (dealer.HandValue() <= 2 || dealer.HandValue() >= 7))
                || (player.HandValue() == 16 && (dealer.HandValue() <= 3 || dealer.HandValue() >= 7))
                || (player.HandValue() == 15 && (dealer.HandValue() <= 3 || dealer.HandValue() >= 7))
                || (player.HandValue() == 14 && (dealer.HandValue() <= 4 || dealer.HandValue() >= 7))
                || (player.HandValue() == 13 && (dealer.HandValue() <= 4 || dealer.HandValue() >= 7))
                || (player.HandValue() == 12))
                {
                    gameController.Hit();                    
                }
                else
                {
                    gameController.Stand();                    
                }
            }
        }
        else if(!gameController.softHand)   // Hard Totals
        {
            
            
            if(doubleButton.interactable)
            {
                if((int.Parse(playerTotal.text) == 11) 
                || (int.Parse(playerTotal.text) == 10 && dealer.HandValue() <= 9) 
                || (int.Parse(playerTotal.text) == 9 && dealer.HandValue() <= 6 && dealer.HandValue() >= 3))
                {
                    gameController.DoubleDown();
                }
            }
                    
            if(hitButton.interactable)
            {
                if((int.Parse(playerTotal.text) >= 13 && int.Parse(playerTotal.text) <= 16 && dealer.HandValue() >= 7)
                || (int.Parse(playerTotal.text) == 12 && (dealer.HandValue() >= 7 || dealer.HandValue() <= 3))
                || (int.Parse(playerTotal.text) <= 11))
                {
                    gameController.Hit();
                }
                else
                {
                    gameController.Stand();
                }     
            }
        }              
    }
}
