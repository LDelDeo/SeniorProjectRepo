using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public Transform player; 

    void Update()
    {
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
