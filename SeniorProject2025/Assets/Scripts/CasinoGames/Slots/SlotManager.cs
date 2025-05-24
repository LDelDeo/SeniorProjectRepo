using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlotMachineManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text creditsText;
    public TMP_Text betText;
    public TMP_Text winText;

    public Button spinButton;
    public Button maxBetButton;
    public Button resetBetButton;
    public Button increaseBetButton;
    public Button decreaseBetButton;

    [Header("Reels")]
    public Image[] reelImages; // 4 Images for reels
    public Sprite[] symbolSprites; // Clear symbol sprites
    public Sprite[] blurredSprites; // Blurred versions (same order/length)
    public int[] payouts; // Multipliers for each symbol index

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip spinClip;
    public AudioClip winClip;
    public AudioClip jackpotClip;

    [Header("Player")]
    public PlayerData playerData;

    private int currentBet = 10;
    private Coroutine currentSpinRoutine;
    private Coroutine creditAnimationRoutine;
    private bool isSpinning = false;

    void Start()
    {
        spinButton.onClick.AddListener(Spin);
        maxBetButton.onClick.AddListener(() => SetMaxBet());
        resetBetButton.onClick.AddListener(() => SetResetBet());
        increaseBetButton.onClick.AddListener(() => AdjustBet(+10));
        decreaseBetButton.onClick.AddListener(() => AdjustBet(-10));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateUI();
    }

    public void ExitGame()
    {
        SceneHelper.SaveAndLoadScene("MainScene");
    }

    void AdjustBet(int delta)
    {
        currentBet = Mathf.Clamp(currentBet + delta, 10, playerData.credits);
        UpdateUI();
    }

    void SetMaxBet()
    {
        currentBet = playerData.credits;
        UpdateUI();
    } 
    void SetResetBet()
    {
        currentBet = 10;
        UpdateUI();
    }

    void Spin()
    {
        if (playerData.credits < currentBet) return;

        if (isSpinning && currentSpinRoutine != null)
        {
            StopCoroutine(currentSpinRoutine);
            audioSource.Stop();
        }

        playerData.SpendCredits(currentBet);
        winText.text = "WIN: 0";
        isSpinning = true;

        audioSource.Stop();
        audioSource.clip = spinClip;
        audioSource.loop = true;
        audioSource.Play();

        currentSpinRoutine = StartCoroutine(SpinAllReels());
        UpdateUI();
    }

    IEnumerator SpinAllReels()
    {
        int[] rollResult = new int[4];

        for (int i = 0; i < 4; i++)
        {
            yield return StartCoroutine(SpinReel(i, 1f + i * 0.2f, rollResult));
        }

        yield return new WaitForSeconds(0.2f);

        audioSource.Stop();
        audioSource.loop = false;

        bool allMatch = true;
        for (int i = 1; i < 4; i++)
        {
            if (rollResult[i] != rollResult[0])
            {
                allMatch = false;
                break;
            }
        }

        int winAmount = 0;
        if (allMatch)
        {
            int matchedIndex = rollResult[0];
            winAmount = currentBet * payouts[matchedIndex];

            if (creditAnimationRoutine != null)
                StopCoroutine(creditAnimationRoutine);

            creditAnimationRoutine = StartCoroutine(AnimateCreditGain(winAmount));

            winText.text = $"WIN: {winAmount}";

            audioSource.PlayOneShot(
                matchedIndex == payouts.Length - 1 ? jackpotClip : winClip
            );
        }
        else
        {
            winText.text = "WIN: 0";
        }

        UpdateUI();
        isSpinning = false;
        currentSpinRoutine = null;
    }

    IEnumerator SpinReel(int reelIndex, float spinDuration, int[] resultArray)
    {
        float time = 0f;
        float speed = 0.05f;

        while (time < spinDuration)
        {
            int randIndex = Random.Range(0, blurredSprites.Length);
            reelImages[reelIndex].sprite = blurredSprites[randIndex];
            yield return new WaitForSeconds(speed);
            time += speed;
        }

        int finalIndex = Random.Range(0, symbolSprites.Length);
        reelImages[reelIndex].sprite = symbolSprites[finalIndex];
        resultArray[reelIndex] = finalIndex;
    }

    IEnumerator AnimateCreditGain(int amount)
    {
        int startCredits = playerData.credits;
        int targetCredits = startCredits + amount;
        float duration = 0.75f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int current = Mathf.RoundToInt(Mathf.Lerp(startCredits, targetCredits, t));
            creditsText.text = $"CREDITS: {current}";
            yield return null;
        }

        playerData.credits = targetCredits;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (creditAnimationRoutine == null)
            creditsText.text = $"CREDITS: {playerData.credits}";

        betText.text = $"TOTAL BET: {currentBet}";
    }
}
