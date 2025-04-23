using UnityEngine;

public class BillboardingText : MonoBehaviour
{
    [Header("Target")]
    private Camera cameraToFace;

    void Start()
    {
        cameraToFace = Camera.main;
    }

    void Update()
    {
        if (cameraToFace == null) return;

        // Make the object face the camera
        transform.LookAt(cameraToFace.transform.position);

        // Lock rotation to Y axis only (no tilting up/down)
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y - 180, 0f);
    }
}
