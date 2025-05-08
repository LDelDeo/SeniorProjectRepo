// using UnityEngine;
// using Cinemachine;

// public class CarCameraEffects : MonoBehaviour
// {
//     [SerializeField] private Rigidbody carRigidbody;
//     [SerializeField] private CinemachineVirtualCamera virtualCam;
//     [SerializeField] private float baseFOV = 60f;
//     [SerializeField] private float maxFOV = 80f;
//     [SerializeField] private float speedForMaxFOV = 30f;

//     private void Update()
//     {
//         float speed = carRigidbody.velocity.magnitude;
//         float t = Mathf.InverseLerp(0, speedForMaxFOV, speed);
//         virtualCam.m_Lens.FieldOfView = Mathf.Lerp(baseFOV, maxFOV, t);
//     }
// }
