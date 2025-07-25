using UnityEngine;

public class NPCCarSpawner : MonoBehaviour
{
    [Header("Car Prefabs")]
    public GameObject[] carPrefabs;

    [Header("Spawn Amounts (match order of prefabs)")]
    public int[] amountToSpawn;

    void Start()
    {
        SpawnCars();
    }

    void SpawnCars()
    {
        for (int i = 0; i < carPrefabs.Length; i++)
        {
            if (i >= amountToSpawn.Length || carPrefabs[i] == null)
                continue;

            for (int j = 0; j < amountToSpawn[i]; j++)
            {
                Instantiate(carPrefabs[i]);
            }
        }
    }

    public void SpawnOneMoreCar()
    {
        if (carPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, carPrefabs.Length);
        GameObject randomCar = carPrefabs[randomIndex];
        Instantiate(randomCar);
    }

}
