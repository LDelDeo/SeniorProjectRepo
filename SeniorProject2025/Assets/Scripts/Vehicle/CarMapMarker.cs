using UnityEngine;

public class CarMapMarker : MonoBehaviour
{
    public Transform markerTransform;
    void LateUpdate()
    {
        if (markerTransform != null)
        {
            markerTransform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

}
