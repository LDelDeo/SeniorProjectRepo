using UnityEngine;
using TMPro;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class LevelUpDisplay : MonoBehaviour
{
    [Header("Grabs")]
    PlayerData playerData;
    public CrimeCompletion crimeCompletion;

    [Header("UI References")]
    public GameObject levelUpScreenPrefab;
    public Transform canvasTransform;


    private void Update()
    {
        if (crimeCompletion == null)
        {
            playerData.IsCrimeStopped = false;
        }

    }
    public void ShowLevelUpScreen(int level)
    {
        if (levelUpScreenPrefab == null || canvasTransform == null)
        {
            Debug.LogWarning("LevelUpDisplay is missing references!");
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameObject screen = Instantiate(levelUpScreenPrefab, canvasTransform);

        TMP_Text levelText = screen.transform.Find("LevelText").GetComponent<TMP_Text>();
        levelText.text = "You Reached Level " + level + "!";

       
    }
    public IEnumerator DelayLevelUp(bool IsCrimeStoppedActive)
    {
        yield return new WaitUntil(() => IsCrimeStoppedActive == false);
        ShowLevelUpScreen(playerData.level);
    }
}