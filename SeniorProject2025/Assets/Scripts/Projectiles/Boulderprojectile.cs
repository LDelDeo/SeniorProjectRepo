using UnityEngine;

public class BoulderProjectile : MonoBehaviour
{
    private RangedOrcEnemy owner;

    public void SetOwner(RangedOrcEnemy orc)
    {
        owner = orc;
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
