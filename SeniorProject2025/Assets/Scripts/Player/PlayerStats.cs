using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Damage")]
    public float playerRangedDamage = 1f;
    public float playerMeleeDamage = 1f;

    [Header("Range")]
    public float playerRangedRange = 10f;
    public float playerMeleeRange = 2f;

    [Header("Health")]
    public float maxHealth = 200f;
    public float health;

    [Header("Shield")]
    public bool isBlocking = false;
    public bool canBlock = true;
    public float shieldUpTime = 3f;
    public float shieldDownTime = 3f;
    public bool isShieldCooldown = false;
    public int blockAmt = 0;
    public int maxBlockAmt = 3;

    [Header("Respawn")]
    public bool isRespawning = false;

    [Header("Upgrades")]
    public int UpgradesAvailable = 0;

}
