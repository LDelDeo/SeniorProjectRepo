using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class FPShooting : MonoBehaviour
{
    [Header("Grabs")]
    public PlayerStats playerStats;
    public FPController fpController;
    public Camera cam;
    public Animator shieldAnim;
    public Animator gunAnim;  // Animator reference for the gun or player model
    public Animator meleeAnim;
    public GameObject gun;
    public GameObject melee;
    private EnterAssault enterAssault;
    private EnterDrugDeal enterDrugDeal;
    private EnterVandalism enterVandalism;
    private EnterGraffiti enterGraffiti;
    public Animator weaponTypeAnim;
    

    [Header("Shooting & Reloading")]
    private float timeToReload = 2.5f;
    private bool hasAmmo;
    public int bullets;
    public TMP_Text bulletsText;
    public TMP_Text reloadText;
    public ParticleSystem muzzleFlash;
    private bool isReloading = false;
    public Image reticle;
    

    [Header("Camera Shake Settings")]
    public float shakeMagnitude = 0.1f;
    public float shakeDuration = 0.2f;
    private Vector3 originalCamPosition;

    [Header("Shield Settings")]
    private Coroutine shieldCoroutine;
    private Coroutine shieldCooldownRoutine;
    public TMP_Text shieldStatusText;
    public Image shieldCooldownBar;

    [Header("Hitmarker Settings")]
    public Image hitmarkerImage;
    public Image DeathMarkerImage;
    public float hitmarkerDuration = 0.1f;
    public float DeathmarkerDuration = 0.1f;
    private Coroutine hitmarkerRoutine;
    private Coroutine deathmarkerRoutine;

    //Weapon Types
    public enum WeaponType { Gun, Melee, None }
    public WeaponType currentWeapon = WeaponType.Gun;


    private void Start()
    {
        hasAmmo = true;
        bullets = 16;
        reloadText.text = "";
        hitmarkerImage.enabled = false;
        originalCamPosition = cam.transform.localPosition; // Store the camera's original position
        shieldStatusText.text = "Shield Ready";
        shieldCooldownBar.fillAmount = 0f;

        fpController = FindObjectOfType<FPController>();
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

        if (enterAssault == null || enterDrugDeal == null || enterVandalism == null || enterGraffiti == null)
        {
            enterAssault = FindObjectOfType<EnterAssault>();
            enterDrugDeal = FindObjectOfType<EnterDrugDeal>();
            enterVandalism = FindObjectOfType<EnterVandalism>();
            enterGraffiti = FindObjectOfType<EnterGraffiti>();
        }

        UpdateReticle();    

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
        if (Input.GetKeyDown(KeyCode.R) && bullets != 16 && !isReloading)
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

    private void UpdateReticle()
    {
        float range = currentWeapon == WeaponType.Gun 
            ? playerStats.playerRangedRange 
            : playerStats.playerMeleeRange;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            if (currentWeapon == WeaponType.Gun &&
                (hit.collider.CompareTag("GoblinGraffitiEnemy") ||
                hit.collider.CompareTag("MeleeOrcEnemy") ||
                hit.collider.CompareTag("RangedOrcEnemy") ||
                hit.collider.CompareTag("MeleeHumanEnemy") ||
                hit.collider.CompareTag("RangedHumanEnemy")))
            {
                reticle.color = Color.red;
                return;
            }

            if (currentWeapon == WeaponType.Melee &&
                (hit.collider.CompareTag("GoblinGraffitiEnemy") ||
                hit.collider.CompareTag("MeleeHumanEnemy") ||
                hit.collider.CompareTag("RangedHumanEnemy")))
            {
                reticle.color = Color.red;
                return;
            }
        }

        // Default color when not over a valid target
        reticle.color = Color.white;
    }

    private void Shoot()
    {

        if (bullets == 0 && !isReloading && currentWeapon == WeaponType.Gun && !playerStats.isBlocking && !fpController.isSprinting)
        {
            Reload();
        }

        if (!playerStats.isBlocking && hasAmmo && currentWeapon == WeaponType.Gun && !isReloading && !fpController.isSprinting)
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
                //Goblins
                if (hit.collider.CompareTag("GoblinGraffitiEnemy"))
                {
                    hit.collider.GetComponent<GoblinGraffitiEnemy>().TakeDamageFromGun();
                    enterGraffiti.crimeFoughtCorrectly = false; // You Are Not Supposed To Kill Lower Tier Threats
                }

                //Orcs
                if (hit.collider.tag == "MeleeOrcEnemy")
                {
                    hit.collider.GetComponent<MeleeOrcEnemy>().TakeDamage(playerStats.playerRangedDamage);
                }

                if (hit.collider.tag == "RangedOrcEnemy")
                {
                    hit.collider.GetComponent<RangedOrcEnemy>().TakeDamage(playerStats.playerRangedDamage);
                }
                
                //Humans
                if(hit.collider.tag == "MeleeHumanEnemy")
                {
                    hit.collider.GetComponent<MeleeHumanEnemy>().TakeDamageFromGun();
                    enterAssault.crimeFoughtCorrectly = false; // You Are Not Supposed To Kill Lower Tier Threats
                    enterVandalism.crimeFoughtCorrectly = false; // You Are Not Supposed To Kill Lower Tier Threats
                }
                if(hit.collider.tag == "RangedHumanEnemy")
                {
                    hit.collider.GetComponent<RangedHumanEnemy>().TakeDamageFromGun();
                    enterDrugDeal.crimeFoughtCorrectly = false; // You Are Not Supposed To Kill Lower Tier Threats
                }
                
            }
        }
       
    }
    public void RefillAmmo()
    {
        hasAmmo = true;
        bullets = 16;
        isReloading = false;
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

                //Goblins
                if (hit.collider.CompareTag("GoblinGraffitiEnemy"))
                {
                    hit.collider.GetComponent<GoblinGraffitiEnemy>().TakeDamageFromBaton(playerStats.playerMeleeDamage);
                    enterGraffiti.crimeFoughtCorrectly = false; // You Are Not Supposed To Use Weapons on Tier 1 Threats
                }

                //Humans
                if (hit.collider.CompareTag("MeleeHumanEnemy"))
                {
                    hit.collider.GetComponent<MeleeHumanEnemy>().TakeDamageFromBaton(playerStats.playerMeleeDamage);
                }
                if (hit.collider.tag == "RangedHumanEnemy")
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
        isReloading = true;
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

        shieldCooldownRoutine = StartCoroutine(ShieldCooldown());
    }

    public void Hitmarker()
    {
        if (hitmarkerRoutine != null)
            StopCoroutine(hitmarkerRoutine);

        hitmarkerRoutine = StartCoroutine(ShowHitMarker());

    }
    public void Deathmarker()
    {
        if (deathmarkerRoutine != null)
            StopCoroutine(deathmarkerRoutine);

        deathmarkerRoutine = StartCoroutine(ShowDeathMarker());

    }

    private void SwitchWeapon(WeaponType type)
    {
        currentWeapon = type;
        
        weaponTypeAnim.SetTrigger("WeaponSwitch");

        switch (type)
        {
            case WeaponType.Gun:
                // Active
                gunAnim.enabled = true;
                gun.SetActive(true);
                // Inactive
                meleeAnim.enabled = false;
                melee.SetActive(false);
                break;

            case WeaponType.Melee:
                // Active
                meleeAnim.enabled = true;
                melee.SetActive(true);
                // Inactive
                gunAnim.enabled = false;
                gun.SetActive(false);
                break;

            case WeaponType.None:
                // Melee
                meleeAnim.enabled = false;
                melee.SetActive(false);
                // Gun
                gun.SetActive(false);
                gunAnim.enabled = false;
                break;
                
                
        }
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

    public void CancelShieldCooldown()
    {
        if (shieldCooldownRoutine != null)
        {
            StopCoroutine(shieldCooldownRoutine);
            shieldCooldownRoutine = null;
        }

        playerStats.canBlock = true;
        playerStats.isShieldCooldown = false;
        shieldStatusText.text = "Shield Ready";
    }

    private IEnumerator ShowHitMarker()
    {
        hitmarkerImage.enabled = true;
        yield return new WaitForSeconds(hitmarkerDuration);
        hitmarkerImage.enabled = false;
        hitmarkerRoutine = null;
    } 
    
    private IEnumerator ShowDeathMarker()
    {
        DeathMarkerImage.enabled = true;
        yield return new WaitForSeconds(hitmarkerDuration);
        DeathMarkerImage.enabled = false;
        deathmarkerRoutine = null;
    }

}
