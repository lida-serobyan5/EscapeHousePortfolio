using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;


public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    private float currentVolume;
    private int currentQuality;
    private bool currentFullscreen;
    [Header("With saved progress")]
    public Slider volumeSlider;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    [Header("Without saved progress")]
    public Slider volumeSlider1;
    public TMP_Dropdown qualityDropdown1;
    public Toggle fullscreenToggle1;

    private SettingsManager settingsManager; 

    void Awake()
    {
        currentVolume = 1f;
        currentQuality = QualitySettings.GetQualityLevel();
        currentFullscreen = Screen.fullScreen;
    }

    void OnEnable()
    {
        if (PersistentDataManager.instance != null)
        {
            PlayerSettingsData loadedSettings = PersistentDataManager.instance.LoadSettings();
            ApplyLoadedSettings(loadedSettings); 
        }
    }

    public void OnSettingsChanged()
    {
        var dataManager = FindObjectOfType<PersistentDataManager>();
        if (dataManager != null && !string.IsNullOrEmpty(dataManager.userEmail))
        {
            StartCoroutine(FindObjectOfType<ProgressManager>().SaveProgress(dataManager.userEmail, null));
        }
        else
        {
            UnityEngine.Debug.LogWarning("User email not found when trying to save settings.");
        }
    }


    public void ApplyAndSaveSettings()
    {
        if (gameObject.activeInHierarchy)
        {
            PlayerSettingsData settings = GetCurrentSettings();

            UnityEngine.Debug.Log("Saving Settings: " + settings.volume + ", " + settings.qualityLevel + ", " + settings.isFullScreen);

            if (PersistentDataManager.instance != null)
            {
                PersistentDataManager.instance.SaveSettings(settings); 
            }
            else
            {
                UnityEngine.Debug.LogWarning("PersistentDataManager not found in ApplyAndSaveSettings.");
            }

            if (settingsManager != null)
            {
                settingsManager.SaveSettings(settings);
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("BackgroundImage is inactive, cannot start coroutine.");
        }
    }


    public void SetVolume(float volume)
    {
        if (Mathf.Approximately(currentVolume, volume)) return; 
        audioMixer.SetFloat("volume", volume);
        currentVolume = volume;
        ApplyAndSaveSettings();
    }


    public void SetQuality(int qualityIndex)
    {
        if (currentQuality == qualityIndex) return;
        QualitySettings.SetQualityLevel(qualityIndex);
        currentQuality = qualityIndex;
        UnityEngine.Debug.Log("Quality set to: " + qualityIndex);
        ApplyAndSaveSettings();
    }

    public void SetFullScreen(bool isFullScreen = true)
    {
        if (currentFullscreen == isFullScreen) return;
        Screen.fullScreen = isFullScreen;
        currentFullscreen = isFullScreen;
        ApplyAndSaveSettings();
    }



    public PlayerSettingsData GetCurrentSettings()
    {
        return new PlayerSettingsData
        {
            volume = currentVolume,
            qualityLevel = currentQuality,
            isFullScreen = currentFullscreen,
        };
    }

    public void ApplyLoadedSettings(PlayerSettingsData settings)
    {
        audioMixer.SetFloat("volume", settings.volume);
        currentVolume = settings.volume;

        if (volumeSlider != null)
        {
            volumeSlider.value = settings.volume;
        }

        QualitySettings.SetQualityLevel(settings.qualityLevel);
        currentQuality = settings.qualityLevel;

        Screen.fullScreen = settings.isFullScreen;
        currentFullscreen = settings.isFullScreen;

        if (volumeSlider != null && volumeSlider1 != null)
            volumeSlider.value = settings.volume;
            volumeSlider1.value = settings.volume;

        if (qualityDropdown != null && qualityDropdown1 != null)
            qualityDropdown.value = settings.qualityLevel;
            qualityDropdown1.value = settings.qualityLevel;

        if (fullscreenToggle != null && fullscreenToggle1 != null)
            fullscreenToggle.isOn = settings.isFullScreen;
            fullscreenToggle1.isOn = settings.isFullScreen;

    }
}
