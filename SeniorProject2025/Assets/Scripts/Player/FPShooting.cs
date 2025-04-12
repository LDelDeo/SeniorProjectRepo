using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class FPShooting : MonoBehaviour
{
    [Header("Grabs")]
    public PlayerStats playerStats;
    public Camera cam;
    public Animator shieldAnim;
    public Animator gunAnim;  // Animator reference for the gun or player model

    [Header("Gameplay")]
    private bool isBlocking;

    [Header("Shooting & Reloading")]
    private float timeToReload = 2.5f;
    private bool hasAmmo;
    private int bullets;
    public TMP_Text bulletsText;
    public TMP_Text reloadText;
    public ParticleSystem muzzleFlash;

    [Header("Camera Shake Settings")]
    public float shakeMagnitude = 0.1f;
    public float shakeDuration = 0.2f;
    private Vector3 originalCamPosition;

    private void Start()
    {
        hasAmmo = true;
        bullets = 16;
        reloadText.text = "";
        originalCamPosition = cam.transform.localPosition; // Store the camera's original position
    }

    private void Update()
    {
        bulletsText.text = "" + bullets;

        // Left Click to Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        // Right Click to Shield
        if (Input.GetMouseButtonDown(1))
        {
            Block();
        }

        if (Input.GetMouseButtonUp(1))
        {
            Unblock();
        }

        // R to Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (bullets <= 0)
        {
            hasAmmo = false;
            reloadText.text = "R to Reload";
        }
        else
        {
            reloadText.text = "";
        }
    }

    private void Shoot()
    {
        if (!isBlocking && hasAmmo)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            bullets--;
            muzzleFlash.Play();

            // Trigger Camera Shake
            StartCoroutine(ShakeCamera());

            // Play the shooting animation
            gunAnim.SetTrigger("ShootTrigger"); // Trigger the shoot animation

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.tag == "MeleeOrcEnemy")
                    hit.collider.GetComponent<MeleeOrcEnemy>().TakeDamage(playerStats.playerRangedDamage);

                if (hit.collider.tag == "RangedOrcEnemy")
                    hit.collider.GetComponent<RangedOrcEnemy>().TakeDamage(playerStats.playerRangedDamage);

                //Add Bullets
            }
        }
    }

    private void Reload()
    {
        // Play Reload Animation here
        gunAnim.SetTrigger("ReloadTrigger"); // Trigger the reload animation
        StartCoroutine(Reloading());
    }

    private IEnumerator Reloading()
    {
        yield return new WaitForSeconds(timeToReload);
        hasAmmo = true;
        bullets = 16;
    }

    private void Block()
    {
        isBlocking = true;
        shieldAnim.SetBool("shieldUp", true);
    }

    private void Unblock()
    {
        isBlocking = false;
        shieldAnim.SetBool("shieldUp", false);
    }

    private IEnumerator ShakeCamera()
    {
        float elapsed = 0.0f;
        
        // While the shake duration is still active, shake the camera
        while (elapsed < shakeDuration)
        {
            float shakeX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float shakeY = Random.Range(-shakeMagnitude, shakeMagnitude);
            cam.transform.localPosition = originalCamPosition + new Vector3(shakeX, shakeY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // After the shake, reset the camera position back to its original position
        cam.transform.localPosition = originalCamPosition;
    }
}
