using UnityEngine;

public class GrassCulling : MonoBehaviour
{
    public float cullDistance = 50f;
    private Camera currentCamera;

    void Update()
    {
        UpdateCurrentCamera();
        if (currentCamera == null) return;

        float distance = Vector3.Distance(transform.position, currentCamera.transform.position);
        bool shouldBeVisible = distance < cullDistance;

        if (gameObject.activeSelf != shouldBeVisible)
            gameObject.SetActive(shouldBeVisible);
    }

    void UpdateCurrentCamera()
    {
        if (Camera.current != null)
            currentCamera = Camera.current;
    }
}
