using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private RangedHumanEnemy owner;

    public void SetOwner(RangedHumanEnemy human)
    {
        owner = human;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (owner != null)
            {
                owner.DealDamage();
            }

            Destroy(gameObject);
        }
       else
        {

        }
    }
}
