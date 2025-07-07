using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public static class ContinueData
{
    public static PlayerProgressData loadedProgress;
    public static PlayerSettingsData loadedSettings;
}

public class MainMenu : MonoBehaviour
{
    private ProgressManager progressManager;

    void Start()
    {
        PersistentDataManager.instance?.SyncToLoggedInUser();

        StartCoroutine(LoadSettingsAtStartup());
    }

    IEnumerator LoadSettingsAtStartup()
    {
        string email = LoggedInUser.Email;
        var progressManager = FindObjectOfType<ProgressManager>();
        PlayerSettingsData settings = null;

        if (progressManager != null)
        {
            yield return progressManager.LoadProgress(email, (progress, loadedSettings) =>
            {
                settings = loadedSettings;

                if (settings != null)
                {
                    ContinueData.loadedSettings = settings;
                    PersistentDataManager.instance.SaveSettings(settings);
                    UnityEngine.Debug.Log("Settings loaded on main menu start.");
                }
            });
        }

        if (settings != null)
        {
            ContinueData.loadedSettings = settings;
            PersistentDataManager.instance.SaveSettings(settings);
            UnityEngine.Debug.Log("Settings applied after loading.");

            var optionsMenu = FindObjectOfType<OptionsMenu>(true); 
            if (optionsMenu != null)
            {
                optionsMenu.ApplyLoadedSettings(settings);
                UnityEngine.Debug.Log("Applied settings to OptionsMenu in MainMenu.");
            }
        }
    }

    public void PlayGame()
    {
        StartCoroutine(StartNewGameWithoutOverwritingSettings());
    }

    IEnumerator StartNewGameWithoutOverwritingSettings()
    {
        ContinueData.loadedProgress = null;
        PersistentDataManager.instance.hasLoadedFromContinue = false;

        var settings = ContinueData.loadedSettings; 

        if (settings != null)
        {
            PersistentDataManager.instance.SaveSettings(settings);
            UnityEngine.Debug.Log("Preserving settings for new game.");

            var optionsMenu = FindObjectOfType<OptionsMenu>(true);
            if (optionsMenu != null)
            {
                optionsMenu.ApplyLoadedSettings(settings);
                UnityEngine.Debug.Log("Applied settings to OptionsMenu in Play path.");
            }
        }

        UnityEngine.Debug.Log("Starting fresh game with current settings.");
        SceneManager.LoadSceneAsync(1);
        yield return null;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadProgressThenLoadScene());
    }

    private IEnumerator LoadProgressThenLoadScene()
    {
        string email = LoggedInUser.Email;
        bool loadFinished = false;

        progressManager = FindObjectOfType<ProgressManager>();
        if (progressManager == null)
        {
            UnityEngine.Debug.LogError("ProgressManager not found in MainMenu scene!");
            yield break;
        }

        yield return progressManager.LoadProgress(email, (loadedData, loadedSettings) =>
        {
            if (loadedData != null)
            {
                ContinueData.loadedProgress = loadedData;
                ContinueData.loadedSettings = loadedSettings;
                PersistentDataManager.instance.hasLoadedFromContinue = true;
                UnityEngine.Debug.Log("Loaded saved progress and settings successfully.");
            }
            else
            {
                UnityEngine.Debug.LogWarning("No saved progress found, starting fresh.");
                PersistentDataManager.instance.hasLoadedFromContinue = false;
            }
            loadFinished = true;
        });

        while (!loadFinished)
        {
            yield return null;
        }

        UnityEngine.Debug.Log("Loading game scene now...");
        PersistentDataManager.instance.hasLoadedFromContinue = true;
        SceneManager.LoadScene(1);
    }
}
