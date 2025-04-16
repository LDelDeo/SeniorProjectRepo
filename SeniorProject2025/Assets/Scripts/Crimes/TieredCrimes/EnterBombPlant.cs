using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class EnterBombPlant : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject exclamationPoint;

    // Bomb Countdown
    public TMP_Text countdownText;
    private float countdownTime = 60f;
    private bool timerStarted = false;
    private bool timerEnded = false;

    [Header("Script Grabs")]
    private CrimeCompletion crimeCompletion;
    private WireCut wireCut;
    private PlayerHealth playerHealth;

    private void Start()
    {
        countdownText.text = "";
    }

    void Update()
    {
        if (crimeCompletion == null)
            crimeCompletion = FindObjectOfType<CrimeCompletion>();

        if (wireCut == null)
            wireCut = FindObjectOfType<WireCut>();

        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        if (timerStarted && !timerEnded)
        {
            countdownTime -= Time.deltaTime;
            countdownText.text = Mathf.CeilToInt(countdownTime).ToString();

            if (countdownTime <= 0f)
            {
                //wireCut.doneCorrectly = false;

                playerHealth.playerDied();
                Destroy(exclamationPoint);
                Destroy(gameObject);
                timerEnded = true;
            }

            if (wireCut != null)
            {
                if (wireCut.doneCorrectly)
                {
                    Debug.Log("Crime Stopped!");
                    crimeCompletion.CrimeStopped(crimeCompletion.tierThreeXP, crimeCompletion.tierThreeCredits);

                    Destroy(exclamationPoint);
                    Destroy(gameObject);
                    timerEnded = true;
                }
            }
        }
    }



        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                timerStarted = true;

                foreach (Transform child in transform)
                {
                    MeleeOrcEnemy melee = child.GetComponent<MeleeOrcEnemy>();
                    if (melee != null)
                    {
                        melee.BecomeHostile();
                    }

                    RangedOrcEnemy ranged = child.GetComponent<RangedOrcEnemy>();
                    if (ranged != null)
                    {
                        ranged.BecomeHostile();
                    }
                }
            }
        }
    }

    
