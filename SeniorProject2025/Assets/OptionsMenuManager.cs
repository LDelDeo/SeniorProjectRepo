using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    public FPController fpController;
    public FPShooting fpsShooting;

    public AudioSource musicSource;
    public List<AudioSource> sfxAudioSources = new List<AudioSource>();

    public Slider musicSlider;
    public Slider audioSlider;
    public Slider sensitivitySlider;

    void Start()
    {
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        audioSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sensitivitySlider.value = sensitivity;

        
    }
    void Update()
    {
        if (fpController == null)
        {
            fpController = FindFirstObjectByType<FPController>();
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (musicSource != null)
            musicSource.volume = Mathf.Lerp(0.0001f, 1f, Mathf.Pow(value, 2f));
    }


    public void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);

        foreach (AudioSource source in sfxAudioSources)
        {
            if (source != null)
                source.volume = Mathf.Lerp(0.0001f, 1f, Mathf.Pow(value, 2f)); 
        }
    }

    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);

        if (fpController != null)
        {
            fpController.UpdateSensitivityFromPrefs();
        }
    }

}

