using UnityEngine;
using TMPro;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;
    public AudioSource ambianceSource;

    [Header("Music Tracks")]
    public AudioClip suburbs;
    public AudioClip rich;
    public AudioClip hood;
    public AudioClip industrial;

    [Header("Amabiance Tracks")]
    public AudioClip suburbsSFX;
    public AudioClip richSFX;
    public AudioClip hoodSFX;
    public AudioClip industrialSFX;

    [Header("City Enter Animation")]
    public Animator anim;
    public TMP_Text cityNameText;

    public void PlayMusic(AudioClip clip, string cityName)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        cityNameText.text = cityName;
        anim.SetTrigger("enteringCity");
    }
    
    public void PlayAmbiance(AudioClip clip)
    {
        ambianceSource.clip = clip;
        ambianceSource.Play();
    }
}
