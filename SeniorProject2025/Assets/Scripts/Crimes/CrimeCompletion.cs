using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CrimeCompletion : MonoBehaviour
{
    [Header("Grabs")]
    public PlayerData playerData;

    [Header("References")]
    public GameObject completionScreenPrefab;
    public Transform canvasTransform;

    [Header("XP & Credits Amounts")]

    public int tierOneXP = 100;
    public int tierTwoXP = 250;
    public int tierThreeXP = 600;

    public int tierOneCredits = 25;
    public int tierTwoCredits = 95;
    public int tierThreeCredits = 145;

    public int failedXP = 5;
    public int failedCredits = 0;

    public void CrimeStopped(int XP, int Credits)
    {
        playerData.IsCrimeStopped = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameObject screen = Instantiate(completionScreenPrefab, canvasTransform);

        TMP_Text xpGained = screen.transform.Find("XPText").GetComponent<TMP_Text>();
        TMP_Text creditsGained = screen.transform.Find("CreditsText").GetComponent<TMP_Text>();

        xpGained.text = "You Gained " + XP + " XP";
        creditsGained.text = "You Gained " + Credits + " Credits";

        playerData.AddCredits(Credits);
        playerData.AddXP(XP);

        

    }

   
}
