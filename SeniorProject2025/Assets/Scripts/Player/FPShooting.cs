using UnityEngine;

public class FPShooting : MonoBehaviour
{
    [Header("Grabs")]
    public PlayerStats playerStats;
    public Camera cam;
    public Animator shieldAnim;
    public Animator gunAnim;

    [Header("Gameplay")]
    private bool isBlocking;

    private void Update()
    {
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
    }
    private void Shoot()
    {
        if (!isBlocking)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            gunAnim.Play("Recoil");
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
