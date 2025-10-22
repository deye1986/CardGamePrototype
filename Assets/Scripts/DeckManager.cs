using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class DeckManager : MonoBehaviour, IPointerClickHandler
{
    public int playerScore;

    public GameObject cardPrefab;
    public List<Card> deck;

    public GameObject playerArea;
    public GameObject dealerArea;

    public TMP_Text playerScoreText;
    public TMP_Text dealerScoreText;


    public List<Card> playerCards;
    public List<Card> dealerCards;


    private int dealerScore; // ? already a value in the playerscoretext G.O.'s

    private bool isDealersTurn = false;

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

            card.cardArt = cardArt;

            deck.Add(card);
        }
    }

    public void StartGame()
    {
        playerScore = 0;
        for (int i = 0; i < 2; i++)
        {
            DrawCard();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DrawCard();
    }

    public void DrawCard()
    {
        Debug.Log("Drawing cards for player..");
        Card playerCard = Instantiate(deck[Random.Range(0, deck.Count)], new Vector3(0, 0, 0), Quaternion.identity);
        playerCard.transform.SetParent(playerArea.transform, false);
        playerCards.Add(playerCard);

        Debug.Log("cards for dealer ..");
        Card dealerCard = Instantiate(deck[Random.Range(0, deck.Count)], new Vector3(0, 0, 0), Quaternion.identity);
        dealerCard.transform.SetParent(dealerArea.transform, false);
        dealerCards.Add(dealerCard);

        AddToPlayerScore(playerCard.value);
        playerScoreText.text = $"{playerScore}";
        HasPlayerBust(); 

        AddToDealerScore(dealerCard.value);
        dealerScoreText.text = $"{dealerScore}";
        // has dealer bust function or has bust universal function, that checkss both participants scores
    }

    private int AddToPlayerScore(int value) // combine score methods 
    {
        if (value > 10)
        {
            value = 10;
        }

        // ace logic
        
        playerScore += value;
        return playerScore;
    }

    private int AddToDealerScore(int value) // combine score metods
    {
        if (value > 10)
        {
            value = 10;
        }

        // ace logic
        
        dealerScore += value;
        return dealerScore;
    }

    public void HasPlayerBust()
    {
        if (playerScore >= 22)
        {
            GameOver("bust");
        }
    }

    public void GameOver(string text)
    {
        Debug.Log(text);
        Debug.Log("dealer turn...");

    }
}
