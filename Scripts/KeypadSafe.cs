using UnityEngine;
using UnityEngine.UI;

public class KeypadSafe : Keypad
{
   
    public override void CheckCode()
    {
        if (enteredCode == correctCode)
        {
            accessGranted = true;
            Debug.Log("Safe Unlocked!");
            doorAnimator.SetTrigger("SafeOpen");  
            ToggleKeypad();
        }
        else
        {
            Debug.Log("Incorrect Safe Code!");
            enteredCode = "";
            UpdateDisplay();
        }
    }
}
