using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
    int dealersFirstCard = -1;

    public CardStack player;
    public CardStack dealer;
    public CardStack deck;

    public Button hitButton;
    public Button standButton;
    public Button doubleButton;
    public Button playAgainButton;
    public int l;
    public int m;
    public bool playerBlackjack = false;
    public bool dealerBlackjack = false;

    public Text winnerText;
    public Text playerTotal;
    public Text dealerTotal;

    public PlayerBot playerBot;
    
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
        player.Push(deck.Pop());
        doubleButton.interactable = false;
        if (player.HandValue() > 21)
        {
            hitButton.interactable = false;
            standButton.interactable = false;
            doubleButton.interactable = false;
            StartCoroutine(DealersTurn());
        }
    }

    public void Stand()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
        doubleButton.interactable = false;
        StartCoroutine(DealersTurn());
    }

    public void DoubleDown()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
        doubleButton.interactable = false;

        playerBot.totalAmountMoney -= playerBot.betAmountMoney;
        playerBot.betAmountMoney += playerBot.betAmountMoney;
        player.Push(deck.Pop());

        StartCoroutine(DealersTurn());
    }

    public void PlayAgain()
    {
        playAgainButton.interactable = false;
        playerBlackjack = false;
        dealerBlackjack = false;

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
    }

    #endregion

    void StartGame()
    {
        l = 0;
        m = 0;
        for (int i = 0; i < 2; i++)
        {
            player.Push(deck.Pop());
            TotalText();
        }
        HitDealer();
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
    }

    void Update()
    {        
        TotalText();
        WaitForBet();
        if(l < 1 && int.Parse(playerTotal.text) == 21)
        {
            playerBlackjack = true;
            Stand();
            l++;
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
            }
        }
    }

    void TotalText()
    {
        // Player
        playerTotal.text = player.HandValue().ToString();

        // Dealear
        dealerTotal.text = dealer.HandValue().ToString();

        playerBot.HitTheButtons();
    }

    IEnumerator DealersTurn()
    {
        hitButton.interactable = false;
        standButton.interactable = false;

        CardStackView view = dealer.GetComponent<CardStackView>();
        view.Toggle(dealersFirstCard, true);
        view.ShowCards();
        yield return new WaitForSeconds(1f);

        while (dealer.HandValue() < 17)
        {
            HitDealer();
            m++;
            yield return new WaitForSeconds(1f);
        } 

        if(m < 2 && int.Parse(dealerTotal.text) == 21)
        {
            dealerBlackjack = true;
        }

        if(playerBlackjack && !dealerBlackjack)
        {
            winnerText.text = "Winner, winner! Chicken dinner";
            playerBot.totalAmountMoney += playerBot.bettingAmount * 2;
            playerBlackjack = false;
            dealerBlackjack = false;
        }
        else if(!playerBlackjack && dealerBlackjack)
        {
            winnerText.text = "Sorry-- you lose";
            playerBlackjack = false;
            dealerBlackjack = false;
        }
        else if(playerBlackjack && dealerBlackjack)
        {
            winnerText.text ="Draw!";
            playerBot.totalAmountMoney += playerBot.bettingAmount;
            playerBlackjack = false;
            dealerBlackjack = false;
        }
        else
        {
            if ((player.HandValue() > 21 && dealer.HandValue() <= 21) || (dealer.HandValue() > player.HandValue() && dealer.HandValue() <= 21))
            {
                winnerText.text = "Sorry-- you lose";
            }
            else if ((player.HandValue() <= 21 && player.HandValue() > dealer.HandValue()) || (player.HandValue() <= 21 && dealer.HandValue() > 21))
            {
                winnerText.text = "Winner, winner! Chicken dinner";
                playerBot.totalAmountMoney += playerBot.bettingAmount * 2;
            }
            else if((dealer.HandValue() > 21 && player.HandValue() > 21) || (dealer.HandValue() == player.HandValue()))
            {
                winnerText.text ="Draw!";
                playerBot.totalAmountMoney += playerBot.bettingAmount;
            }
            else
            {
                winnerText.text = "The house wins!";
            }
        }

        yield return new WaitForSeconds(1f);
        playAgainButton.interactable = true;
    }
}
