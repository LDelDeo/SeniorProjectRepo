using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCarScript : MonoBehaviour
{
    public GameObject enterText;  // UI Text for entering car
    public GameObject carPrefab;  // The car prefab to instantiate
    public GameObject player;     // The player GameObject
    private bool playerInTriggerZone = false;  // To track player presence in the trigger zone

    void Start()
    {
        // Optional: Make sure you can find the player object in case you don't manually assign it
        // player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // Check for 'E' key input when player is inside the trigger zone
        if (playerInTriggerZone && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed. Instantiating car...");

            // Instantiate the car in front of the player (adjust position if needed)
            Instantiate(carPrefab, player.transform.position + Vector3.forward * 2f, Quaternion.identity);

            // Destroy the player object to simulate entering the car
            Destroy(player);

            // Optionally destroy the trigger object (if it's not needed after use)
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone.");
            enterText.SetActive(true);  // Show the 'Enter' text when player enters the trigger
            playerInTriggerZone = true;  // Set flag to true when player is in the trigger zone
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger zone.");
            enterText.SetActive(false);  // Hide the 'Enter' text when player exits the trigger
            playerInTriggerZone = false;  // Set flag to false when player exits the trigger zone
        }
    }
}
