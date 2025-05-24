using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class Roulette : MonoBehaviour
{
    [Header("References")]
    public Animator rouletteWheel;
    public PlayerData playerData;

    [Header("UI")]
    public TMP_Text gameText;
    public TMP_Text currentBalanceDisplay;
    public TMP_InputField betInputField; 

    private int betSize;
    private int wheelColor; // 0 = Black, 1 = Red
    private bool betBlack;
    private bool canPlaceABet = true;

    [Header("Sound Effects")]
    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip placeBetSFX;
    public AudioClip winMoneySFX;

    void Start()
    {
        gameText.text = "Place Your Bets!";
        UpdateBalanceDisplay();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Exit()
    {
        SceneHelper.SaveAndLoadScene("MainScene");
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

        audioSource.PlayOneShot(placeBetSFX);

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

        StartCoroutine(UpdateResultText());
    }

    private void UpdateBalanceDisplay()
    {
        currentBalanceDisplay.text = "Credits: " + playerData.credits;
    }

    private IEnumerator UpdateResultText()
    {
        yield return new WaitForSeconds(3f); // Wait for the wheel spin animation

        if (betBlack)
        {
            if (wheelColor == 0)
            {
                gameText.text = "Black! You Win!";
                audioSource.PlayOneShot(winMoneySFX); 
                audioSource.PlayOneShot(placeBetSFX);
            }
            else
            {
                gameText.text = "Red, You Lose";
            }
        }
        else
        {
            if (wheelColor == 1)
            {
                gameText.text = "Red! You Win!";
                audioSource.PlayOneShot(winMoneySFX);
                audioSource.PlayOneShot(placeBetSFX);
            }
            else
            {
                gameText.text = "Black, You Lose";
            }
        }

        UpdateBalanceDisplay();

        yield return new WaitForSeconds(3f);
        gameText.text = "Place Your Bets!";
        canPlaceABet = true;
    }

}
