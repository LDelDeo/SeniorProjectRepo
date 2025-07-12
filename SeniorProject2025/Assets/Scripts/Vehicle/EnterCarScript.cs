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
    public bool areLightsOn;
    public Transform playerInCarTransform;
    public Transform exitCarTransform;
    public Transform lookForwardTransform;
   public FPController playerMovement;
   public FPShooting fpShooting;
   public DebugConsole debugConsole;
    public AudioSource siren;

    void Start()
    {
        isInCar = PlayerPrefs.GetInt("IsInCar", 0) == 1;

        if (isInCar)
        {
            EnterCar();

            siren.Play();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            carCamera.gameObject.SetActive(false);
            carCanvas.gameObject.SetActive(false);
            if (carControllerScript != null)
            {
                carControllerScript.enabled = false;
            }
        }
    }

    void Update()
    {

        if (!isInCar)
        {
            carControllerScript.rb.linearVelocity = new Vector3(0, 0, 0);
        }
        else
        {
            player.transform.position = playerInCarTransform.position;
        }

        // If the player is in the trigger zone and presses the 'E' key
        if (playerInTriggerZone && Input.GetKeyDown(KeyCode.E) && !debugConsole.consoleOpen)
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
            carLights.SetActive(!carLights.activeSelf);

            areLightsOn = carLights.activeSelf;

            if (areLightsOn)
            {
                siren.Play();
            }
            else
            {
                siren.Stop();
            }
        }
    }


    private void EnterCar()
    {
        // Disable Player Movement Script and Character Controller
        playerMovement.enabled = false;
        // Set Player Pos to Under Map Car Transform
        player.transform.position = playerInCarTransform.position;

        player.GetComponent<Rigidbody>().useGravity = false;
        // Disable Player Camera
        playerCamera.gameObject.SetActive(false);
        // Disbale Player HUD
        playerCanvas.gameObject.SetActive(false);

        // Disable Player Shoot Script
        fpShooting.enabled = false;
        // Enable the car's camera and canvas
        carCamera.gameObject.SetActive(true); // Enable the car's camera
        carCanvas.gameObject.SetActive(true); // Enable the car's UI canvas

        // Enable the car's controller script when the player is in the car
        if (carControllerScript != null)
        {
            carControllerScript.enabled = true;
        }

        isInCar = true; // Player is now in the car
        enterText.SetActive(false); // Hide the 'Enter' text
        PlayerPrefs.SetInt("IsInCar", isInCar ? 1 : 0);

        if (areLightsOn)
        {
            siren.Play();
        }
        else
        {
            siren.Stop();
        }
    }

    private void ExitCar()
    {
        siren.Stop();
        carControllerScript.StopEngineSound();

        // Enable Player Camera
        playerCamera.gameObject.SetActive(true);

        // Enable Player HUD
        playerCanvas.gameObject.SetActive(true);
        
        // Enable Player Shoot Script
        fpShooting.enabled = true;
        player.transform.rotation = Quaternion.identity;

        // Disable the car's camera and canvas
        carCamera.gameObject.SetActive(false); // Disable the car's camera
        carCanvas.gameObject.SetActive(false); // Disable the car's UI canvas

        player.GetComponent<Rigidbody>().useGravity = true;

        // Disable the car's controller script when the player is out of the car
        if (carControllerScript != null)
        {
            carControllerScript.enabled = false;
        }

        isInCar = false; // Player is now out of the car
        enterText.SetActive(true); // Show the 'Enter' text again

        player.transform.position = exitCarTransform.position;
        // Make Player look Forward
        player.transform.LookAt(lookForwardTransform.position);
        // Enable Player Movement Script and Character Controller
        playerMovement.enabled = true;
        PlayerPrefs.SetInt("IsInCar", isInCar ? 1 : 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enterText.SetActive(true); // Show the 'Enter' text when player enters the trigger
            playerInTriggerZone = true; // Set flag to true when player is in the trigger zone
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enterText.SetActive(false); // Hide the 'Enter' text when player exits the trigger
            playerInTriggerZone = false; // Set flag to false when player exits the trigger zone
        }
    }

}
