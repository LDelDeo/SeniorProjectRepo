using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CrimeCompletion : MonoBehaviour
{
    [Header("Grabs")]
    public PlayerData playerData;
    public FPShooting fPShooting;
    public FPController fPController;

    [Header("References")]
    public GameObject completionScreenPrefab;
    public Transform canvasTransform;
    public GameObject playerHUD;

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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        fPShooting.enabled = false;
        fPController.enabled = false;

        playerHUD.SetActive(false);

        GameObject screen = Instantiate(completionScreenPrefab, canvasTransform);

        TMP_Text xpGained = screen.transform.Find("XPText").GetComponent<TMP_Text>();
        TMP_Text creditsGained = screen.transform.Find("CreditsText").GetComponent<TMP_Text>();

        xpGained.text = "You Gained " + XP + " XP";
        creditsGained.text = "You Gained " + Credits + " Credits";

        int oldLevel = playerData.level;
        int oldXP = playerData.xp;

        playerData.AddCredits(Credits);
        playerData.AddXP(XP);

        int newLevel = playerData.level;
        int newXP = playerData.xp;

        // Update completion screen UI
        Slider xpBar = screen.transform.Find("CompletionXPBar").GetComponent<Slider>();
        TMP_Text completionLevelText = screen.transform.Find("CompletionLevelText").GetComponent<TMP_Text>();
        TMP_Text levelUpText = screen.transform.Find("LevelUpText").GetComponent<TMP_Text>();

        if (xpBar != null)
        {
            xpBar.maxValue = playerData.xpToNextLevel;
            xpBar.value = 0;
            StartCoroutine(AnimateXPBar(xpBar, oldXP, newXP, playerData.xpToNextLevel, oldLevel, newLevel));
        }

        if (completionLevelText != null)
            completionLevelText.text = "Level " + playerData.level;

        if (levelUpText != null)
        {
            if (playerData.level > oldLevel)
                StartCoroutine(ShowLevelUpMessage(levelUpText));
        }

    }

    private IEnumerator AnimateXPBar(Slider xpBar, int oldXP, int newXP, int finalMaxXP, int oldLevel, int newLevel)
    {
        float duration = 0.8f;
        int startXP = oldXP;
        int currentMax = finalMaxXP;
        int levelsToAnimate = newLevel - oldLevel;

        if (levelsToAnimate <= 0)
        {
            // Just animate XP gain within same level
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                xpBar.value = Mathf.Lerp(startXP, newXP, t / duration);
                yield return null;
            }
            xpBar.value = newXP;
        }
        else
        {
            // Animate level-up progression
            for (int i = 0; i < levelsToAnimate; i++)
            {
                float t = 0f;
                while (t < duration)
                {
                    t += Time.deltaTime;
                    xpBar.maxValue = currentMax;
                    xpBar.value = Mathf.Lerp(startXP, currentMax, t / duration);
                    yield return null;
                }

                xpBar.value = currentMax;
                yield return new WaitForSeconds(0.15f);

                startXP = 0;
                currentMax = Mathf.RoundToInt(currentMax * 2.5f); // You can change this scaling if needed
            }

            // Animate final partial XP
            xpBar.maxValue = finalMaxXP;
            float tFinal = 0f;
            while (tFinal < duration)
            {
                tFinal += Time.deltaTime;
                xpBar.value = Mathf.Lerp(0, newXP, tFinal / duration);
                yield return null;
            }

            xpBar.value = newXP;
        }
    }

    private IEnumerator ShowLevelUpMessage(TMP_Text levelUpText)
    {
        levelUpText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        levelUpText.gameObject.SetActive(false);
    }

}
