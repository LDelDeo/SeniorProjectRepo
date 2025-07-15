using UnityEngine;
using System.Collections.Generic;

public class CrimeSpawner : MonoBehaviour
{
    [Header("Script Grabs")]
    public PlayerData playerData;

    [Header("Spawn Locations")]
    // Tier 1 Crimes
    public GameObject[] illegalParkedCarLocations;
    public GameObject[] graffitiLocations;

    // Tier 2 Crimes
    public GameObject[] drugDealLocations;
    public GameObject[] streetFightLocations;
    public GameObject[] vandalismLocations;

    // Tier 3 Crimes
    public GameObject[] armedRobberyLocations;
    public GameObject[] bombPlantLocations;

    [Header("Crime Prefabs")]
    // Tier 1 Crimes
    public GameObject illegalParkedCarPrefab;
    public GameObject graffitiPrefab;

    // Tier 2 Crimes
    public GameObject drugDealPrefab;
    public GameObject streetFightPrefab;
    public GameObject vandalismPrefab;

    // Tier 3 Crimes
    public GameObject armedRobberyPrefab;
    public GameObject bombPlantPrefab;

    [Header("Spawning Intervals")]
    public float tierOneInterval = 45f;
    public float tierTwoInterval = 60f;
    public float tierThreeInterval = 75f;

    private float tierOneTimer;
    private float tierTwoTimer;
    private float tierThreeTimer;

    [SerializeField] private AudioSource crimeSpawnerAudioSource;
    [SerializeField] private AudioClip spawnCrimeSound;

    void Start()
    {
        tierOneTimer = tierOneInterval;
        tierTwoTimer = tierTwoInterval;
        tierThreeTimer = tierThreeInterval;

        // Start the game with a crime
        List<(GameObject prefab, GameObject[] locations, string tag, int requiredLevel)> initialCrimes = new List<(GameObject, GameObject[], string, int)>();

        if (!isCrimeActive("crimeOne") && illegalParkedCarLocations.Length > 0 && playerData.level >= 1)
            initialCrimes.Add((illegalParkedCarPrefab, illegalParkedCarLocations, "crimeOne", 1));

        if (!isCrimeActive("crimeTwo") && graffitiLocations.Length > 0 && playerData.level >= 2)
            initialCrimes.Add((graffitiPrefab, graffitiLocations, "crimeTwo", 2));

        if (initialCrimes.Count > 0)
        {
            var chosen = initialCrimes[Random.Range(0, initialCrimes.Count)];
            Instantiate(chosen.prefab, GetRandomPos(chosen.locations), Quaternion.identity);
        }
    }

    void Update()
    {
        tierOneTimer -= Time.deltaTime;
        tierTwoTimer -= Time.deltaTime;
        tierThreeTimer -= Time.deltaTime;

        if (tierOneTimer <= 0f)
        {
            spawnCrime(1);
            tierOneTimer = tierOneInterval;
        }

        if (tierTwoTimer <= 0f)
        {
            spawnCrime(2);
            tierTwoTimer = tierTwoInterval;
        }

        if (tierThreeTimer <= 0f)
        {
            spawnCrime(3);
            tierThreeTimer = tierThreeInterval;
        }
    }

    public bool isCrimeActive(string crimeTag)
    {
        return GameObject.FindWithTag(crimeTag) != null;
    }

    public void spawnCrime(int tier)
    {
        List<(GameObject prefab, GameObject[] locations, string tag, int requiredLevel)> availableCrimes = new List<(GameObject, GameObject[], string, int)>();

        switch (tier)
        {
            case 1:
                if (!isCrimeActive("crimeOne") && playerData.level >= 1 && illegalParkedCarLocations.Length > 0)
                    availableCrimes.Add((illegalParkedCarPrefab, illegalParkedCarLocations, "crimeOne", 1));
                if (!isCrimeActive("crimeTwo") && playerData.level >= 2 && graffitiLocations.Length > 0)
                    availableCrimes.Add((graffitiPrefab, graffitiLocations, "crimeTwo", 2));
                break;

            case 2:
                if (!isCrimeActive("crimeSeven") && playerData.level >= 3 && vandalismLocations.Length > 0)
                    availableCrimes.Add((vandalismPrefab, vandalismLocations, "crimeSeven", 3));
                if (!isCrimeActive("crimeThree") && playerData.level >= 4 && streetFightLocations.Length > 0)
                    availableCrimes.Add((streetFightPrefab, streetFightLocations, "crimeThree", 4));
                if (!isCrimeActive("crimeFour") && playerData.level >= 5 && drugDealLocations.Length > 0)
                    availableCrimes.Add((drugDealPrefab, drugDealLocations, "crimeFour", 5));
                break;

            case 3:
                if (!isCrimeActive("crimeFive") && playerData.level >= 6 && armedRobberyLocations.Length > 0)
                    availableCrimes.Add((armedRobberyPrefab, armedRobberyLocations, "crimeFive", 6));
                if (!isCrimeActive("crimeSix") && playerData.level >= 7 && bombPlantLocations.Length > 0)
                    availableCrimes.Add((bombPlantPrefab, bombPlantLocations, "crimeSix", 7));
                break;
        }

        if (availableCrimes.Count > 0)
        {
            var chosenCrime = availableCrimes[Random.Range(0, availableCrimes.Count)];
            Instantiate(chosenCrime.prefab, GetRandomPos(chosenCrime.locations), Quaternion.identity);
            crimeSpawnerAudioSource.PlayOneShot(spawnCrimeSound, 1.0f);
        }
    }

    private Vector3 GetRandomPos(GameObject[] locations)
    {
        return locations[Random.Range(0, locations.Length)].transform.position;
    }
}
