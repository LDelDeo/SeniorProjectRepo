using UnityEngine;

public class EnemyHandCollider : MonoBehaviour
{
    private MeleeOrcEnemy orc;

    private void Start()
    {
        orc = GetComponentInParent<MeleeOrcEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            orc.TryDealDamage();
        }
    }
}
