using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class RideTheBusGameManager : MonoBehaviour
{
    [Header("Text & Inputs")]
    public TMP_InputField betInput;
    public TMP_Text creditsText;
    public TMP_Text feedbackText;
    public TMP_Text cashOutText;
    public TMP_Text initialBet;
    public TMP_Text[] cardMessages;

    [Header("Buttons")]
    public Button betButton;
    public Button redButton, blackButton;
    public Button higherButton, lowerButton;
    public Button insideButton, outsideButton;
    public Button heartsButton, diamondsButton, clubsButton, spadesButton;
    public Button cashOutButton;

    [Header("Images")]
    public Image[] cardImages;

    [Header("Game Values")]
    private float currentMultiplier = 1f;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip placeBetSFX;
    public AudioClip cardFlipSFX;
    public AudioClip winMoneySFX;

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
        cashOutText.gameObject.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        UpdateCreditsUI();
        UpdateInstructions();
    }

    private void UpdateInstructions()
    {
        if (currentState == GameState.RedOrBlack)
        {
            feedbackText.text = "Will the First Card be Red or Black?";
        }

        if (currentState == GameState.HigherOrLower)
        {
            feedbackText.text = "Will the Second Card be Higher or Lower than " + drawnCards[0].value + "?";
        }

        if (currentState == GameState.InsideOrOutside)
        {
            feedbackText.text = "Will the Third Card be inside or outside the range " + drawnCards[0].value + " - " + drawnCards[1].value + "?";
        }

        if (currentState == GameState.GuessSuit)
        {
            feedbackText.text = "What Suit will the Final Card Be?";
        }
    }

    private void UpdateCreditsUI()
    {
        creditsText.text = "Credits: " + PlayerPrefs.GetInt("Credits");
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;    

        cashOutButton.gameObject.SetActive(newState != GameState.WaitingForBet && newState != GameState.GameOver);

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

        cashOutButton.gameObject.SetActive(
        newState == GameState.HigherOrLower ||
        newState == GameState.InsideOrOutside ||
        newState == GameState.GuessSuit);

        initialBet.gameObject.SetActive(
        newState == GameState.RedOrBlack ||
        newState == GameState.HigherOrLower ||
        newState == GameState.InsideOrOutside ||
        newState == GameState.GuessSuit);
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

        audioSource.PlayOneShot(placeBetSFX);

        PlayerPrefs.SetInt("Credits", credits - betAmount);
        UpdateInitialBetText();
        UpdateCreditsUI();
        StartNewRound();
    }

    public void OnCashOut()
    {
        int winnings = Mathf.RoundToInt(betAmount * currentMultiplier);
        int credits = PlayerPrefs.GetInt("Credits");
        PlayerPrefs.SetInt("Credits", credits + winnings);
        cashOutButton.gameObject.SetActive(false);
        cashOutText.gameObject.SetActive(false);
        UpdateCreditsUI();

        audioSource.PlayOneShot(winMoneySFX);
        audioSource.PlayOneShot(placeBetSFX);

        feedbackText.text = $"Cashed out for +{winnings} credits!";
        SetGameState(GameState.WaitingForBet);
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

    private void UpdateCashOutText()
    {
        int potentialWinnings = Mathf.RoundToInt(betAmount * currentMultiplier);
        cashOutText.text = $"Cash Out: {potentialWinnings} credits";
    }

    private void UpdateInitialBetText()
    {
        initialBet.text = "Initial Bet: " + betAmount;
    }

    public void OnRedOrBlackGuess(string guess)
    {
        Card firstCard = drawnCards[0];
        bool isRed = (firstCard.suit == "Hearts" || firstCard.suit == "Diamonds");
        bool correct = (guess == "Red" && isRed) || (guess == "Black" && !isRed);

        StartCoroutine(RevealCard(0, correct));

        if (correct)
        {
            SetGameState(GameState.HigherOrLower);
            currentMultiplier = 2f;
            cashOutButton.gameObject.SetActive(true);
            cashOutText.gameObject.SetActive(true);
            UpdateCashOutText();
        }
        else
        {
            EndGame("Wrong! Try again.");
            cashOutButton.gameObject.SetActive(false);
            cashOutText.gameObject.SetActive(false);
        }

    }

    public void OnHigherLowerGuess(string guess)
    {
        int first = drawnCards[0].value;
        int second = drawnCards[1].value;

        bool correct = (guess == "Higher" && second > first) || (guess == "Lower" && second < first);

        StartCoroutine(RevealCard(1, correct));

        if (correct)
        {
            SetGameState(GameState.InsideOrOutside);
            currentMultiplier = 4f;
            UpdateCashOutText();
        }
        else
        {
            EndGame("Incorrect! Game over.");
            cashOutButton.gameObject.SetActive(false);
            cashOutText.gameObject.SetActive(false);
        }

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

        if (correct)
        {
            SetGameState(GameState.GuessSuit);
            currentMultiplier = 8f; 
            UpdateCashOutText();
        }
        else
        {
            EndGame("Nope! Try again.");
            cashOutButton.gameObject.SetActive(false);
            cashOutText.gameObject.SetActive(false);
        }

    }

    public void OnSuitGuess(string guessSuit)
    {
        Card fourthCard = drawnCards[3];
        bool correct = fourthCard.suit == guessSuit;

        StartCoroutine(RevealCard(3, correct));

        if (correct)
        {
            int winnings = betAmount * 35;
            PlayerPrefs.SetInt("Credits", PlayerPrefs.GetInt("Credits") + winnings);
            UpdateCreditsUI();
            feedbackText.text = "You won! +" + winnings + " credits.";
            cashOutText.gameObject.SetActive(false);

            audioSource.PlayOneShot(winMoneySFX);
            audioSource.PlayOneShot(placeBetSFX);
        }
        else
        {
            feedbackText.text = "Wrong suit. Better luck next time.";
            cashOutText.gameObject.SetActive(false);
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
        audioSource.PlayOneShot(cardFlipSFX);

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
        SceneHelper.SaveAndLoadScene("MainScene");
        //LoadingScreenManager.Instance.LoadSceneWithLoadingScreen("MainScene");
    }

    [System.Serializable]
    public class Card
    {
        public int value;
        public string suit;
    }
}