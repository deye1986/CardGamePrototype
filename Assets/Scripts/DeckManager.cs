using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class DeckManager : MonoBehaviour, IPointerClickHandler
{
    public List<Card> playerCards; public List<Card> dealerCards; public List<Card> deck;
    public int playerScore; public int dealerScore;
    private bool isDealersTurn = false;
    public GameObject playerArea; public GameObject dealerArea; 
    public GameObject cardPrefab;
    public TMP_Text playerScoreText; public TMP_Text dealerScoreText;

    public Button stand;


    void Start()
    {
        foreach (var suit in System.Enum.GetValues(typeof(SuitsEnum)))
        {
            CreateCard(suit);
        }
        StartGame();
    }

    #region create card method
    public void CreateCard(object suit)
    {
        for (int value = 1; value <= 13; value++)
        {
            string path = $"{suit}/{value}";
            Sprite cardArt = Resources.Load<Sprite>(path);

            GameObject cardObject = Instantiate(cardPrefab, transform);
            Card card = cardObject.GetComponent<Card>();

            Transform suitTransform = cardObject.transform.Find("suit");
            Transform valueTransform = cardObject.transform.Find("value");
            Transform artTransform = cardObject.transform.Find("cardArt");

            TMP_Text suitText = cardObject.transform.Find("suit").GetComponent<TMP_Text>();
            TMP_Text valueText = cardObject.transform.Find("value").GetComponent<TMP_Text>();
            Image artImage = artTransform.GetComponent<Image>();

            artImage.sprite = cardArt;
            suitText.text = $"{suit}";
            valueText.text = $"{value}";

            card.suit = (SuitsEnum)suit;
            card.value = value;
            if (card.value > 10)
            {
                card.value = 10;
            }

            card.cardArt = cardArt;
            deck.Add(card);
        }
    }
    #endregion

    public void StartGame() // should only be called once at the start of each round.
    {
        playerScore = 0; dealerScore = 0;
        playerScoreText.text = $"{playerScore}";
        dealerScoreText.text = $"{dealerScore}";

        for (int i = 0; i < 2; i++)
        {
            DrawCard(true);
            DrawCard(false);
        }

        isDealersTurn = false;
        Debug.Log("Players turn..");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDealersTurn)
        {
            DrawCard(true);
        }
    }

    public async Task DrawCard(bool isPlayersTurn)
    {
        Debug.Log($"deck count: {deck.Count}"); // test the deck count after a card drawn
        if (isPlayersTurn)
        {
            Debug.Log("Drawing card for player..");
            await Task.Delay(500);
            Card playerCard = Instantiate(deck[Random.Range(0, deck.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(playerArea.transform, false);
            playerCards.Add(playerCard);

            deck.Remove(playerCard); // test remove card

            CountPlayerScore(playerCard.value);
            Debug.Log($"current score for player; {playerScore}");
            playerScoreText.text = $"{playerScore}";
            HasPlayerTwentyOne();
            HasPlayerBust();
        }

        if (!isPlayersTurn)
        {
            Debug.Log("card for dealer ..");
            await Task.Delay(500);
            Card dealerCard = Instantiate(deck[Random.Range(0, deck.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            dealerCard.transform.SetParent(dealerArea.transform, false);
            dealerCards.Add(dealerCard);

            deck.Remove(dealerCard); // not working properly

            CountDealerScore(dealerCard.value);
            Debug.Log($"current scor for dealer; {dealerScore}");
            dealerScoreText.text = $"{dealerScore}";
            HasDealerBlackjack();
            HasDealerTwentyOne();
            HasDealerBust();
        }
    }

    public async Task DealerStandConditions()
    {
        if (isDealersTurn)
        {
            while (dealerScore < 18)
            {
                await DrawCard(false);
            }

            if (dealerScore <= 21)
            {
            // dealer stands on 18 19 or 20
            Debug.Log($"Dealer stands on {dealerScore}");
            dealerScoreText.text = $"Stands on {dealerScore}";
            isDealersTurn = false;
            CompareScores();
            }
        }
    }
    
    public async void Stand()
    {
        isDealersTurn = true;
        playerScoreText.text = $"Stands on {playerScore}";
        await DealerStandConditions();
    }

    private int CountPlayerScore(int value) // ace logic
    {
        playerScore += value;
        return playerScore;
    }

    private int CountDealerScore(int value)
    {
        dealerScore += value;
        return dealerScore;
    }

    public async void HasPlayerBust()
    {
        if (playerScore >= 22)
        {
            isDealersTurn = true;
            Debug.Log($"Player has bust with score of {playerScore}");
            playerScoreText.text = $"Bust with {playerScore}";
            await DealerStandConditions();
        }
    }

    public void HasDealerBust()
    {
        if (dealerScore >= 22)
        {
            GameOver("Dealer has bust.");
        }
    }

    public async void HasPlayerTwentyOne()
    {
        if ((playerCards[0].value == 1 && playerCards[1].value == 10) || (playerCards[0].value == 10 && playerCards[1].value == 1))
        {
            Debug.Log("Player has blackjack!");
            playerScoreText.text = $"{playerScore} Blackjack!";
            isDealersTurn = true;
            await DealerStandConditions();
        }
    }

    public void HasDealerBlackjack()
    {
        if (dealerScore == 21 && dealerCards.Count == 2)
        {
            dealerScoreText.text = $"{dealerScore}Blackjack!";
            GameOver("Dealer has blackjack!");
        }
    }

    public void HasDealerTwentyOne()
    {
        if (dealerScore == 21)
        {
            dealerScoreText.text = $"Dealer has {dealerScore}!";
            GameOver("Dealer has 21!");
        }
    }
    
    public void CompareScores()
    {
        if (dealerScore == playerScore)
        {
            GameOver("Its a tie.");
        }
        else if (dealerScore > playerScore)
        {
            GameOver($"dealer wins with score of {dealerScore}");
        }
        else
        {
            if (playerScore <= 21)
            {
                GameOver($"Player wins with a score of {playerScore}");
            }
            else
            {
                GameOver($"dealer wins with score of {dealerScore}");
            }
            
        }
    }

    public async void GameOver(string text)
    {
        isDealersTurn = false;
        Debug.Log(text);
        await Task.Delay(200);
        GameObject[] usedCards = GameObject.FindGameObjectsWithTag("Card");

        foreach (GameObject card in usedCards)
        {
            Destroy(card);
            await Task.Delay(100);
        }
        Debug.Log("Starting new game.");
        StartGame();
    }
}
