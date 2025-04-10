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
    public Animator gunAnim;

    [Header("Gameplay")]
    private bool isBlocking;

    [Header("Shooting & Reloading")]
    private float timeToReload = 2.5f;
    private bool hasAmmo;
    private int bullets;
    public TMP_Text bulletsText;
    public TMP_Text reloadText;

    private void Start()
    {
        hasAmmo = true;
        bullets = 16;
        reloadText.text = "";
    }

    private void Update()
    {
        bulletsText.text = "" + bullets;

        // Left Click to Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            gunAnim.Play("Recoil");
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
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Hit " + hit.collider.name);
                

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
        //Play Animation Here
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
}
