using UnityEngine;

public class MeleeDamageEventHuman : MonoBehaviour
{
    private MeleeHumanEnemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<MeleeHumanEnemy>();
        if (enemy == null)
            Debug.LogError("MeleeHumanEnemy script not found in parent!");
    }

    // These are the functions you add as animation events
    public void EnableDamageWindow()
    {
        if (enemy != null)
            enemy.EnableDamageWindow();
    }

    public void TryDealDamage()
    {
        if (enemy != null)
            enemy.TryDealDamage();
    }
}
