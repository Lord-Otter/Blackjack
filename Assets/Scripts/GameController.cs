using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
    public int numberOfLoops;

    [HideInInspector] public int dealersFirstCard = -1;

    public CardStack player;
    public CardStack dealer;
    public CardStack deck;
    private GraphController graphController;

    public Button hitButton;
    public Button standButton;
    public Button doubleButton;
    public Button playAgainButton;
    public int l;
    public int m;
    public bool playerBlackjack = false;
    public bool dealerBlackjack = false;
    public bool hasHit = false;
    public bool softHand;
    public bool canMakeMove;
    private float elapsedTime;
    private float timeWhenRestart;
    private int numberOfResets;
    [HideInInspector] public int currentRound = 0;

    public Text currentRoundText;
    public Text winnerText;
    public Text playerTotal;
    public Text dealerTotal;

    public PlayerBot playerBot;

    public enum LastHandOutcome {Win, Loss, Draw}
    public LastHandOutcome lastHandOutcome;

    public bool simulationEnd = false;

    public List<int> deathRoundList = new List<int>();
    public GameObject canvas1;
    public GameObject canvas2;
    private Transform camTransform;
    
    /*
     * Cards dealt to each player
     * First player hits/stands/bust
     * Dealer's turn; must have minimum of 17 score hand
     * Dealers cards; first card is hidden, subsequent cards are facing
     */

    #region Public methods

    public void Hit()
    {
        l++;
        hasHit = true;
        softHand = false;
        player.Push(deck.Pop());
        doubleButton.interactable = false;
        if (player.HandValue() > 21)
        {
            hitButton.interactable = false;
            standButton.interactable = false;
            doubleButton.interactable = false;
            StartCoroutine(DealersTurn());
        }
        StartCoroutine(PlayerMakeMove());
    }

    public void Stand()
    {
        hitButton.interactable = false;
        softHand = false;
        standButton.interactable = false;
        doubleButton.interactable = false;
        StartCoroutine(DealersTurn());
    }

    public void DoubleDown()
    {
        hitButton.interactable = false;
        softHand = false;
        standButton.interactable = false;
        doubleButton.interactable = false;

        playerBot.totalAmountMoney -= playerBot.betAmountMoney;
        playerBot.betAmountMoney += playerBot.betAmountMoney;
        player.Push(deck.Pop());

        StartCoroutine(DealersTurn());
    }

    public void PlayAgain()
    {
        timeWhenRestart = Time.time;
        playAgainButton.interactable = false;
        softHand = false;
        playerBlackjack = false;
        dealerBlackjack = false;
        hasHit = false;
        softHand = false;

        player.GetComponent<CardStackView>().Clear();
        dealer.GetComponent<CardStackView>().Clear();
        deck.GetComponent<CardStackView>().Clear();
        deck.CreateDeck();

        winnerText.text = "";

        hitButton.interactable = true;
        standButton.interactable = true;
        doubleButton.interactable = true;

        dealersFirstCard = -1;

        playerBot.betAmountMoney = 0;
    }

    #endregion

    #region Unity messages

    void Start()
    {
        playerBot = GameObject.Find("PlayerBot").GetComponent<PlayerBot>();
        graphController = GameObject.Find("GraphController").GetComponent<GraphController>();
        camTransform = GameObject.Find("Main Camera").GetComponent<Transform>();

        numberOfResets = 0;
        simulationEnd = false;
    }

    #endregion

    void StartGame()
    {
        currentRound++;
        graphController.UpdateGraph();
        if(currentRound > 1)
        {
            //graphController.UpdateGraph();
        }
        playerBot.BetMoney();
        currentRoundText.text = $"Round: {currentRound.ToString()}";
        l = 0;
        m = 0;
        for (int i = 0; i < 2; i++)
        {
            player.Push(deck.Pop());
            TotalText();
        }
        HitDealer();
    }

    public void ResetAll()
    {
        numberOfResets++;
        if(numberOfResets >= numberOfLoops)
        {
            simulationEnd = true;

            EndOfSimulation();
        }
        deathRoundList.Add(currentRound);
        currentRound = 0;
        playerBot.totalAmountMoney = playerBot.startingAmountMoney;
    }

    public void EndOfSimulation()
    {
        canvas1.SetActive(false);
        canvas2.SetActive(true);
        camTransform.position = new Vector3(-5.5f, 6f, -10f);

    }

    void HitDealer()
    {
        int card = deck.Pop();

        if (dealersFirstCard < 0)
        {
            dealersFirstCard = card;
        }

        dealer.Push(card);
        if (dealer.CardCount >= 2)
        {
            CardStackView view = dealer.GetComponent<CardStackView>();
            view.Toggle(card, true);
        }
        CardStackView views = dealer.GetComponent<CardStackView>();
        views.Toggle(dealersFirstCard, true);
    }

    void Update()
    {
        if(simulationEnd)
        {
            return;
        }

        if(!softHand)
        {
            if(player.aces > 0 && !hasHit)
            {
                softHand = true;
            }
        }

        TotalText();
        WaitForBet();
        if(l < 1 && int.Parse(playerTotal.text) == 21)
        {
            playerBlackjack = true;
            Stand();
            l++;
        }

        RestartIfStuck();
    }

    void RestartIfStuck()
    {
        if(Time.time - timeWhenRestart > 15)
        {
            PlayAgain();
        }
    }

    void WaitForBet()
    {
        if(playerBot.betHasBeenMade)
        {
            if(playerBot.betAmountMoney > 0)
            {
                StartGame();

                hitButton.interactable = true;
                standButton.interactable = true;
                doubleButton.interactable = true;
                
                
                playerBot.betHasBeenMade = false;
                StartCoroutine(PlayerMakeMove());
            }
        }
    }

    void TotalText()
    {
        // Player
        playerTotal.text = player.HandValue().ToString();

        // Dealear
        dealerTotal.text = dealer.HandValue().ToString();
        
        //playerBot.HitTheButtons();
    }

    IEnumerator PlayerMakeMove()
    {
        yield return new WaitForSeconds(0.01f);
        canMakeMove = true;
    }

    IEnumerator DealersTurn()
    {
        hitButton.interactable = false;
        standButton.interactable = false;

        CardStackView view = dealer.GetComponent<CardStackView>();
        view.Toggle(dealersFirstCard, true);
        view.ShowCards();
        yield return new WaitForSeconds(1f);

        while (dealer.HandValue() < 17 && player.HandValue() <= 21)
        {
            HitDealer();
            m++;
            yield return new WaitForSeconds(0.01f);
        } 

        if(m < 2 && int.Parse(dealerTotal.text) == 21)
        {
            dealerBlackjack = true;
        }

        if(playerBlackjack && !dealerBlackjack)
        {
            winnerText.text = "You Win!";
            playerBot.totalAmountMoney += playerBot.lastBet * 2;
            playerBlackjack = false;
            dealerBlackjack = false;
        }
        else if(!playerBlackjack && dealerBlackjack)
        {
            winnerText.text = "You Lose!";
            playerBlackjack = false;
            dealerBlackjack = false;
        }
        else if(playerBlackjack && dealerBlackjack)
        {
            winnerText.text ="Draw!";
            playerBot.totalAmountMoney += playerBot.lastBet;
            playerBlackjack = false;
            dealerBlackjack = false;
        }
        else
        {
            if (player.HandValue() > 21 || (dealer.HandValue() > player.HandValue() && dealer.HandValue() <= 21))
            {
                winnerText.color = Color.red;
                winnerText.text = "You Lose!";
                lastHandOutcome = LastHandOutcome.Loss;
                playerBot.actualDifference = playerBot.difference;
            }
            else if ((player.HandValue() <= 21 && player.HandValue() > dealer.HandValue()) || (player.HandValue() <= 21 && dealer.HandValue() > 21))
            {
                winnerText.color = Color.green;
                winnerText.text = "You Win!";
                lastHandOutcome = LastHandOutcome.Win;
                playerBot.totalAmountMoney += playerBot.lastBet * 2;
                playerBot.actualDifference = playerBot.difference;
            }
            else if((dealer.HandValue() > 21 && player.HandValue() > 21) || (dealer.HandValue() == player.HandValue()))
            {
                winnerText.color = Color.gray;
                winnerText.text ="Draw!";
                lastHandOutcome = LastHandOutcome.Draw;
                playerBot.totalAmountMoney += playerBot.lastBet;
                playerBot.actualDifference = playerBot.difference;
            }
            else
            {
                winnerText.text = "The house wins!";
            }
        }

        yield return new WaitForSeconds(0.01f);
        playAgainButton.interactable = true;
    }
}
