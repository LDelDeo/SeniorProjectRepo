using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Destroy this game object when it collides with anything
        Destroy(gameObject);
    }
}
