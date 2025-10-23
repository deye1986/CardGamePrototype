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

    public List<Card> playerCards;
    public List<Card> dealerCards;


    void Start()
    {
        foreach (var suit in System.Enum.GetValues(typeof(SuitsEnum)))
        {
            CreateCard(suit);
        }
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

    public void StartGame() // should only be called once at the start of the game.
    {
        playerScore = 0;
        dealerScore = 0;

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
            foreach(Card i in deck) // check deck after removing
            {
                Debug.Log($"{i.suit}: {i.value}");
            }
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
            foreach (Card i in deck)
            {
                Debug.Log($"{i.suit}: {i.value}");
            }
            Debug.Log(deck.Count);
            
            CountDealerScore(dealerCard.value);
            Debug.Log($"current scor for dealer; {dealerScore}");
            dealerScoreText.text = $"{dealerScore}";
            HasDealerTwentyOne();
            // has dealer bust function or has bust universal function, that checkss all scores
        } 
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
        }
    }

    public void HasDealerBust()
    {
        if (dealerScore >= 22)
        {
            isDealersTurn = false;
            Debug.Log("dealer has bust");
        }
    }

    public void HasPlayerTwentyOne()
    {
        if (playerScore == 21)
        {
            Debug.Log("Player has blackjack!");
            isDealersTurn = true;
        }
    }
    
    public void HasDealerTwentyOne()
    {
        if (dealerScore == 21)
        {
            Debug.Log("Dealer has blackjack!");
            isDealersTurn = false;
        }
    }

    public void GameOver(string text)
    {
        Debug.Log(text);
        Debug.Log("game over");
        // clear UI cards, score keeping, and restart game. 
    }
}
