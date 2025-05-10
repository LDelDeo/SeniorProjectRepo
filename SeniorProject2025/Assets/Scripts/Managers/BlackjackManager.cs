using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BlackjackManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text resultText;
    public TMP_Text matchResultText;
    public TMP_Text playerCreditsText;

    public Button hitButton;
    public Button standButton;
    public Button restartButton;
    public Button placeBetButton;
    public Button exitButton;

    public TMP_InputField betInputField;
    public TMP_InputField matchBetInputField;

    public Transform playerHandArea;
    public Transform dealerHandArea;

    public PlayerData playerData;

    private List<Card> deck;
    private List<Card> playerHand;
    private List<Card> dealerHand;

    private bool gameOver = false;
    private int currentBet = 0;
    private int matchBet = 0;

    void Start()
    {
        placeBetButton.onClick.AddListener(PlaceBet);
        hitButton.onClick.AddListener(OnHit);
        standButton.onClick.AddListener(OnStand);
        restartButton.onClick.AddListener(OnRestart);
        exitButton.onClick.AddListener(OnExit);

        DisableMainButtons();
        resultText.text = "Place your bets to begin.";
        UpdateCreditsUI();
    }

    void PlaceBet()
    {
        if (!int.TryParse(betInputField.text, out currentBet) || currentBet <= 0)
        {
            resultText.text = "Invalid main bet.";
            return;
        }

        if (!int.TryParse(matchBetInputField.text, out matchBet)) matchBet = 0;

        if (currentBet + matchBet > playerData.credits)
        {
            resultText.text = "Not enough credits.";
            return;
        }

        playerData.SpendCredits(currentBet + matchBet);
        UpdateCreditsUI();
        StartCoroutine(DealInitialCards());
    }

    IEnumerator DealInitialCards()
    {
        deck = GenerateDeck();
        ShuffleDeck();
        playerHand = new List<Card>();
        dealerHand = new List<Card>();

        ClearHandArea(playerHandArea);
        ClearHandArea(dealerHandArea);

        yield return DealCard(playerHand, playerHandArea);
        yield return DealCard(dealerHand, dealerHandArea);
        yield return DealCard(playerHand, playerHandArea);
        yield return DealCard(dealerHand, dealerHandArea, faceDown: true);

        gameOver = false;
        resultText.text = "";
        matchResultText.text = "";

        hitButton.interactable = true;
        standButton.interactable = true;
        placeBetButton.interactable = false;
        betInputField.interactable = false;
        matchBetInputField.interactable = false;

        CheckMatchDealer();
    }

    IEnumerator DealCard(List<Card> hand, Transform area, bool faceDown = false)
    {
        Card card = DrawCard();
        hand.Add(card);

        GameObject cardPrefab = null;

        if (faceDown)
        {
            string[] backs = { "Card_Back_Red", "Card_Back_Blue" };
            string selectedBack = backs[Random.Range(0, backs.Length)];
            cardPrefab = Resources.Load<GameObject>("Cards/" + selectedBack);
        }
        else
        {
            string prefabName = $"Card_{CardFace(card.value)}_{card.suit}";
            cardPrefab = Resources.Load<GameObject>("Cards/" + prefabName);
        }

        if (cardPrefab != null)
        {
            GameObject cardGO = Instantiate(cardPrefab, area);
            cardGO.transform.localScale = Vector3.zero;

            float time = 0.2f;
            Vector3 targetScale = Vector3.one;
            while (time > 0)
            {
                cardGO.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, 1 - time / 0.2f);
                time -= Time.deltaTime;
                yield return null;
            }
            cardGO.transform.localScale = targetScale;

            if (faceDown)
            {
                CardVisual cv = cardGO.GetComponent<CardVisual>();
                if (cv != null)
                    cv.SetHiddenCard(card);
            }
        }
        else
        {
            Debug.LogWarning("Missing card prefab.");
        }

        yield return new WaitForSeconds(0.1f);
    }

    void EndGame(bool playerWon, bool tie = false)
    {
        gameOver = true;
        DisableMainButtons();

        if (tie)
        {
            resultText.text = "It's a tie! Bet returned.";
            playerData.AddCredits(currentBet);
        }
        else if (playerWon)
        {
            resultText.text = "You win! +" + (currentBet * 2);
            playerData.AddCredits(currentBet * 2);
        }
        else
        {
            resultText.text = "You lose.";
        }

        FlipDealerSecondCard();
        UpdateCreditsUI();
    }

    void FlipDealerSecondCard()
    {
        if (dealerHandArea.childCount < 2) return;

        Transform secondCard = dealerHandArea.GetChild(1);
        CardVisual cv = secondCard.GetComponent<CardVisual>();
        if (cv != null)
            cv.FlipToFront(dealerHand[1]);
    }

    void CheckMatchDealer()
    {
        if (matchBet <= 0) return;

        int playerFirst = playerHand[0].value;
        int dealerUp = dealerHand[0].value;

        if (playerFirst == dealerUp)
        {
            matchResultText.text = "Match Dealer! +" + (matchBet * 2);
            playerData.AddCredits(matchBet * 2);
        }
        else
        {
            matchResultText.text = "No match.";
        }

        UpdateCreditsUI();
    }

    public void OnHit()
    {
        if (gameOver) return;
        StartCoroutine(DealCard(playerHand, playerHandArea));
        StartCoroutine(CheckBustAfterDelay());
    }

    IEnumerator CheckBustAfterDelay()
    {
        yield return new WaitForSeconds(0.4f);
        if (GetHandValue(playerHand) > 21)
        {
            EndGame(false);
        }
    }

    public void OnStand()
    {
        if (gameOver) return;
        StartCoroutine(DealerPlay());
    }

    IEnumerator DealerPlay()
    {
        FlipDealerSecondCard();
        yield return new WaitForSeconds(0.5f);

        while (GetHandValue(dealerHand) < 17)
        {
            yield return DealCard(dealerHand, dealerHandArea);
        }

        int playerTotal = GetHandValue(playerHand);
        int dealerTotal = GetHandValue(dealerHand);

        if (dealerTotal > 21 || playerTotal > dealerTotal)
        {
            EndGame(true);
        }
        else if (dealerTotal == playerTotal)
        {
            EndGame(false, true);
        }
        else
        {
            EndGame(false);
        }
    }

    public void OnRestart()
    {
        resultText.text = "Place your bets to begin.";
        matchResultText.text = "";

        currentBet = 0;
        matchBet = 0;

        placeBetButton.interactable = true;
        betInputField.text = "";
        matchBetInputField.text = "";
        betInputField.interactable = true;
        matchBetInputField.interactable = true;

        ClearHandArea(playerHandArea);
        ClearHandArea(dealerHandArea);

        DisableMainButtons();
        UpdateCreditsUI();
    }

    public void OnExit()
    {
        SceneManager.LoadScene("MainScene");
    }

    void DisableMainButtons()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
    }

    void ClearHandArea(Transform area)
    {
        foreach (Transform child in area)
        {
            Destroy(child.gameObject);
        }
    }

    List<Card> GenerateDeck()
    {
        List<Card> newDeck = new();
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        foreach (string suit in suits)
            for (int i = 1; i <= 13; i++)
                newDeck.Add(new Card(i, suit));
        return newDeck;
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int r = Random.Range(0, deck.Count);
            (deck[i], deck[r]) = (deck[r], deck[i]);
        }
    }

    Card DrawCard()
    {
        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    int GetHandValue(List<Card> hand)
    {
        int total = 0, aces = 0;
        foreach (Card card in hand)
        {
            total += Mathf.Min(card.value, 10);
            if (card.value == 1) aces++;
        }
        while (aces > 0 && total + 10 <= 21)
        {
            total += 10;
            aces--;
        }
        return total;
    }

    string CardFace(int value) => value switch
    {
        1 => "A",
        11 => "J",
        12 => "Q",
        13 => "K",
        _ => value.ToString()
    };

    void UpdateCreditsUI()
    {
        if (playerCreditsText != null && playerData != null)
        {
            playerCreditsText.text = "Credits: " + playerData.credits;
        }
    }
}