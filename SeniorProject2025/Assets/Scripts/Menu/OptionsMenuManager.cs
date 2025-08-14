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
    public Toggle fullscreenToggle;

    private GameObject[] npcCar;

    void Start()
    {
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        sensitivitySlider.SetValueWithoutNotify(sensitivity);
        musicSlider.SetValueWithoutNotify(musicVolume);
        audioSlider.SetValueWithoutNotify(sfxVolume);

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        audioSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        OnSensitivityChanged(sensitivity);
        OnMusicVolumeChanged(musicVolume);
        OnSFXVolumeChanged(sfxVolume);

        // Fullscreen toggle setup
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = isFullscreen;
        fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
    }

    void Update()
    {
        if (fpController == null)
        {
            fpController = FindFirstObjectByType<FPController>();
        }

        if (npcCar == null)
        {
            npcCar = GameObject.FindGameObjectsWithTag("NPCCar");

            foreach (GameObject car in npcCar)
            {
                AudioSource[] sources = car.GetComponents<AudioSource>();
                foreach (AudioSource src in sources)
                {
                    sfxAudioSources.Add(src);
                    src.volume = audioSlider.value;
                }
            }
        }

        if (musicSource != null)
        {
            float targetVol = Mathf.Lerp(0.0001f, 1f, Mathf.Pow(PlayerPrefs.GetFloat("MusicVolume", 1f), 2f));
            if (!Mathf.Approximately(musicSource.volume, targetVol))
            {
                musicSource.volume = targetVol;
            }
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

    public void OnFullscreenToggled(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1280, 720, false); // Width, Height, fullscreen false
        }

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

}
