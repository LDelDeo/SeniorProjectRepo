using UnityEngine;

public class WheelVisualSpin : MonoBehaviour
{ 
    public float spinSpeed = -360f;  // Degrees per second

    void Update()
    {

        transform.Rotate(Vector3.forward * -spinSpeed * Time.deltaTime);

    }
}
