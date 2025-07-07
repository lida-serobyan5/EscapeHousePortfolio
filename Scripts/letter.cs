using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class letter : MonoBehaviour
{
    public GameObject letterUI;

    bool toggle;

    public FirstPersonController player;

    public Renderer letterMesh;
    public Renderer cubeMesh;

    public void openCloseLetter()
    {
        toggle = !toggle;

        if (toggle == false)
        {
            letterUI.SetActive(false);
            letterMesh.enabled = true;
            if (cubeMesh != null)
            {
                cubeMesh.enabled = true;
            }
            player.enabled = true;
        }

        if (toggle == true)
        {
            letterUI.SetActive(true);
            letterMesh.enabled = false;

            if (cubeMesh != null)
            {
                cubeMesh.enabled = false;
            }
            player.enabled = false;
        }
    }
}
