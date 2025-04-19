using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCarScript : MonoBehaviour
{
    public GameObject enterText;
    public GameObject player;
    public GameObject car;  // Reference to the car in the scene
    public Camera carCamera;
    public Camera playerCamera;
    public Canvas carCanvas;
    public Canvas playerCanvas;
    public CarController carControllerScript;  // Reference to the car controller script
    private bool playerInTriggerZone = false;
    public bool isInCar = false;
    public GameObject carLights;
    private bool areLightsOn;

    void Start()
    {
        // Initially, the car's camera and canvas are off, and the player is visible.
        carCamera.gameObject.SetActive(false);
        carCanvas.gameObject.SetActive(false);
        if (carControllerScript != null)
        {
            carControllerScript.enabled = false;  // Disable car controller by default when the player is not in the car
        }
    }

    void Update()
    {
        if (!isInCar) 
        {
            carControllerScript.rb.linearVelocity = new Vector3(0, 0, 0);
        }
        // If the player is in the trigger zone and presses the 'E' key
        if (playerInTriggerZone && Input.GetKeyDown(KeyCode.E))
        {
            if (!isInCar)
            {
                EnterCar(); // Enter the car
            }
            else
            {
                ExitCar(); // Exit the car
            }
        }

        if (carLights.activeSelf)
        {
            areLightsOn = true;
        }
        else
        {
            areLightsOn = false;
        }
        

        CarLights();
    }

    private void CarLights()
    {
        if (isInCar && Input.GetKeyDown(KeyCode.P))
        {
            if (!areLightsOn)
            {
                carLights.SetActive(true);
            }
            else
            {
                carLights.SetActive(false);
            }
        }
    }

    private void EnterCar()
    {
        Debug.Log("Player entered the car.");

        // Disable the entire player (including the player model, camera, and canvas)
        player.SetActive(false);  // Disables the entire player GameObject

        // Enable the car's camera and canvas
        carCamera.gameObject.SetActive(true); // Enable the car's camera
        carCanvas.gameObject.SetActive(true); // Enable the car's UI canvas

        // Enable the car's controller script when the player is in the car
        if (carControllerScript != null)
        {
            carControllerScript.enabled = true;
        }

        // Make the player a child of the car
        player.transform.SetParent(car.transform);

        isInCar = true; // Player is now in the car
        enterText.SetActive(false); // Hide the 'Enter' text
    }

    private void ExitCar()
    {
        Debug.Log("Player exited the car.");

        // Disable the entire player (including the player model, camera, and canvas)
        player.SetActive(true);  // Enables the entire player GameObject
        player.transform.rotation = Quaternion.identity;

        // Disable the car's camera and canvas
        carCamera.gameObject.SetActive(false); // Disable the car's camera
        carCanvas.gameObject.SetActive(false); // Disable the car's UI canvas

        // Disable the car's controller script when the player is out of the car
        if (carControllerScript != null)
        {
            carControllerScript.enabled = false;
        }

        // Detach the player from the car and place them slightly to the right
        player.transform.SetParent(null);
        player.transform.position = car.transform.position + new Vector3(2f, 5f, 0); // Adjust the 2f value to position them better

        isInCar = false; // Player is now out of the car
        enterText.SetActive(true); // Show the 'Enter' text again

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone.");
            enterText.SetActive(true); // Show the 'Enter' text when player enters the trigger
            playerInTriggerZone = true; // Set flag to true when player is in the trigger zone
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger zone.");
            enterText.SetActive(false); // Hide the 'Enter' text when player exits the trigger
            playerInTriggerZone = false; // Set flag to false when player exits the trigger zone
        }
    }
}
