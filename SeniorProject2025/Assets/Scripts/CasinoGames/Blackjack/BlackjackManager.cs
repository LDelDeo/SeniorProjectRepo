using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BlackjackManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text resultText;
    public TMP_Text matchResultText;
    public TMP_Text playerCreditsText;
    public TMP_Text playerPointsText;
    public TMP_Text dealerPointsText;

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
    private bool dealerHasHiddenCard = false;
    private Card hiddenDealerCard;
    private bool hasHiddenDealerCard = false;

    private int currentBet = 0;
    private int matchBet = 0;

    public AudioSource audioSource;
    public AudioClip placeBetSFX;
    public AudioClip cardFlipSFX;
    public AudioClip winMoneySFX;

    void Start()
    {
        placeBetButton.onClick.AddListener(PlaceBet);
        hitButton.onClick.AddListener(OnHit);
        standButton.onClick.AddListener(OnStand);
        restartButton.onClick.AddListener(OnRestart);
        exitButton.onClick.AddListener(OnExit);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        restartButton.interactable = false;
        DisableMainButtons();
        resultText.text = "Place your bets to begin.";
        
        StartCoroutine(DelayedUIRefresh());

    }

    void PlaceBet()
    {
        if (!int.TryParse(betInputField.text, out currentBet) || currentBet <= 0)
        {
            resultText.text = "Invalid main bet.";
            return;
        }

        if (!int.TryParse(matchBetInputField.text, out matchBet) || matchBet < 0)
        {
            matchBet = 0;
        }

        if (currentBet + matchBet > playerData.credits)
        {
            resultText.text = "Not enough credits.";
            return;
        }

        int handsPlayed = PlayerPrefs.GetInt("HandsPlayedBJ", 0);
        handsPlayed++;
        PlayerPrefs.SetInt("HandsPlayedBJ", handsPlayed);
        PlayerPrefs.Save();

        placeBetButton.interactable = false;
        betInputField.interactable = false;
        matchBetInputField.interactable = false;

        playerData.SpendCredits(currentBet + matchBet);
        audioSource.PlayOneShot(placeBetSFX);
        UpdateCreditsUI();
        StartCoroutine(DealInitialCards());
    }

    IEnumerator DealInitialCards()
    {
        deck = GenerateDeck();
        ShuffleDeck();
        ClearHandArea(playerHandArea);
        ClearHandArea(dealerHandArea);

        playerHand?.Clear();
        dealerHand?.Clear();
        playerHand = new List<Card>();
        dealerHand = new List<Card>();

        dealerPointsText.text = "Dealer: ?";
        playerPointsText.text = "Player: 0";

        yield return DealCard(playerHand, playerHandArea);
        yield return DealCard(dealerHand, dealerHandArea);
        yield return DealCard(playerHand, playerHandArea);
        Card hiddenCard = DrawCard();
        hiddenDealerCard = hiddenCard;
        hasHiddenDealerCard = true;
        yield return DealCard(null, dealerHandArea, faceDown: true);
        dealerHasHiddenCard = true;


        gameOver = false;
        resultText.text = "";
        matchResultText.text = "";

        hitButton.interactable = true;
        standButton.interactable = true;
        placeBetButton.interactable = false;
        betInputField.interactable = false;
        matchBetInputField.interactable = false;

        UpdatePointsUI();
        CheckMatchDealer();

        yield return new WaitForSeconds(0.1f);

        int playerTotal = GetHandValue(playerHand);
        if (playerTotal == 21 && playerHand.Count == 2)
        {
            HighlightPlayerCards(Color.green); // blackjack

            yield return new WaitForSeconds(0.4f);
            yield return StartCoroutine(EndGame(true));
        }
    }

    IEnumerator DealCard(List<Card> hand, Transform area, bool faceDown = false)
    {
        audioSource.PlayOneShot(cardFlipSFX);
        Card card = faceDown && hand == null ? hiddenDealerCard : DrawCard();
        if (hand != null)
        {
            hand.Add(card);
        }

        GameObject cardPrefab = null;
        if (faceDown)
        {
            string[] backs = { "Card_Back_Red", "Card_Back_Blue" };
            string selectedBack = backs[Random.Range(0, backs.Length)];
            cardPrefab = Resources.Load<GameObject>("Cards/Backs/" + selectedBack);
        }
        else
        {
            string face = CardFace(card.value);
            string prefabName = $"Card_{face}_{card.suit}";
            cardPrefab = Resources.Load<GameObject>("Cards/Sprites/" + prefabName);
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

        UpdatePointsUI();
        yield return new WaitForSeconds(0.1f);
    }



    string CardFace(int value)
    {
        return value switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => value.ToString()
        };
    }

    public void AddDebugCredits()
    {
        playerData.AddCredits(1000);
        UpdateCreditsUI();
        Debug.Log("Added 1000 credits via debug button.");
    }

    IEnumerator EndGame(bool playerWon, bool tie = false)
    {
        gameOver = true;
        DisableMainButtons();

        yield return StartCoroutine(FlipAllDealerCards());

        if (tie)
        {
            resultText.text = "It's a tie! Bet returned.";
            playerData.AddCredits(currentBet);
        }
        else if (playerWon)
        {
            resultText.text = "You win! +" + (currentBet * 2);
            playerData.AddCredits(currentBet * 2);
            audioSource.PlayOneShot(winMoneySFX);
            audioSource.PlayOneShot(placeBetSFX);
        }
        else
        {
            resultText.text = "You lose.";
        }

        UpdatePointsUI();
        UpdateCreditsUI();
        yield return new WaitForSeconds(2f);

        restartButton.interactable = true;
        placeBetButton.interactable = true;
        betInputField.interactable = true;
        matchBetInputField.interactable = true;
    }

    void CheckMatchDealer()
    {
        if (matchBet <= 0) return;

        int playerFirst = playerHand[0].value;
        int playerSecond = playerHand[1].value;
        int dealerUp = dealerHand[0].value;

        if (playerFirst == dealerUp && playerSecond != dealerUp || playerSecond == dealerUp && playerFirst != dealerUp)
        {
            matchResultText.text = "Match Dealer! +" + (matchBet * 11);
            playerData.AddCredits(matchBet * 11);
            audioSource.PlayOneShot(winMoneySFX);
            audioSource.PlayOneShot(placeBetSFX);
        }
        else if (playerFirst == dealerUp && playerSecond == dealerUp)
        {
            matchResultText.text = "TRIPS!!! +" + (matchBet * 100);
            playerData.AddCredits(matchBet * 100);
            audioSource.PlayOneShot(winMoneySFX);
            audioSource.PlayOneShot(placeBetSFX);
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
        StartCoroutine(CheckHitOutcome());
    }

    IEnumerator CheckHitOutcome()
    {
        yield return new WaitForSeconds(0.4f);
        int total = GetHandValue(playerHand);

        if (total == 21)
        {
            HighlightPlayerCards(Color.green); // blackjack

            yield return StartCoroutine(EndGame(true));
        }
        else if (total > 21)
        {
            HighlightPlayerCards(Color.red);   // bust

            yield return StartCoroutine(EndGame(false));
        }
    }

    public void OnStand()
    {
        if (gameOver) return;
        StartCoroutine(DealerPlay());
    }

    IEnumerator DealerPlay()
    {
        // Reveal hidden card
        if (hasHiddenDealerCard)
        {
            dealerHand.Add(hiddenDealerCard);
            hasHiddenDealerCard = false;

            // Flip visual card
            if (dealerHandArea.childCount >= 2)
            {
                Transform secondCard = dealerHandArea.GetChild(1);
                CardVisual cv = secondCard.GetComponent<CardVisual>();
                if (cv != null)
                {
                    cv.FlipToFront(dealerHand[1]);
                    yield return new WaitForSeconds(0.4f);
                }
            }

            dealerHasHiddenCard = false;
            UpdatePointsUI();
            yield return new WaitForSeconds(0.1f);
        }

        int playerTotal = GetHandValue(playerHand);
        int dealerTotal = GetHandValue(dealerHand);

        // Dealer has blackjack
        if (dealerHand.Count == 2 && dealerTotal == 21)
        {
            HighlightDealerCards(Color.green);
            yield return new WaitForSeconds(0.4f);
            yield return StartCoroutine(EndGame(false));
            yield break;
        }

        // Dealer already beats player without drawing
        if (dealerTotal > playerTotal && dealerTotal <= 21)
        {
            yield return StartCoroutine(EndGame(false));
            yield break;
        }
        yield return new WaitForSeconds(2f);
        // Dealer hits on soft 17 or if under 17
        while (GetHandValue(dealerHand) < 17 || IsSoft17(dealerHand))
        {
            yield return DealCard(dealerHand, dealerHandArea);
            UpdatePointsUI();
            yield return new WaitForSeconds(0.2f);
        }

        dealerTotal = GetHandValue(dealerHand);

        if (dealerTotal > 21)
        {
            HighlightDealerCards(Color.red); // dealer bust
            yield return new WaitForSeconds(0.4f);
            yield return StartCoroutine(EndGame(true));
        }
        else if (dealerTotal == playerTotal)
        {
            yield return StartCoroutine(EndGame(false, true));
        }
        else if (playerTotal > dealerTotal)
        {
            yield return StartCoroutine(EndGame(true));
        }
        else
        {
            yield return StartCoroutine(EndGame(false));
        }
    }


    bool IsSoft17(List<Card> hand)
    {
        int total = 0;
        bool hasAce = false;

        foreach (Card card in hand)
        {
            int value = Mathf.Min(card.value, 10);
            total += value;
            if (card.value == 1)
            {
                hasAce = true;
            }
        }

        // If there's an Ace and counting it as 11 makes total == 17
        return hasAce && (total + 10 == 17);
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
        UpdatePointsUI();
        UpdateCreditsUI();
        restartButton.interactable = false;
    }

    public void OnExit()
    {
        SceneHelper.SaveAndLoadScene("MainScene");
        //LoadingScreenManager.Instance.LoadSceneWithLoadingScreen("MainScene");
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
            Destroy(child.gameObject, 0.1f);
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

    void HighlightPlayerCards(Color color)
    {
        for (int i = 0; i < playerHandArea.childCount; i++)
        {
            Transform card = playerHandArea.GetChild(i);
            Image img = card.GetComponent<Image>();
            if (img != null)
            {
                img.color = color;
            }
        }
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

    void UpdatePointsUI()
    {
        if (playerPointsText != null)
            playerPointsText.text = "Player: " + GetHandValue(playerHand);

        if (dealerPointsText != null)
        {
            if (gameOver || !dealerHasHiddenCard)
            {
                dealerPointsText.text = "Dealer: " + GetHandValue(dealerHand);
            }
            else if (dealerHand.Count > 0)
            {
                int visible = Mathf.Min(dealerHand[0].value, 10);
                if (dealerHand[0].value == 1) visible = 11;
                dealerPointsText.text = "Dealer: " + visible + " + ?";
            }
            else
            {
                dealerPointsText.text = "Dealer: ?";
            }
        }
    }

    void UpdateCreditsUI()
    {
        if (playerCreditsText != null && playerData != null)
        {
            playerCreditsText.text = "" + playerData.credits;
        }
    }

    IEnumerator FlipAllDealerCards()
    {
        audioSource.PlayOneShot(cardFlipSFX);
        dealerHasHiddenCard = false;

        if (hasHiddenDealerCard)
        {
            dealerHand.Add(hiddenDealerCard);
            hasHiddenDealerCard = false;
        }

        for (int i = 0; i < dealerHandArea.childCount && i < dealerHand.Count; i++)
        {
            Transform cardObj = dealerHandArea.GetChild(i);
            CardVisual cv = cardObj.GetComponent<CardVisual>();

            if (cv != null)
            {
                cv.FlipToFront(dealerHand[i]);
                yield return new WaitForSeconds(0.3f);
            }
        }

        UpdatePointsUI();
    }
    void HighlightDealerCards(Color color)
    {
        for (int i = 0; i < dealerHandArea.childCount; i++)
        {
            Transform card = dealerHandArea.GetChild(i);
            Image img = card.GetComponent<Image>();
            if (img != null)
            {
                img.color = color;
            }
        }
    }
    IEnumerator DelayedUIRefresh()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateCreditsUI();
    }


}
