using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class RideTheBusGameManager : MonoBehaviour
{
    [Header("Text & Inputs")]
    public TMP_InputField betInput;
    public TMP_Text creditsText;
    public TMP_Text feedbackText;
    public TMP_Text[] cardMessages;

    [Header("Buttons")]
    public Button betButton;
    public Button redButton, blackButton;
    public Button higherButton, lowerButton;
    public Button insideButton, outsideButton;
    public Button heartsButton, diamondsButton, clubsButton, spadesButton;

    [Header("Images")]
    public Image[] cardImages;

    private enum GameState { WaitingForBet, RedOrBlack, HigherOrLower, InsideOrOutside, GuessSuit, GameOver }
    private GameState currentState = GameState.WaitingForBet;

    private List<Card> deck;
    private List<Card> drawnCards;
    private int betAmount;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Credits")) PlayerPrefs.SetInt("Credits", 1000);
        UpdateCreditsUI();
        SetGameState(GameState.WaitingForBet);
    }

    private void Update()
    {
        UpdateCreditsUI();
    }

    private void UpdateCreditsUI()
    {
        creditsText.text = "Credits: " + PlayerPrefs.GetInt("Credits");
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;    

        redButton.gameObject.SetActive(newState == GameState.RedOrBlack);
        blackButton.gameObject.SetActive(newState == GameState.RedOrBlack);

        higherButton.gameObject.SetActive(newState == GameState.HigherOrLower);
        lowerButton.gameObject.SetActive(newState == GameState.HigherOrLower);

        insideButton.gameObject.SetActive(newState == GameState.InsideOrOutside);
        outsideButton.gameObject.SetActive(newState == GameState.InsideOrOutside);

        bool suitGuess = newState == GameState.GuessSuit;
        heartsButton.gameObject.SetActive(suitGuess);
        diamondsButton.gameObject.SetActive(suitGuess);
        clubsButton.gameObject.SetActive(suitGuess);
        spadesButton.gameObject.SetActive(suitGuess);

        bool isWaiting = newState == GameState.WaitingForBet;
        betInput.gameObject.SetActive(isWaiting);
        betButton.gameObject.SetActive(isWaiting);
    }


    public void OnPlaceBet()
    {
        if (!int.TryParse(betInput.text, out betAmount) || betAmount <= 0)
        {
            feedbackText.text = "Enter a valid bet.";
            return;
        }

        int credits = PlayerPrefs.GetInt("Credits");
        if (betAmount > credits)
        {
            feedbackText.text = "Not enough credits!";
            return;
        }

        PlayerPrefs.SetInt("Credits", credits - betAmount);
        UpdateCreditsUI();
        StartNewRound();
    }

    private void StartNewRound()
    {
        deck = CreateDeck();
        drawnCards = deck.OrderBy(c => Random.value).Take(4).ToList();

        ResetCardBacks();
        ClearAllCardMessages();
        feedbackText.text = "";
        SetGameState(GameState.RedOrBlack);
    }

    private void ResetCardBacks()
    {
        foreach (var image in cardImages)
        {
            if (image != null)
                image.gameObject.SetActive(true);
        }

        foreach (Transform t in cardImages[0].transform.parent)
        {
            if (t.name.StartsWith("Card_")) Destroy(t.gameObject);
        }
    }

    public void OnRedOrBlackGuess(string guess)
    {
        Card firstCard = drawnCards[0];
        bool isRed = (firstCard.suit == "Hearts" || firstCard.suit == "Diamonds");
        bool correct = (guess == "Red" && isRed) || (guess == "Black" && !isRed);

        StartCoroutine(RevealCard(0, correct));

        if (correct) SetGameState(GameState.HigherOrLower);
        else EndGame("Wrong! Try again.");
    }

    public void OnHigherLowerGuess(string guess)
    {
        int first = drawnCards[0].value;
        int second = drawnCards[1].value;

        bool correct = (guess == "Higher" && second > first) || (guess == "Lower" && second < first);

        StartCoroutine(RevealCard(1, correct));

        if (correct) SetGameState(GameState.InsideOrOutside);
        else EndGame("Incorrect! Game over.");
    }

    public void OnInsideOutsideGuess(string guess)
    {
        int a = drawnCards[0].value;
        int b = drawnCards[1].value;
        int c = drawnCards[2].value;

        int min = Mathf.Min(a, b);
        int max = Mathf.Max(a, b);

        bool isInside = c > min && c < max;
        bool correct = (guess == "Inside" && isInside) || (guess == "Outside" && !isInside);

        StartCoroutine(RevealCard(2, correct));

        if (correct) SetGameState(GameState.GuessSuit);
        else EndGame("Nope! Try again.");
    }

    public void OnSuitGuess(string guessSuit)
    {
        Card fourthCard = drawnCards[3];
        bool correct = fourthCard.suit == guessSuit;

        StartCoroutine(RevealCard(3, correct));

        if (correct)
        {
            int winnings = betAmount * 20;
            PlayerPrefs.SetInt("Credits", PlayerPrefs.GetInt("Credits") + winnings);
            UpdateCreditsUI();
            feedbackText.text = "You won! +" + winnings + " credits.";
        }
        else
        {
            feedbackText.text = "Wrong suit. Better luck next time.";
        }

        SetGameState(GameState.WaitingForBet);
    }

    private void EndGame(string message)
    {
        feedbackText.text = message;
        SetGameState(GameState.WaitingForBet);
    }

    private IEnumerator RevealCard(int index, bool? correct = null)
    {
        if (cardImages == null || cardImages.Length <= index || cardImages[index] == null)
        {
            Debug.LogError("Invalid card image reference at index: " + index);
            yield break;
        }

        Card card = drawnCards[index];
        string cardValue = card.value.ToString();

        if (cardValue == "1") cardValue = "A";
        else if (cardValue == "11") cardValue = "J";
        else if (cardValue == "12") cardValue = "Q";
        else if (cardValue == "13") cardValue = "K";

        string prefabName = "Card_" + cardValue + "_" + card.suit;
        GameObject cardPrefab = Resources.Load<GameObject>("Cards/Sprites/" + prefabName);

        if (cardPrefab == null)
        {
            Debug.LogError($"Card prefab {prefabName} not found!");
            yield break;
        }

        GameObject instantiatedCard = Instantiate(cardPrefab, cardImages[index].transform.position, Quaternion.identity);
        instantiatedCard.transform.SetParent(cardImages[index].transform.parent, false);
        instantiatedCard.transform.SetSiblingIndex(cardImages[index].transform.GetSiblingIndex());
        cardImages[index].gameObject.SetActive(false);

        if (correct.HasValue)
        {
            string msg = correct.Value ? "Correct!" : "Incorrect!";
            Color color = correct.Value ? Color.green : Color.red;
            ShowCardMessage(index, msg, color);
        }

        yield return null;
    }

    private void ShowCardMessage(int index, string message, Color color)
    {
        if (cardMessages != null && index >= 0 && index < cardMessages.Length)
        {
            cardMessages[index].text = message;
            cardMessages[index].color = color;
            cardMessages[index].gameObject.SetActive(true);
        }
    }

    private void ClearAllCardMessages()
    {
        if (cardMessages == null) return;

        foreach (var msg in cardMessages)
        {
            if (msg != null)
            {
                msg.text = "";
                msg.gameObject.SetActive(false);
            }
        }
    }

    private List<Card> CreateDeck()
    {
        List<Card> deck = new List<Card>();
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };

        for (int value = 1; value <= 13; value++)
        {
            foreach (string suit in suits)
            {
                deck.Add(new Card { value = value, suit = suit });
            }
        }

        return deck;
    }

    public void OnExit()
    {
        SceneManager.LoadScene("MainScene");
    }

    [System.Serializable]
    public class Card
    {
        public int value;
        public string suit;
    }
}

