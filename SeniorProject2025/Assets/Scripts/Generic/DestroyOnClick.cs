using UnityEngine;

public class DestroyOnClick : MonoBehaviour
{
    private FPShooting fPShooting;
    private FPController fPController;
    void Start()
    {
        fPShooting = FindObjectOfType<FPShooting>();
        fPController = FindObjectOfType<FPController>();
    }
    public void DestroyOnButtonClick()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fPShooting.enabled = true;
        fPController.enabled = true;

        Destroy(gameObject);
    }
}
