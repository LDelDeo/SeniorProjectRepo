using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject prefabToRespawn;  
    public Transform respawnLocation;   
    private GameObject currentInstance;
    private bool isRespawning = false;

    void Start()
    {
        SpawnObject();
    }

    void Update()
    {
        if (currentInstance == null && !isRespawning)
        {
            isRespawning = true;
            Invoke(nameof(SpawnObject), 2f); // Delay respawn (optional)
        }
    }

    void SpawnObject()
    {
        isRespawning = false;
        currentInstance = Instantiate(prefabToRespawn, respawnLocation.position, respawnLocation.rotation);
    }
}

