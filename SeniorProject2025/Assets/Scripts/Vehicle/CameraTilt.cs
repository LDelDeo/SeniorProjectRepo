using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // Reference to the main camera
    [SerializeField] private CarController carController; // Reference to the CarController
    [SerializeField] private float maxTiltAngle = 10f; // Maximum tilt angle for the camera
    [SerializeField] private float tiltSmoothing = 5f; // Smoothing for the tilt

    private float currentTilt = 0f;

    void LateUpdate()
    {
        // Get the target tilt based on the car's horizontal input (steering)
        float targetTilt = -carController.horizontalInput * maxTiltAngle;

        // Smoothly interpolate to the target tilt
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmoothing);

        // Apply the tilt to the camera's local rotation
        mainCamera.transform.localRotation = Quaternion.Euler(0, 0, currentTilt);
    }
}
