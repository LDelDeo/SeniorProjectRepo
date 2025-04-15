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
    public Animator meleeAnim;
    public GameObject gun;
    public GameObject melee;
    private EnterAssault enterAssault;
    private EnterDrugDeal enterDrugDeal;
    

    [Header("Shooting & Reloading")]
    private float timeToReload = 2.5f;
    private bool hasAmmo;
    private int bullets;
    public TMP_Text bulletsText;
    public TMP_Text reloadText;
    public ParticleSystem muzzleFlash;
    private bool isReloading = false;
    

    [Header("Camera Shake Settings")]
    public float shakeMagnitude = 0.1f;
    public float shakeDuration = 0.2f;
    private Vector3 originalCamPosition;

    [Header("Shield Settings")]
    private Coroutine shieldCoroutine;
    public TMP_Text shieldStatusText;
    public Image shieldCooldownBar;

    //Weapon Types
    private enum WeaponType { Gun, Melee }
    private WeaponType currentWeapon = WeaponType.Gun;


    private void Start()
    {
        hasAmmo = true;
        bullets = 16;
        reloadText.text = "";
        originalCamPosition = cam.transform.localPosition; // Store the camera's original position
        shieldStatusText.text = "Shield Ready";
        shieldCooldownBar.fillAmount = 0f;
    }

    private void Update()
    {
        if (currentWeapon == WeaponType.Gun)
        {
            bulletsText.text = "" + bullets;
        }
        else
        {
            bulletsText.text = "";
        }
        

        // Button to Switch Weapons
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(WeaponType.Gun);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(WeaponType.Melee);

        // Scroll Wheel to Switch Weapons
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentWeapon = currentWeapon == WeaponType.Gun ? WeaponType.Melee : WeaponType.Gun;
            SwitchWeapon(currentWeapon);
        }

        if (enterAssault == null || enterDrugDeal == null)
        {
            enterAssault = FindObjectOfType<EnterAssault>();
            enterDrugDeal = FindObjectOfType<EnterDrugDeal>();
        }

           

        // Left Click to Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            MeleeAttack();
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
        if (Input.GetKeyDown(KeyCode.R) && bullets != 16)
        {
            Reload();
        }

        if (bullets == 0)
        {
            hasAmmo = false;
        }

        if (bullets <= 3)
        {
            reloadText.text = "R to Reload";
        }
        else
        {
            reloadText.text = "";
        }
    }

    private void Shoot()
    {
        if (!playerStats.isBlocking && hasAmmo && currentWeapon == WeaponType.Gun && !isReloading)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            bullets--;
            muzzleFlash.Play();

            // Trigger Camera Shake
            StartCoroutine(ShakeCamera());

            // Play the shooting animation
            gunAnim.SetTrigger("ShootTrigger");

            if (Physics.Raycast(ray, out RaycastHit hit, playerStats.playerRangedRange))
            {
                //Orcs
                if (hit.collider.tag == "MeleeOrcEnemy")
                    hit.collider.GetComponent<MeleeOrcEnemy>().TakeDamage(playerStats.playerRangedDamage);

                if (hit.collider.tag == "RangedOrcEnemy")
                    hit.collider.GetComponent<RangedOrcEnemy>().TakeDamage(playerStats.playerRangedDamage);
                
                //Humans
                if(hit.collider.tag == "MeleeHumanEnemy")
                {
                    hit.collider.GetComponent<MeleeHumanEnemy>().TakeDamageFromGun();
                    enterAssault.crimeFoughtCorrectly = false; // You Are Not Supposed To Kill Lower Tier Threats
                }
                if(hit.collider.tag == "RangedHumanEnemy")
                {
                    hit.collider.GetComponent<RangedHumanEnemy>().TakeDamageFromGun();
                    enterDrugDeal.crimeFoughtCorrectly = false; // You Are Not Supposed To Kill Lower Tier Threats
                }
                
            }
        }
    }

    private void MeleeAttack()
    {
        if (!playerStats.isBlocking && currentWeapon == WeaponType.Melee)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            // Play melee animation
            meleeAnim.SetTrigger("Melee");

            if (Physics.Raycast(ray, out RaycastHit hit, playerStats.playerMeleeRange))
            {
                //Baton Cannot Deal Damage to Orcs
                //Baton CAN Deal Damage to Humans, Elves & Other Species

                if (hit.collider.CompareTag("MeleeHumanEnemy"))
                {
                    hit.collider.GetComponent<MeleeHumanEnemy>().TakeDamageFromBaton(playerStats.playerMeleeDamage);
                }
                if(hit.collider.tag == "RangedHumanEnemy")
                {
                    hit.collider.GetComponent<RangedHumanEnemy>().TakeDamageFromBaton(playerStats.playerMeleeDamage);

                }
                    

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
        isReloading = true;
        yield return new WaitForSeconds(timeToReload);
        hasAmmo = true;
        bullets = 16;
        isReloading = false;
    }

    private void Block()
    {
        if (playerStats.canBlock && !playerStats.isBlocking)
        {
            playerStats.isBlocking = true;
            playerStats.canBlock = false;
            shieldAnim.SetBool("shieldUp", true);
            shieldStatusText.text = "Shield Active";
            shieldCoroutine = StartCoroutine(ShieldRoutine());
        }
    }

    public void Unblock()
    {
        if (!playerStats.isBlocking)
            return;

        playerStats.isBlocking = false;
        shieldAnim.SetBool("shieldUp", false);
        shieldStatusText.text = "Recharging...";

        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            shieldCoroutine = null;
        }

        shieldCooldownBar.gameObject.SetActive(false); 

        StartCoroutine(ShieldCooldown());
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

    private void SwitchWeapon(WeaponType type)
    {
        currentWeapon = type;

        switch (type)
        {
            case WeaponType.Gun:
                gun.SetActive(true);
                melee.SetActive(false);
                break;

            case WeaponType.Melee:
                gun.SetActive(false);
                melee.SetActive(true);
                break;
        }
    }

    private IEnumerator ShieldRoutine()
    {
        float shieldTime = playerStats.shieldUpTime;
        float timer = shieldTime;

        shieldCooldownBar.fillAmount = 1f; // Full when starting
        shieldCooldownBar.gameObject.SetActive(true); // Show the image

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            shieldCooldownBar.fillAmount = timer / shieldTime;
            yield return null;
        }

        Unblock();
    }

    private IEnumerator ShieldCooldown()
    {
        playerStats.isShieldCooldown = true;
        float cooldownTime = playerStats.shieldDownTime;
        float timer = cooldownTime;

        while (timer > 0f)
        {
            shieldStatusText.text = "Cooldown: " + timer.ToString("F1") + "s";
            timer -= Time.deltaTime;
            yield return null;
        }

        playerStats.canBlock = true;
        playerStats.isShieldCooldown = false;
        shieldStatusText.text = "Shield Ready";
    }

}
