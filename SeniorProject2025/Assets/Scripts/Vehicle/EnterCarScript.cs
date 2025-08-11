using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using TMPro;

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
    public Transform exitCarTransformRight;
    public Transform exitCarTransformBack;
    public Transform exitCarTransformFront;
    public Transform finalExit;
    public bool isBlocked;
    public Vector3 boxSize = new Vector3(2, 1, 2); 
    public LayerMask obstructionMask;
    public Transform lookForwardTransform;
   public FPController playerMovement;
   public FPShooting fpShooting;
   public DebugConsole debugConsole;
    public AudioSource sirenAudioSource;
    public AudioClip siren;
    public Animator gunAnim;
    public Animator exitCarText;
    private float currentWhyRotation;

    void Start()
    {
        isInCar = PlayerPrefs.GetInt("IsInCar", 0) == 1;

        if (isInCar)
        {
            EnterCar();

            // Setup and play looping siren
            if (sirenAudioSource != null && siren != null)
            {
                sirenAudioSource.clip = siren;
                sirenAudioSource.loop = true;
                sirenAudioSource.Play();
            }

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
            player.transform.rotation = car.transform.rotation * Quaternion.Euler(0, 180, 0);

            currentWhyRotation = car.transform.eulerAngles.y;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                exitCarText.SetTrigger("PauseAttempt");
            }
        }

        // If the player is in the trigger zone and presses the 'E' key
        if (playerInTriggerZone && Input.GetKeyDown(KeyCode.E) && !debugConsole.consoleOpen && !fpShooting.isInNoWeaponZone)
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
        if (isInCar && Input.GetKeyDown(KeyCode.Q))
        {
            carLights.SetActive(!carLights.activeSelf);
            areLightsOn = carLights.activeSelf;

            if (areLightsOn)
            {
                if (sirenAudioSource != null && siren != null)
                {
                    sirenAudioSource.clip = siren;
                    sirenAudioSource.loop = true;
                    sirenAudioSource.Play();
                }
            }
            else
            {
                if (sirenAudioSource != null && sirenAudioSource.isPlaying)
                {
                    sirenAudioSource.Stop();
                }
            }
        }
    }


    private void EnterCar()
    {
        playerMovement.enabled = false;
        player.transform.position = playerInCarTransform.position;
        player.GetComponent<Rigidbody>().useGravity = false;

        playerCamera.gameObject.SetActive(false);
        playerCanvas.gameObject.SetActive(false);
        fpShooting.enabled = false;

        carCamera.gameObject.SetActive(true);
        carCanvas.gameObject.SetActive(true);

        if (carControllerScript != null)
        {
            carControllerScript.enabled = true;
        }

        isInCar = true;
        enterText.SetActive(false);
        PlayerPrefs.SetInt("IsInCar", 1);

        if (areLightsOn)
        {
            if (sirenAudioSource != null && siren != null)
            {
                sirenAudioSource.clip = siren;
                sirenAudioSource.loop = true;
                sirenAudioSource.Play();
            }
        }
        else
        {
            if (sirenAudioSource != null && sirenAudioSource.isPlaying)
            {
                sirenAudioSource.Stop();
            }
        }
    }

    public void ExitCar()
    {
        if (sirenAudioSource != null && sirenAudioSource.isPlaying)
        {
            sirenAudioSource.Stop();
        }

        exitCarText.GetComponent<TMP_Text>().color = new Color32(230,0,0,0);

        //gunAnim.SetTrigger("enterCar");

        carControllerScript.OnExitCar();

        playerCamera.gameObject.SetActive(true);

        // Enable Player HUD
        playerCanvas.gameObject.SetActive(true);
        
        // Enable Player Shoot Script
        fpShooting.enabled = true;
        car.transform.rotation = Quaternion.Euler(0, currentWhyRotation,0);
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

        // Check if any colliders are overlapping with the trigger box
        // Decide where to spawn (priority order)
        Transform[] exits =
        {
    exitCarTransform,
    exitCarTransformRight,
    exitCarTransformBack,
    exitCarTransformFront
};

        Transform finalExit = null;
        foreach (var t in exits)
        {
            // use each exit's own rotation
            if (!CheckIfTransformIsBlocked(t.position, t.rotation))
            {
                finalExit = t;
                break;
            }
        }

        // Fallback if somehow all are blocked
        if (finalExit == null)
            finalExit = exitCarTransformBack ?? exitCarTransform;

        // Move player
        player.transform.position = finalExit.position;


        // Make Player look Forward
        player.transform.LookAt(lookForwardTransform.position);
        // Enable Player Movement Script and Character Controller
        playerMovement.enabled = true;
        PlayerPrefs.SetInt("IsInCar", isInCar ? 1 : 0);
    }

    public bool CheckIfTransformIsBlocked(Vector3 checkCenter, Quaternion checkRotation)
    {
        return Physics.CheckBox(checkCenter, boxSize * 0.5f, checkRotation, obstructionMask);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !fpShooting.isInNoWeaponZone)
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
