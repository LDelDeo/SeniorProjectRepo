using UnityEngine;

public class MapMarkerFollow : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        Vector3 targetPosition = new Vector3(player.position.x, 21, player.position.z);
        transform.position = targetPosition;
    }
}
