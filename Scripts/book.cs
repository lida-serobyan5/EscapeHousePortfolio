using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1;
    bool rotate = false;

    private void Start()
    {
        InitialState();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.D)) 
        {
            RotateForward();
        }

        if (Input.GetKeyDown(KeyCode.A)) 
        {
            RotateBack();
        }
    }

    public void InitialState()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].transform.rotation = Quaternion.identity;
        }
        pages[0].SetAsLastSibling();
    }

    public void RotateForward()
    {
        if (rotate == true) return;

        if (index + 1 >= pages.Count) return; 

        index++;
        float angle = 180;
        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle, true));
    }

    public void RotateBack()
    {
        if (rotate == true) return;

        if (index < 0) return; 

        float angle = 0;
        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle, false));
    }


    IEnumerator Rotate(float angle, bool forward)
    {
        float value = 0f;
        while (true)
        {
            rotate = true;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            value += Time.deltaTime * pageSpeed;
            pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value); 
            float angle1 = Quaternion.Angle(pages[index].rotation, targetRotation); 
            if (angle1 < 0.1f)
            {
                if (forward == false)
                {
                    index--;
                }
                rotate = false;
                break;
            }
            yield return null;
        }
    }
}
