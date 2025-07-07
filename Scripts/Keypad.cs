using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Keypad : MonoBehaviour
{
    public TMPro.TextMeshProUGUI displayText;
    public string correctCode = "";
    public string enteredCode = "";
    public GameObject keypadUI;
    public MonoBehaviour player;
    public Animator doorAnimator;  
    public bool isOpen = false;
    [SerializeField] public bool accessGranted = false;
    public string uniqueID;

    public void Update()
    {
        if (!isOpen) return;  

        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i)))
            {
                AddDigit(i.ToString());
            }
            if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Keypad" + i)))
            {
                AddDigit(i.ToString());
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CheckCode();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && enteredCode.Length > 0)
        {
            enteredCode = enteredCode.Substring(0, enteredCode.Length - 1);
            UpdateDisplay();
        }
    }

    public void AddDigit(string digit)
    {
        if (enteredCode.Length < correctCode.Length)
        {
            enteredCode += digit;
            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {
        displayText.text = enteredCode;
    }

    public virtual void CheckCode()
    {
        if (enteredCode == correctCode)
        {
            accessGranted = true;
            Debug.Log("Access Granted!");
            doorAnimator.SetTrigger("OpenDoor"); 
            ToggleKeypad(); 
        }
        else
        {
            Debug.Log("Incorrect Code!");
            enteredCode = "";
            UpdateDisplay();
        }
    }

    public void ToggleKeypad()
    {
        isOpen = !isOpen;
        keypadUI.SetActive(isOpen);
        player.enabled = !isOpen;
        UIBlocker.IsUILocked = isOpen;

        if (isOpen)
        {
            enteredCode = "";
            UpdateDisplay(); 

            Debug.Log("Current Correct Code: " + correctCode);
        }
    }
}
