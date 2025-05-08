using UnityEngine;

public class CarCameraEffects : MonoBehaviour
{
    [SerializeField] private Rigidbody carRigidbody;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float maxFOV = 90f;
    [SerializeField] private float speedForMaxFOV = 75f;

    private void Update()
    {
        float speed = carRigidbody.linearVelocity.magnitude;
        float t = Mathf.InverseLerp(0, speedForMaxFOV, speed);
        mainCamera.fieldOfView = Mathf.Lerp(baseFOV, maxFOV, t);
    }
}
