using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Damage")]
    public float playerRangedDamage = 25f;
    public float playerMeleeDamage = 50f;

    [Header("Health")]
    public float maxHealth = 200f;
    public float health;

}
