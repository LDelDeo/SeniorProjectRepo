using System.Collections;
using UnityEngine;

public class WeaponAimAndShoot : MonoBehaviour
{
    [Header("Camera Animator")]
    [SerializeField] private Animator cinemachineAnimator;
    [SerializeField] private string aimCameraState = "AimCamera";
    [SerializeField] private string followCameraState = "FollowCamera";

    [Header("Player")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private float aimRotateSpeed = 10f;


    private Camera cam;
    private bool canRotateToAim;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleAimingInput();
        HandleShootingInput();
    }

    private void HandleAimingInput()
    {
        if (Input.GetMouseButtonDown(1))
            StartAiming();

        if (Input.GetMouseButtonUp(1))
            StopAiming();

        if (GameManager.Instance.IsAiming && canRotateToAim)
            AimTowardsLookPoint();
    }

    private void HandleShootingInput()
    {
        if (GameManager.Instance.IsAiming && canRotateToAim && Input.GetMouseButtonDown(0))
        {
            Shoot();
            GameManager.Instance.TriggerShoot();
        }
    }

    private void StartAiming()
    {
        cinemachineAnimator.Play(aimCameraState);
        GameManager.Instance.SetAiming(true);
        StartCoroutine(DelayAimRotation(0.3f));
    }

    private void StopAiming()
    {
        cinemachineAnimator.Play(followCameraState);
        GameManager.Instance.SetAiming(false);
        canRotateToAim = false;
    }

  

    private void AimTowardsLookPoint()
    {
        Vector3 lookDirection = cam.transform.forward;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            playerBody.rotation = Quaternion.Lerp(playerBody.rotation, targetRotation, Time.deltaTime * aimRotateSpeed);
        }
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Hit " + hit.collider.name);
            //Add Bullets
        }
    }

    private IEnumerator DelayAimRotation(float delay)
    {
        yield return new WaitForSeconds(delay);
        canRotateToAim = true;
    }
}
