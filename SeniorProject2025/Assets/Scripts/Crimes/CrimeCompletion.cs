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
        TMP_Text totalCredits = screen.transform.Find("CompletionCreditsText").GetComponent<TMP_Text>();

        xpGained.text = "You Gained " + XP + " XP";
        creditsGained.text = "You Gained " + Credits + " Credits";
        

        int oldLevel = playerData.level;
        int oldXP = playerData.xp;

        playerData.AddCredits(Credits);
        playerData.AddXP(XP);

        totalCredits.text = "" + PlayerPrefs.GetInt("Credits", 0);

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
        float xpBarBecomesActiveAt = 5.5f;
        float waitBeforeStart = 8.0f;
        float duration = 3.75f;

        yield return new WaitForSeconds(xpBarBecomesActiveAt);

        while (!xpBar.gameObject.activeInHierarchy)
            yield return null;

        yield return null;

        xpBar.maxValue = playerData.GetXPRequiredForLevel(oldLevel);
        xpBar.value = oldXP;

        yield return new WaitForSeconds(waitBeforeStart - xpBarBecomesActiveAt);

        if (newLevel == oldLevel)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                xpBar.value = Mathf.Lerp(oldXP, newXP, t / duration);
                yield return null;
            }
            xpBar.value = newXP;
        }
        else
        {
            int currentXP = oldXP;

            for (int level = oldLevel; level < newLevel; level++)
            {
                int levelXPRequired = playerData.GetXPRequiredForLevel(level);
                xpBar.maxValue = levelXPRequired;
                xpBar.value = currentXP;

                float t = 0f;
                while (t < duration)
                {
                    t += Time.deltaTime;
                    xpBar.value = Mathf.Lerp(currentXP, levelXPRequired, t / duration);
                    yield return null;
                }

                xpBar.value = levelXPRequired;
                yield return new WaitForSeconds(0.2f);

                float resetDuration = 0.5f;
                float tReset = 0f;
                while (tReset < resetDuration)
                {
                    tReset += Time.deltaTime;
                    xpBar.value = Mathf.Lerp(levelXPRequired, 0, tReset / resetDuration);
                    yield return null;
                }

                xpBar.value = 0;
                currentXP = 0;
            }

            int finalLevelXPRequired = playerData.GetXPRequiredForLevel(newLevel);
            xpBar.maxValue = finalLevelXPRequired;
            xpBar.value = 0;

            float finalT = 0f;
            while (finalT < duration)
            {
                finalT += Time.deltaTime;
                xpBar.value = Mathf.Lerp(0, newXP, finalT / duration);
                yield return null;
            }

            xpBar.value = newXP;
        }
    }

    private IEnumerator ShowLevelUpMessage(TMP_Text levelUpText)
    {
        yield return new WaitForSeconds(11.75f);
        levelUpText.gameObject.SetActive(true);
        //yield return new WaitForSeconds(3f);
        //levelUpText.gameObject.SetActive(false);
    }

}
