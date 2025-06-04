using UnityEngine;
using TMPro;
using System.Collections;

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

    [Header("Ambiance Tracks")]
    public AudioClip suburbsSFX;
    public AudioClip richSFX;
    public AudioClip hoodSFX;
    public AudioClip industrialSFX;

    [Header("City Enter Animation")]
    public Animator anim;
    public TMP_Text cityNameText;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    private Coroutine fadeCoroutine;

    public void PlayMusic(AudioClip clip, string cityName)
    {
        if (audioSource.clip != clip)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeMusic(clip));
        }

        cityNameText.text = cityName;
        anim.SetTrigger("enteringCity");
    }

    public void PlayAmbiance(AudioClip clip)
    {
        ambianceSource.clip = clip;
        ambianceSource.Play();
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        // Fade out current music
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in new music
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = startVolume;
    }
}
