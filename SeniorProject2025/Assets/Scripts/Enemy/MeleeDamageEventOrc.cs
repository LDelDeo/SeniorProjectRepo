using UnityEngine;

public class MeleeDamageEventOrc : MonoBehaviour
{
    private MeleeOrcEnemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<MeleeOrcEnemy>();
        if (enemy == null)
            Debug.LogError("MeleeOrcEnemy script not found in parent!");
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
