using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;


public class DeckManager : MonoBehaviour, IPointerClickHandler
{
    public int playerScore;
    public int dealerScore;
    private bool isDealersTurn = false;

    public GameObject cardPrefab;
    public List<Card> deck;

    public GameObject playerArea;
    public GameObject dealerArea;
    public TMP_Text playerScoreText;
    public TMP_Text dealerScoreText;

    public Button stand;

    public List<Card> playerCards;
    public List<Card> dealerCards;


    void Start()
    {

        StartGame();
    }

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

    public void StartGame() // should only be called once at the start of each round.
    {
        playerScore = 0;
        dealerScore = 0;

        foreach (var suit in System.Enum.GetValues(typeof(SuitsEnum)))
        {
            CreateCard(suit);
        }

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
        if (isDealersTurn)
        {
            DrawCard(false);
        }
        if (!isDealersTurn)
        {
            DrawCard(true);
        }
    }

    public void DrawCard(bool isPlayersTurn)
    {
        if (isPlayersTurn)
        {
            Debug.Log("Drawing card for player..");
            Card playerCard = Instantiate(deck[Random.Range(0, deck.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(playerArea.transform, false);
            playerCards.Add(playerCard);

            deck.Remove(playerCard); // test remove card

            Debug.Log(deck.Count);

            CountPlayerScore(playerCard.value);
            Debug.Log($"current scor for player; {playerScore}");
            playerScoreText.text = $"{playerScore}";
            HasPlayerTwentyOne();
            HasPlayerBust();
        }

        if (!isPlayersTurn)
        {
            Debug.Log("card for dealer ..");
            Card dealerCard = Instantiate(deck[Random.Range(0, deck.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            dealerCard.transform.SetParent(dealerArea.transform, false);
            dealerCards.Add(dealerCard);

            deck.Remove(dealerCard);

            Debug.Log(deck.Count);

            CountDealerScore(dealerCard.value);
            Debug.Log($"current scor for dealer; {dealerScore}");
            dealerScoreText.text = $"{dealerScore}";
            HasDealerTwentyOne();
            HasDealerBust();
            // has dealer bust function or has bust universal function, that checkss all scores
        }
    }

    public void DealerStandConditions()
    {
        // dealer stands on 18 or higher
        if (dealerScore <= 20 && dealerScore >= 18)
        {
            Debug.Log($"Dealer stands on {dealerScore}");
            dealerScoreText.text = $"Stands on {dealerScore}";
            CompareScores();
        }
    }
    
    public void Stand()
    {
        isDealersTurn = true;
        playerScoreText.text = $"Player stands on {playerScore}";
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

    public void HasPlayerBust()
    {
        if (playerScore >= 22)
        {
            isDealersTurn = true;
            Debug.Log($"Player has bust with score of {playerScore}");
            playerScoreText.text = $"Bust with {playerScore}";
        }
    }

    public void HasDealerBust()
    {
        if (dealerScore >= 22)
        {
            isDealersTurn = false;
            GameOver("Dealer has bust.");
        }
    }

    public void HasPlayerTwentyOne()
    {
        if (playerScore == 21)
        {
            Debug.Log("Player has blackjack!");
            playerScoreText.text = $"{playerScore} Blackjack!";
            isDealersTurn = true;
        }
    }

    public void HasDealerTwentyOne()
    {
        if (dealerScore == 21)
        {
            isDealersTurn = false;
            GameOver("Dealer has blackjack!");
        }
    }
    
    public void CompareScores()
    {
        if (dealerScore == playerScore)
        {
            Debug.Log("Its a tie.");
            GameOver($"Its a tie.");
        }

        else if (dealerScore > playerScore)
        {
            Debug.Log("Dealer wins");
            GameOver($"dealer wins with score of {dealerScore}");
        }

        else
        {
            Debug.Log("Player Wins");
            GameOver($"Player wins with a score of {playerScore}");
        }
    }

    public void GameOver(string text)
    {
        Debug.Log(text);
        Debug.Log("game over");
        // clear UI cards, score keeping, and restart game. Destroy(object, time);

        GameObject[] usedCards = GameObject.FindGameObjectsWithTag("Card");
        foreach(GameObject card in usedCards)
        {
            Destroy(card);
        }
        StartGame();
    }
}
