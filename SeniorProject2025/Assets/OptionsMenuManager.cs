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
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        sensitivitySlider.value = sensitivity;
        musicSlider.value = musicVolume;
        audioSlider.value = sfxVolume;

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        audioSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Apply initial volume based on loaded prefs
        OnMusicVolumeChanged(musicVolume);
        OnSFXVolumeChanged(sfxVolume);
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
        PlayerPrefs.SetFloat("MusicVolume", value);

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

