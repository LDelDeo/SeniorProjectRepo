using UnityEngine;

public class MeleeDealDamage : MonoBehaviour
{
    private MeleeOrcEnemy orc;

    private void Start()
    {
        orc = GetComponentInParent<MeleeOrcEnemy>();
    }

    private void AnimEnableDamageWindow()
    {
        orc.EnableDamageWindow();
    }
}
