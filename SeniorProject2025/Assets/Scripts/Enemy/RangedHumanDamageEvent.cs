using UnityEngine;

public class RangedHumanDamageEvent : MonoBehaviour
{
    private RangedHumanEnemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<RangedHumanEnemy>();
    }

    public void ShootEvent()
    {
        enemy.ShootProjectile();
    }
}
