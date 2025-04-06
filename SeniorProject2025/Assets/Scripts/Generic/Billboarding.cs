using UnityEngine;

public class Billboarding : MonoBehaviour
{
    [Header("Target")]
    public Transform cameraToFace; 

    void Update()
    {
        transform.LookAt(cameraToFace.position);

        // Optional: Prevent rotation on the X and Z axes if you only want to rotate on the Y axis
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }
}
