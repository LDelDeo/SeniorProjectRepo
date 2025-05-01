using System.Collections;
using UnityEngine;

public class BoulderProjectile : MonoBehaviour
{
    private RangedOrcEnemy owner;
    private bool hasDealtDamage = false;

    public void SetOwner(RangedOrcEnemy orc)
    {
        owner = orc;
    }

    public void Start()
    {
        StartCoroutine(destroyAfterTime());
    }

    public IEnumerator destroyAfterTime()
    {
        yield return new WaitForSeconds(2.5f);
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