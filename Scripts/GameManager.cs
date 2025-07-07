using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    [Header("Save Settings")]
    public float saveInterval = 5f;
    private float timeSinceLastSave = 0f;
    public Transform playerTransform;

    [Header("References")]
    public ProgressManager progressManager;
    public GameObject loadingScreen;

    private string playerEmail;
    private float autoSaveTimer = 0f;
    public float autoSaveInterval = 30f;

    private void Start()
    {
        playerEmail = LoggedInUser.Email;

        if (progressManager == null)
        {
            UnityEngine.Debug.LogError("ProgressManager is NOT assigned!");
            return;
        }

        if (PersistentDataManager.instance != null)
        {
            var optionsMenu = FindObjectOfType<OptionsMenu>();
            if (optionsMenu != null && PersistentDataManager.instance.playerSettings != null)
            {
                optionsMenu.ApplyLoadedSettings(PersistentDataManager.instance.playerSettings);
                UnityEngine.Debug.Log("Manually reapplied settings from PersistentDataManager in Escape House.");
            }
        }

        if (PersistentDataManager.instance != null && PersistentDataManager.instance.hasLoadedFromContinue)
        {
            loadingScreen?.SetActive(true);
        }

        UnityEngine.Debug.Log("Starting GameManager...");
        StartCoroutine(HandleProgress());
    }

    private void Update()
    {
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            autoSaveTimer = 0f;
            StartCoroutine(SavePlayerProgress());
        }
    }

    private IEnumerator HandleProgress()
    {
        if (PersistentDataManager.instance != null && PersistentDataManager.instance.hasLoadedFromContinue)
        {
            yield return new WaitUntil(() => ContinueData.loadedProgress != null);

            UnityEngine.Debug.Log("Loaded progress found. Applying...");

            PlayerProgressData progressData = ContinueData.loadedProgress;
            PlayerSettingsData settingsData = ContinueData.loadedSettings;


            if (progressData != null)
            {
                yield return ApplyProgressWithLoadingScreen(progressData);
            }

            if (settingsData != null)
            {
                ApplyPlayerSettings(settingsData);
            }

        }
        else
        {
            UnityEngine.Debug.Log("No saved progress found. Starting a new game.");
        }

        yield return new WaitForSeconds(5f);
        yield return SavePlayerProgress();
    }

    public IEnumerator SavePlayerProgress()
    {
        UnityEngine.Debug.Log("Saving Player Progress...");

        UnityEngine.Vector3 position = playerTransform.position;
        UnityEngine.Vector3 rotation = playerTransform.eulerAngles;

        List<string> openedSafes = new();
        KeypadSafe[] allSafeKeypads = GameObject.FindObjectsOfType<KeypadSafe>();
        foreach (var safeKeypad in allSafeKeypads)
        {
            if (safeKeypad.uniqueID.Contains("Safe") && safeKeypad.accessGranted)
            {
                openedSafes.Add(safeKeypad.uniqueID);
                UnityEngine.Debug.Log($"[SAVE] Safe opened: {safeKeypad.uniqueID}");
            }
        }

        List<string> openedDoors = new();
        Keypad[] allDoorKeypads = GameObject.FindObjectsOfType<Keypad>();
        foreach (var doorKeypad in allDoorKeypads)
        {
            if (!(doorKeypad is KeypadSafe) && doorKeypad.uniqueID.Contains("Door") && doorKeypad.accessGranted)
            {
                openedDoors.Add(doorKeypad.uniqueID);
                UnityEngine.Debug.Log($"[SAVE] Door opened: {doorKeypad.uniqueID}");
            }
        }

        var framePlacements = new List<FrameCardPlacement>();
        HistoryFrame[] frames = GameObject.FindObjectsOfType<HistoryFrame>();
        foreach (var frame in frames)
        {
            if (frame.hasCard)
            {
                string frameID = frame.frameID;
                string cardID = frame.GetPlacedCardID();
                if (!string.IsNullOrEmpty(cardID))
                {
                    framePlacements.Add(new FrameCardPlacement
                    {
                        frameID = frameID,
                        cardID = cardID
                    });
                }
            }
        }

        PlayerProgressData progress = new()
        {
            posX = position.x,
            posY = position.y,
            posZ = position.z,
            rotX = rotation.x,
            rotY = rotation.y,
            rotZ = rotation.z,

            openedSafes = openedSafes,
            openedDoors = openedDoors,

            gameFlags = new List<FlagData>
        {
            new FlagData("galaxyDoorUsed", DoorTeleport.galaxyDoorUsed),
            new FlagData("isGameFinished", FinalLetter.creditsShown)
        },
            movedObjects = new List<MovedObjectData>()
           
        };
        progress.cardPlacements = framePlacements;

        MoveObjects moveObjects = FindObjectOfType<MoveObjects>();
        if (moveObjects != null)
        {
            foreach (GameObject obj in moveObjects.interactableObjects)
            {
                if (obj != null)
                {
                    progress.movedObjects.Add(new MovedObjectData()
                    {
                        objectName = obj.name,
                        position = obj.transform.position,
                        rotationX = obj.transform.eulerAngles.x,
                        rotationY = obj.transform.eulerAngles.y,
                        rotationZ = obj.transform.eulerAngles.z
                    });
                }
            }
        }

        yield return progressManager.SaveProgress(playerEmail, progress);
    }

    private void ApplyPlayerSettings(PlayerSettingsData settings)
    {
        OptionsMenu optionsMenu = FindObjectOfType<OptionsMenu>();
        if (optionsMenu != null)
        {
            optionsMenu.ApplyLoadedSettings(settings);
        }
        else
        {
            UnityEngine.Debug.LogWarning("OptionsMenu not found. Settings not applied.");
        }
    }

    public IEnumerator ApplyProgressWithLoadingScreen(PlayerProgressData loadedData)
    {
        playerTransform.position = new UnityEngine.Vector3(loadedData.posX, loadedData.posY, loadedData.posZ);
        playerTransform.eulerAngles = new UnityEngine.Vector3(loadedData.rotX, loadedData.rotY, loadedData.rotZ);

        yield return ApplyMovedObjectsAfterDelay(loadedData.movedObjects);

        yield return new WaitForSeconds(0.5f);

        if (loadedData.openedSafes != null)
        {
            KeypadSafe[] allSafeKeypads = GameObject.FindObjectsOfType<KeypadSafe>();
            foreach (var safeKeypad in allSafeKeypads)
            {
                if (loadedData.openedSafes.Contains(safeKeypad.uniqueID))
                {
                    safeKeypad.accessGranted = true;
                    safeKeypad.enabled = false;

                    Collider col = safeKeypad.GetComponent<Collider>();
                    if (col != null) col.enabled = false;

                    Animator anim = safeKeypad.GetComponentInParent<Animator>();
                    if (anim != null)
                    {
                        anim.SetTrigger("SafeOpen");
                    }
                }
            }
        }

        if (loadedData.openedDoors != null)
        {
            Keypad[] allDoorKeypads = GameObject.FindObjectsOfType<Keypad>();
            foreach (var doorKeypad in allDoorKeypads)
            {
                if (!(doorKeypad is KeypadSafe) &&
                    loadedData.openedDoors.Contains(doorKeypad.uniqueID))
                {
                    doorKeypad.accessGranted = true;
                    doorKeypad.enabled = false;

                    Collider col = doorKeypad.GetComponent<Collider>();
                    if (col != null) col.enabled = false;

                    Animator anim = doorKeypad.GetComponentInParent<Animator>();
                    if (anim != null)
                    {
                        anim.SetTrigger("OpenDoor");
                    }
                }
            }
        }

        if (loadedData.cardPlacements != null)
        {
            Dictionary<string, GameObject> cardsByID = new();
            CardIdentifier[] allCards = GameObject.FindObjectsOfType<CardIdentifier>();
            foreach (var card in allCards)
            {
                cardsByID[card.cardID] = card.gameObject;
            }

            HistoryFrame[] allFrames = GameObject.FindObjectsOfType<HistoryFrame>();
            foreach (var frame in allFrames)
            {
                foreach (var placement in loadedData.cardPlacements)
                {
                    if (frame.frameID == placement.frameID && cardsByID.TryGetValue(placement.cardID, out GameObject card))
                    {
                        frame.LoadCard(card);
                        break;
                    }
                }
            }
        }

        if (loadedData.GetFlag("galaxyDoorUsed"))
        {
            DoorTeleport.galaxyDoorUsed = true;

            DoorTeleport[] teleports = GameObject.FindObjectsOfType<DoorTeleport>();
            foreach (var tp in teleports)
            {
                if (tp.isGalaxyDoor)
                {
                    Collider col = tp.GetComponent<Collider>();
                    if (col != null) col.enabled = false;

                    tp.enabled = false; 
                }
            }
        }

        if (loadedData.GetFlag("isGameFinished"))
        {
            FinalLetter finalLetter = FindObjectOfType<FinalLetter>();
            if (finalLetter != null)
            {
                finalLetter.CloseFinalLetter();
            }
        }


        yield return new WaitForSeconds(2f);
        loadingScreen?.SetActive(false);
    }

    private IEnumerator ApplyMovedObjectsAfterDelay(List<MovedObjectData> movedObjects)
    {
        float timeout = 5f;
        float elapsed = 0f;
        MoveObjects moveObjects = null;

        while (moveObjects == null && elapsed < timeout)
        {
            moveObjects = FindObjectOfType<MoveObjects>();
            if (moveObjects == null)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        if (moveObjects == null || moveObjects.interactableObjects == null)
            yield break;

        elapsed = 0f;
        while (moveObjects.interactableObjects.Count == 0 && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var movedObject in movedObjects)
        {
            foreach (GameObject obj in moveObjects.interactableObjects)
            {
                if (obj.name == movedObject.objectName)
                {
                    obj.transform.position = movedObject.position;
                    obj.transform.eulerAngles = new UnityEngine.Vector3(
                    movedObject.rotationX,
                    movedObject.rotationY,
                    movedObject.rotationZ
                );
                    break;
                }
            }
        }
    }

    public void SaveBeforeExit()
    {
        UnityEngine.Debug.Log("Saving progress before exiting...");
        StartCoroutine(SaveAndQuit());
    }

    private IEnumerator SaveAndQuit()
    {
        yield return SavePlayerProgress();
        yield return new WaitForSeconds(0.5f); 

        OptionsMenu optionsMenu = FindObjectOfType<OptionsMenu>();
        if (optionsMenu != null)
        {
            PlayerSettingsData currentSettings = optionsMenu.GetCurrentSettings();
        }

        UnityEngine.Debug.Log("Progress saved. Quitting...");
        Application.Quit();
    }
}
