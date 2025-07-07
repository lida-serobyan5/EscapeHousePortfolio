using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using UnityEngine.UI;
using System.Numerics;

public class LoginManager : MonoBehaviour
{
    [Header("Register")]
    [SerializeField] TMP_InputField Reg_Email;
    [SerializeField] TMP_InputField Reg_Username;
    [SerializeField] TMP_InputField Reg_Password;
    public GameObject menuUI;
    [SerializeField] private TMP_Text emailErrorText;
    [SerializeField] private TMP_Text usernameErrorText;
    [SerializeField] private TMP_Text passwordErrorText;
    [SerializeField] private TMP_Text emailPlaceholderText;
    [SerializeField] private TMP_Text usernamePlaceholderText;
    [SerializeField] private TMP_Text passwordPlaceholderText;



    [Header("Login")]
    [SerializeField] TMP_InputField Log_Email;
    [SerializeField] TMP_InputField Log_Password;
    public GameObject mainMenuUI;
    [SerializeField] private TMP_Text UsernameText;
    [SerializeField] private TMP_Text emailErrorText2;
    [SerializeField] private TMP_Text passwordErrorText2;
    [SerializeField] private TMP_Text emailPlaceholderText2;
    [SerializeField] private TMP_Text passwordPlaceholderText2;

    private bool isNewlyRegistered = false;

    public void OnEmailFieldSelected()
    {
        emailPlaceholderText.text = "Enter a valid email address";
        emailPlaceholderText2.text = "Enter a valid email address";
    }

    public void OnPasswordFieldSelected()
    {
        passwordPlaceholderText.text = "At least 8 characters";
        passwordPlaceholderText2.text = "At least 8 characters";
    }

    public void OnUsernameFieldSelected()
    {
        usernamePlaceholderText.text = "At least 5 letters";
    }

    public async void OnRegisterPressed()
    {
        if (string.IsNullOrWhiteSpace(Reg_Email.text) || !System.Text.RegularExpressions.Regex.IsMatch(Reg_Email.text, @"^[^@]+@[^@]+\.[^@]+$"))
        {
            UnityEngine.Debug.LogError("Please enter a valid email!");
            emailErrorText.text = "Please enter a valid email!";
            return;
        }

        if (string.IsNullOrWhiteSpace(Reg_Username.text) || Reg_Username.text.Length < 5 || !System.Text.RegularExpressions.Regex.IsMatch(Reg_Username.text, @"^[a-zA-Z]+$"))
        {
            UnityEngine.Debug.LogError("Please enter a valid username!");
            usernameErrorText.text = "Please enter a valid username!";
            return;
        }

        if (string.IsNullOrWhiteSpace(Reg_Password.text) || Reg_Password.text.Length < 8)
        {
            UnityEngine.Debug.LogError("Please enter a valid password!");
            passwordErrorText.text = "Please enter a valid password!";
            return;
        }

        if (await MySqlManager.RegisterUser(Reg_Email.text, Reg_Password.text, Reg_Username.text))
        {
            print("succesfully registered");
            menuUI.SetActive(true);
            LoggedInUser.Email = Reg_Email.text;

            if (PersistentDataManager.instance != null)
            {
                PersistentDataManager.instance.SetLoggedInUser(LoggedInUser.Email, LoggedInUser.Username);
             
            }
            isNewlyRegistered = true;
        }
        else { print("failed to register user"); }
    }

    public async void OnLoginPressed()
    {
        if (string.IsNullOrWhiteSpace(Log_Email.text) || !System.Text.RegularExpressions.Regex.IsMatch(Log_Email.text, @"^[^@]+@[^@]+\.[^@]+$"))
        {
            UnityEngine.Debug.LogError("Please enter a valid email!");
            emailErrorText2.text = "Please enter a valid email!";
            return;
        }

        if (string.IsNullOrWhiteSpace(Log_Password.text) || Log_Password.text.Length < 8)
        {
            UnityEngine.Debug.LogError("Please enter a valid password!");
            passwordErrorText2.text = "Please enter a valid password!";
            return;
        }

        (bool success, string username) = await MySqlManager.LoginUser(Log_Email.text, Log_Password.text);

        if (success)
        {
            LoggedInUser.Email = Log_Email.text;

            if (PersistentDataManager.instance != null)
            {
                PersistentDataManager.instance.SetLoggedInUser(LoggedInUser.Email, LoggedInUser.Username);
            }
            UnityEngine.Debug.Log("Successfully logged in User: " + username);
            UsernameText.text = "Welcome back " + username + "!";

            if (!isNewlyRegistered)
            {
                StartCoroutine(CheckProgressAfterLogin()); 
            }
            else
            {
                menuUI.SetActive(true);
                mainMenuUI.SetActive(false);
            }
        }

        else { print("failed to login user"); }
    }

    private IEnumerator CheckProgressAfterLogin()
    {
        ProgressManager progressManager = FindObjectOfType<ProgressManager>();
        if (progressManager == null)
        {
            UnityEngine.Debug.LogError("ProgressManager not found in LoginManager!");
            yield break;
        }

        string email = LoggedInUser.Email;
        bool loadFinished = false;

        yield return progressManager.LoadProgress(email, (data, settings) =>
        {
            var optionsMenu = FindObjectOfType<OptionsMenu>();

            bool hasProgress = data != null && data.posX > 0;
            /* && (
                            data.movedObjects.Count > 0 ||
                            data.openedSafes.Count > 0 ||
                            data.openedDoors.Count > 0 ||
                            data.gameFlags.Count > 0
                        );*/

            if (hasProgress)
            {
                ContinueData.loadedProgress = data;
                ContinueData.loadedSettings = settings;
                PersistentDataManager.instance.hasLoadedFromContinue = true;

                ProgressManager progressManager = FindObjectOfType<ProgressManager>();
                if (progressManager != null)
                {
                    StartCoroutine(progressManager.LoadProgress(LoggedInUser.Email, (progress, settings) =>
                    {
                        PersistentDataManager.instance.loadedProgress = progress;
                        PersistentDataManager.instance.playerSettings = settings;
                        UnityEngine.Debug.Log("Settings and progress loaded right after login.");
                    }));
                }


                if (optionsMenu != null && settings != null)
                    optionsMenu.ApplyLoadedSettings(settings);

                mainMenuUI.SetActive(true); 
                menuUI.SetActive(false);
            }
            else
            {
                if (settings != null)
                {
                    ContinueData.loadedSettings = settings;
                  
                    if (optionsMenu != null && settings != null)
                    {
                        optionsMenu.ApplyLoadedSettings(settings);
                        UnityEngine.Debug.Log("Applied settings to OptionsMenu after login.");
                    }
                }

                PersistentDataManager.instance.hasLoadedFromContinue = false;
                menuUI.SetActive(true);    
                mainMenuUI.SetActive(false);
            }

            loadFinished = true;
        });

        while (!loadFinished)
        {
            yield return null;
        }
    }
}
