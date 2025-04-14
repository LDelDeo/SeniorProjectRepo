using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private RangedHumanEnemy owner;
    private bool hasDealtDamage = false; 

    public void SetOwner(RangedHumanEnemy human)
    {
        owner = human;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDealtDamage) return;

        if (other.CompareTag("Player"))
        {
            hasDealtDamage = true;

            if (owner != null)
            {
                owner.DealDamage();
            }

            Destroy(gameObject);
        }
    }
}

