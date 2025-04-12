using UnityEngine;

public class DestroyOnClick : MonoBehaviour
{
    public void DestroyOnButtonClick()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Destroy(gameObject);
    }
}
