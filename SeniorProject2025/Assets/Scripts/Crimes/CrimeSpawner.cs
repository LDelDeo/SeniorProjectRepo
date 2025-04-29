using UnityEngine;

public class CrimeSpawner : MonoBehaviour
{
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

    void Start()
    {
        tierOneTimer = tierOneInterval;
        tierTwoTimer = tierTwoInterval;
        tierThreeTimer = tierThreeInterval;

        //Starts the Game With A Crime
        int randomNum = Random.Range(0, 2);

        if (randomNum == 0 && !isCrimeActive("crimeOne") && illegalParkedCarLocations.Length > 0)
        {
            Instantiate(illegalParkedCarPrefab, GetRandomPos(illegalParkedCarLocations), Quaternion.identity);
        }
        else if (randomNum == 1 && !isCrimeActive("crimeTwo") && graffitiLocations.Length > 0)
        {
            Instantiate(graffitiPrefab, GetRandomPos(graffitiLocations), Quaternion.identity);
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
        switch (tier)
        {
            case 1:
                int type1 = Random.Range(0, 2);
                if (type1 == 0 && !isCrimeActive("crimeOne") && illegalParkedCarLocations.Length > 0)
                {
                    Instantiate(illegalParkedCarPrefab, GetRandomPos(illegalParkedCarLocations), Quaternion.identity);
                }
                else if (type1 == 1 && !isCrimeActive("crimeTwo") && graffitiLocations.Length > 0)
                {
                    Instantiate(graffitiPrefab, GetRandomPos(graffitiLocations), Quaternion.identity);
                }
                break;

            case 2:
                int type2 = Random.Range(0, 3);
                if (type2 == 0 && !isCrimeActive("crimeFour") && drugDealLocations.Length > 0)
                {
                    Instantiate(drugDealPrefab, GetRandomPos(drugDealLocations), Quaternion.identity);
                }
                else if (type2 == 1 && !isCrimeActive("crimeThree") && streetFightLocations.Length > 0)
                {
                    Instantiate(streetFightPrefab, GetRandomPos(streetFightLocations), Quaternion.identity);
                }
                else if (type2 == 2 && !isCrimeActive("crimeSeven") && vandalismLocations.Length > 0)
                {
                    Instantiate(vandalismPrefab, GetRandomPos(vandalismLocations), Quaternion.identity);
                }
                break;

            case 3:
                int type3 = Random.Range(0, 2);
                if (type3 == 0 && !isCrimeActive("crimeFive") && armedRobberyLocations.Length > 0)
                {
                    Instantiate(armedRobberyPrefab, GetRandomPos(armedRobberyLocations), Quaternion.identity);
                }
                else if (type3 == 1 && !isCrimeActive("crimeSix") && bombPlantLocations.Length > 0)
                {
                    Instantiate(bombPlantPrefab, GetRandomPos(bombPlantLocations), Quaternion.identity);
                }
                break;
        }
    }

    private Vector3 GetRandomPos(GameObject[] locations)
    {
        return locations[Random.Range(0, locations.Length)].transform.position;
    }
}
