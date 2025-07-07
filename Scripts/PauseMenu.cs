using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject[] otherUIElements; 
    public FirstPersonController player;

    private bool isPaused = false;
    private bool[] previouslyActiveStates; 

    void Start()
    {
        previouslyActiveStates = new bool[otherUIElements.Length];
    }

    void Update()
    {
        if (UIBlocker.IsUILocked) return; 

        if (!isPaused && Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
    }


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
        isPaused = false;

        bool anyUIActive = false;

        for (int i = 0; i < otherUIElements.Length; i++)
        {
            if (otherUIElements[i] != null && previouslyActiveStates[i])
            {
                otherUIElements[i].SetActive(true);

                if (otherUIElements[i].activeSelf)
                {
                    anyUIActive = true;
                }
            }
        }

        player.enabled = !anyUIActive;
    }


    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        player.enabled = false;
        Time.timeScale = 0f;
        isPaused = true;

        for (int i = 0; i < otherUIElements.Length; i++)
        {
            if (otherUIElements[i] != null)
            {
                previouslyActiveStates[i] = otherUIElements[i].activeSelf;
                otherUIElements[i].SetActive(false);
            }
        }
    }
}
