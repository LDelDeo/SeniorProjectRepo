using UnityEngine;

public class FPReload : MonoBehaviour
{
    public FPShooting fpShooting;
    public AudioSource gunAudio;
    public AudioClip reloadSoundTwo;
    public AudioClip reloadSoundThree;


    public void eventReload()
    {
        fpShooting.RefillAmmo();

    }

    public void ClipEjectSound()
    {
        gunAudio.clip = reloadSoundThree;
        gunAudio.Play();
    }

    public void GunCockSound()
    {
        gunAudio.clip = reloadSoundTwo;
        gunAudio.Play();
    }
}
