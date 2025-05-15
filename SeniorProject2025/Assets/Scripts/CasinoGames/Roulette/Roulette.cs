using UnityEngine;
using System.Collections;
using TMPro;

public class Roulette : MonoBehaviour
{
    [Header("References")]
    public Animator rouletteWheel;
    public PlayerData playerData; // Reference to PlayerData script

    [Header("UI")]
    public TMP_Text gameText;
    public TMP_Text currentBalanceDisplay;
    public TMP_InputField betInputField; // Reference to input field for custom bets

    private int betSize;
    private int wheelColor; // 0 = Black, 1 = Red
    private bool betBlack;
    private bool canPlaceABet = true;

    void Start()
    {
        gameText.text = "Place Your Bets!";
        UpdateBalanceDisplay();
    }

    public void BetBlack()
    {
        TryPlaceBet(true);
    }

    public void BetRed()
    {
        TryPlaceBet(false);
    }

    private void TryPlaceBet(bool isBlack)
    {
        if (!canPlaceABet)
        {
            gameText.text = "Currently Spinning, Wait Until Round Is Over!";
            return;
        }

        // Parse the input bet amount
        if (!int.TryParse(betInputField.text, out betSize) || betSize <= 0)
        {
            gameText.text = "Invalid Bet Amount!";
            return;
        }

        if (playerData.credits < betSize)
        {
            gameText.text = "Insufficient Funds";
            return;
        }

        wheelColor = Random.Range(0, 2); // 0 = black, 1 = red
        betBlack = isBlack;
        canPlaceABet = false;

        gameText.text = $"You Bet {betSize} On {(isBlack ? "Black" : "Red")}, Good Luck!";

        // Trigger animation
        if (wheelColor == 0)
            rouletteWheel.SetTrigger("isBlackTrig");
        else
            rouletteWheel.SetTrigger("isRedTrig");

        // Win or lose
        if ((isBlack && wheelColor == 0) || (!isBlack && wheelColor == 1))
            playerData.AddCredits(betSize);
        else
            playerData.SpendCredits(betSize);

        UpdateBalanceDisplay();
        StartCoroutine(UpdateResultText());
    }

    private void UpdateBalanceDisplay()
    {
        currentBalanceDisplay.text = "Balance: " + playerData.credits;
    }

    private IEnumerator UpdateResultText()
    {
        yield return new WaitForSeconds(3f);

        if (betBlack)
            gameText.text = wheelColor == 0 ? "Black! You Win!" : "Red, You Lose";
        else
            gameText.text = wheelColor == 1 ? "Red! You Win!" : "Black, You Lose";

        yield return new WaitForSeconds(3f);
        gameText.text = "Place Your Bets!";
        canPlaceABet = true;
    }
}
