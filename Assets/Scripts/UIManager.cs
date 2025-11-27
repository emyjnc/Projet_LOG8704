using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance => instance;

    public int selectedOption = 0;

    // Lists for each locomotion type
    public List<GameObject> teleportControllers = new List<GameObject>();
    public List<GameObject> turnControllers = new List<GameObject>();
    public List<GameObject> stepControllers = new List<GameObject>();
    public List<GameObject> slideControllers = new List<GameObject>();

    // Individually tracked right-hand interactors
    public GameObject rightTeleport;
    public GameObject rightTurn;
    public GameObject leftSlide;



    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FindLocomotionControllers());
    }

    private IEnumerator FindLocomotionControllers()
    {
        yield return null;

        teleportControllers.Clear();
        turnControllers.Clear();
        stepControllers.Clear();
        slideControllers.Clear();

        rightTeleport = null;
        rightTurn = null;

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var obj in allObjects)
        {
            if (!obj.scene.IsValid()) continue;

            string name = obj.name;

            // TELEPORT
            if (name == "TeleportControllerInteractor")
            {
                teleportControllers.Add(obj);

                if (rightTeleport == null && IsInRightHand(obj.transform))
                    rightTeleport = obj;
            }

            // TURN
            else if (name == "ControllerTurnerInteractor")
            {
                turnControllers.Add(obj);

                if (rightTurn == null && IsInRightHand(obj.transform))
                    rightTurn = obj;
            }

            // STEP
            else if (name == "ControllerStepInteractor")
            {
                stepControllers.Add(obj);
            }

            // SLIDE
            else if (name == "ControllerSlideInteractor")
            {
                slideControllers.Add(obj);

                if (leftSlide == null && IsInLeftHand(obj.transform))
                    leftSlide = obj;
            }
        }

        Debug.Log(
            $"Found locomotion: " +
            $"{teleportControllers.Count} Teleports, " +
            $"{turnControllers.Count} Turns, " +
            $"{stepControllers.Count} Steps, " +
            $"{slideControllers.Count} Slides.\n" +
            $"RightTeleport = {rightTeleport}\n" +
            $"RightTurn     = {rightTurn}"
        );

        ApplyLocomotionMode();
    }

    private bool IsInRightHand(Transform t)
    {
        while (t != null)
        {
            if (t.name.Contains("RightInteractions"))
                return true;
            t = t.parent;
        }
        return false;
    }

    private bool IsInLeftHand(Transform t)
    {
        while (t != null)
        {
            if (t.name.Contains("LeftInteractions"))
                return true;
            t = t.parent;
        }
        return false;
    }


    public void SetOption(int option)
    {
        selectedOption = option;
        ApplyLocomotionMode();
    }

    public void ApplyLocomotionMode()
    {
        // SAFETY: If objects are missing, skip
        if (teleportControllers.Count == 0)
            return;

        switch (selectedOption)
        {
            case 0:
                EnableOnlyRightTeleport();
                break;

            case 1:
                SetAllLocomotion(false);
                break;

            case 2:
                EnableOnlyRightTurn();
                break;
        }
    }

    private void EnableOnlyRightTeleport()
    {
        SetAllLocomotion(false);

        if (rightTeleport != null)
            rightTeleport.SetActive(true);

        Debug.Log("Locomotion: RIGHT TELEPORT ENABLED");
    }

    private void EnableOnlyRightTurn()
    {
        SetAllLocomotion(false);

        if (rightTurn != null)
            rightTurn.SetActive(true);
        if (leftSlide != null)
            leftSlide.SetActive(true);

        Debug.Log("Locomotion: RIGHT TURN ENABLED");
    }

    private void SetAllLocomotion(bool active)
    {
        foreach (var obj in teleportControllers) obj.SetActive(active);
        foreach (var obj in turnControllers) obj.SetActive(active);
        foreach (var obj in stepControllers) obj.SetActive(active);
        foreach (var obj in slideControllers) obj.SetActive(active);

        Debug.Log("Locomotion: ALL CONTROLLERS = " + active);
    }
}
