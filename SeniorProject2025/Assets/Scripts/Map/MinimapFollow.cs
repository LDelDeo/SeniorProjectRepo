using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform target;

    private float fixedY; 

    void Start()
    {
        fixedY = transform.position.y; 
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(target.position.x, fixedY, target.position.z);
    }
}
