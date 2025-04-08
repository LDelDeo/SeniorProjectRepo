using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] private GameObject crosshairUI;

    private void OnEnable()
    {
        GameManager.OnStartAiming += ShowCrosshair;
        GameManager.OnStopAiming += HideCrosshair;
    }

    private void OnDisable()
    {
        GameManager.OnStartAiming -= ShowCrosshair;
        GameManager.OnStopAiming -= HideCrosshair;
    }

    private void Start()
    {
        if (crosshairUI != null)
            crosshairUI.SetActive(false);
    }

    private void ShowCrosshair()
    {
        if (crosshairUI != null)
            crosshairUI.SetActive(true);
    }

    private void HideCrosshair()
    {
        if (crosshairUI != null)
            crosshairUI.SetActive(false);
    }
}
