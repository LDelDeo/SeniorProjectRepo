using UnityEngine;
using System.Collections;
using TMPro;

public class Roulette : MonoBehaviour
{
    [Header("References")]
    public Animator rouletteWheel;

    [Header("Values")]
    private int currentBalance = 1000; //Temporary, Use Player Prefs Currency Here
    private int betSize = 100; // Hardcoded for Testing, Allow a Slider or Input Field to Set This
    private int wheelColor; // 0 for Black, 1 for Red
    private bool betBlack;
    private bool canPlaceABet;

    [Header("Text")]
    public TMP_Text currentBalanceDisplay;
    public TMP_Text gameText;


    public void Start()
    {
        gameText.text = "Place Your Bets!";
        currentBalanceDisplay.text = "Balance: " + currentBalance;
        canPlaceABet = true;
    }

    public void BetBlack()
    {
        if (canPlaceABet)
        {
            wheelColor = Random.Range(0, 2);
            betBlack = true;

            if (currentBalance >= betSize)
            {
                gameText.text = "You Bet " + betSize + " On Black, Good Luck!";
                if (wheelColor == 0) // Will Land On Black
                {
                    rouletteWheel.SetTrigger("isBlackTrig");

                    currentBalance = currentBalance + betSize; // Payout

                    canPlaceABet = false;
                }
                else if (wheelColor == 1) // Will Land On Red
                {
                    rouletteWheel.SetTrigger("isRedTrig");

                    currentBalance = currentBalance - betSize; // Take Bet Away

                    canPlaceABet = false;
                }

                StartCoroutine(updateText());
            }
            else
            {
                gameText.text = "Insufficient Funds";
            }
        }
        else
        {
            gameText.text = "Currently Spinning, Wait Until Round Is Over!";
        }
        
    }

    public void BetRed()
    {
        if (canPlaceABet)
        {
            wheelColor = Random.Range(0, 2);
            betBlack = false;

            if (currentBalance >= betSize)
            {
                gameText.text = "You Bet " + betSize + " On Red, Good Luck!";
                if (wheelColor == 0) // Will Land On Black
                {
                    rouletteWheel.SetTrigger("isBlackTrig");

                    currentBalance = currentBalance - betSize; // Take Bet Away

                    canPlaceABet = false;
                }
                else if (wheelColor == 1) // Will Land On Red
                {
                    rouletteWheel.SetTrigger("isRedTrig");

                    currentBalance = currentBalance + betSize; // Payout

                    canPlaceABet = false;
                }

                StartCoroutine(updateText());
            }
            else
            {
                gameText.text = "Insufficient Funds";
            }
        }
        else
        {
            gameText.text = "Currently Spinning, Wait Until Round Is Over!";
        } 
    }

    public IEnumerator updateText()
    {
        yield return new WaitForSeconds(3.0f);
        currentBalanceDisplay.text = "Balance: " + currentBalance;

        if (betBlack == true) // Bet Black
        {
            if (wheelColor == 0) // Landed On Black
            {
                gameText.text = "Black! You Win!";
            }
            else if (wheelColor == 1) // Landed On Red
            {
                gameText.text = "Red, You Lose";
            }
        }
        else if (betBlack == false) // Bet Red
        {
            if (wheelColor == 0) // Landed On Black
            {
                gameText.text = "Black, You Lose";
            }
            else if (wheelColor == 1)  // Landed On Red
            {
                gameText.text = "Red! You Win";
            }
        }

        yield return new WaitForSeconds(3.0f);
        gameText.text = "Place Your Bets!";
        canPlaceABet = true;

    }
}
