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
            countScore(playerCard.value);
            playerScoreText.text = $"{playerScore}";
            HasPlayerBust(); 
        }

        if (!isPlayersTurn)
        { 
            Debug.Log("card for dealer ..");
            Card dealerCard = Instantiate(deck[Random.Range(0, deck.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            dealerCard.transform.SetParent(dealerArea.transform, false);
            dealerCards.Add(dealerCard);
            countScore(dealerCard.value);
            dealerScoreText.text = $"{dealerScore}";
            // has dealer bust function or has bust universal function, that checkss all scores
        } 
    }

    private int countScore(int value) // combine score methods, ace logic
    {
        if (!isDealersTurn)
        {
            playerScore += value;
            return playerScore;
        }
        else
        {
            dealerScore += value;
            return dealerScore;
        }
    }

    public void HasPlayerBust()
    {
        if (playerScore >= 22)
        {
            isDealersTurn = true;
            GameOver("bust");
            Debug.Log($"Player has bust with score of {playerScore}");
            
        }
    }

    public void GameOver(string text)
    {
        Debug.Log(text);
        Debug.Log("dealer turn...");

    }
}
