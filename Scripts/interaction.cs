using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class interaction : MonoBehaviour
{
    public float interactionDistance;
    public GameObject interactionText;
    public GameObject interactionText2;
    public GameObject interactionText3;
    public GameObject keypad;
    public LayerMask interactionLayers;
    public AudioSource objectsSound;
    public AudioSource openSound;
    private PickUpObjects pickUpScript;
    public FirstPersonController playerController; 


    private void Start()
    {
        pickUpScript = FindObjectOfType<PickUpObjects>();
    }

    void Update()
    {
        interactionText.SetActive(false);
        interactionText2.SetActive(false);
        interactionText3.SetActive(false);
        GameObject heldObject = pickUpScript.heldObject;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactionLayers))
        {
            GameObject hitObject = hit.collider.gameObject;
            HistoryFrame frame = hitObject.GetComponent<HistoryFrame>();
            bool interactedWithFrame = false;

            if (frame != null)
            {
                if (heldObject != null && heldObject.CompareTag("HistoryCard"))
                {
                    if (!frame.hasCard)
                    {
                        interactionText2.SetActive(true);

                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            frame.PlaceCardInFrame(heldObject);
                            pickUpScript.heldObject = null;
                        }
                    }
                    else
                    {
                        interactionText2.SetActive(false); 
                    }
                }
            }

            if (interactedWithFrame)
            {
                pickUpScript.heldObject = null;  
            }

            if (heldObject != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    pickUpScript.DropHeldObject();
                }
                return; 
            }

            if (hitObject.GetComponent<letter>() && hitObject.CompareTag("Pickupable") || hitObject.CompareTag("HistoryCard"))
            {
                interactionText3.SetActive(true);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    hitObject.GetComponent<letter>().openCloseLetter();
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    objectsSound.Play();
                    pickUpScript.TryPickUp(hitObject);
                    interactionText3.SetActive(false);
                }
            }
            else if (hitObject.GetComponent<diary>() && hitObject.CompareTag("Pickupable"))
            {
                interactionText3.SetActive(true);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    hitObject.GetComponent<diary>().OpenCloseDiary();
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    objectsSound.Play();
                    pickUpScript.TryPickUp(hitObject);
                    interactionText3.SetActive(false);
                }
            }
            else if (hitObject.CompareTag("Bear"))
            {
                interactionText2 .SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    AudioSource audioSource = hitObject.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }
                }
            }
            else if (hitObject.CompareTag("Box"))
            {
                Keypad keypad = hitObject.GetComponent<Keypad>();
                if (!keypad.isOpen)
                {
                    interactionText2.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactionText2.SetActive(false);
                        keypad.ToggleKeypad();
                    }
                }
                else
                {
                    interactionText2.SetActive(false);
                }
            }

            else if (hitObject.CompareTag("Letter"))
            {
                interactionText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    hitObject.GetComponent<letter>().openCloseLetter();
                }
            }
            else if (hitObject.GetComponent<diary>())
            {
                interactionText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    hitObject.GetComponent<diary>().OpenCloseDiary();
                }
            }
            else if (hitObject.CompareTag("Pickupable"))
            {
                interactionText2.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    objectsSound.Play();
                    pickUpScript.TryPickUp(hitObject);
                    interactionText2.SetActive(false);
                }
            }
            else if (hitObject.CompareTag("KeypadDoor") || hitObject.CompareTag("KeypadSafe") || hitObject.CompareTag("GalaxyKeypad"))
            {
                Keypad keypad = hitObject.GetComponent<Keypad>();
                Collider keypadCollider = hitObject.GetComponent<Collider>();
                Animator doorAnimator = hitObject.GetComponentInParent<Animator>();
                AudioSource doorSound = hitObject.GetComponentInParent<AudioSource>();

                if (keypad != null && keypadCollider != null && doorAnimator != null)
                {
                    if (keypad.accessGranted)
                    {
                        if (hitObject.CompareTag("GalaxyKeypad"))  
                        {
                            playerController.gravity = 20f;
                        }
                        doorAnimator.SetTrigger("OpenDoor");
                        openSound.Play();

                        keypadCollider.enabled = false;
                        interactionText2.SetActive(false);
                    }
                    else
                    {
                        interactionText2.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            keypad.ToggleKeypad();
                            interactionText2.SetActive(false);
                        }
                    }
                }
            }
            else if (hitObject.CompareTag("Door"))
            {
                Collider doorCollider = hitObject.GetComponent<Collider>();
                Animator doorAnimator = hitObject.GetComponentInParent<Animator>();
                DoorTeleport doorTeleport = hitObject.GetComponent<DoorTeleport>();

                if (doorCollider != null && doorAnimator != null && doorTeleport != null)
                {
                    interactionText2.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        doorAnimator.SetTrigger("OpenDoor");
                        openSound.Play();

                        if (doorTeleport.isGalaxyDoor) 
                        {
                            playerController.gravity = 2f;  
                        }
                        else 
                        {
                            playerController.gravity = 20f;  
                        }

                        StartCoroutine(doorTeleport.Teleport());

                        doorAnimator.SetTrigger("CloseDoor");

                        doorCollider.enabled = false;
                        interactionText2.SetActive(false);
                    }
                }
            }

        }
    }
}




