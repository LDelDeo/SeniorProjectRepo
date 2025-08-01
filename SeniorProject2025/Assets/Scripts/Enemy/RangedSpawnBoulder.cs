using UnityEngine;

public class RangedSpawnBoulder : MonoBehaviour
{
    private RangedOrcEnemy orc;
    public GameObject BoulderGrab;
    public ParticleSystem groundSmash;

    private void Start()
    {
        orc = GetComponentInParent<RangedOrcEnemy>();
        BoulderGrab.SetActive(false);
    }

    private void SpawnBoulder()
    {
        orc.ShootProjectile();
        BoulderGrab.SetActive(false);
    }

    private void GrabbingBoulder()
    {
        BoulderGrab.SetActive(true);
    }

    private void CrackGround()
    {
        groundSmash.Play();
    }
}

