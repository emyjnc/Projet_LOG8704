using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WorldUiToggleSpawner : MonoBehaviour
{
    [Header("UI Prefab")]
    public GameObject uiPrefab;

    [Header("Head Pose (Meta OVRCameraRig)")]
    public OVRCameraRig ovrRig;              // optional; will auto-find
    public Transform fallbackHead;           // optional; e.g., CenterEyeAnchor if you want to drag it manually

    [Header("Input System")]
    public InputActionProperty toggleAction; // bind to a controller button

    [Header("Spawn Pose")]
    public float spawnDistance = 1.0f;
    public float heightOffset = -0.1f;
    public bool lockUpright = true;
    public bool faceUser = true;

    [Header("Behavior")]
    public float respawnDistance = 2.0f;     // if user is farther than this from existing UI, destroy+respawn
    public bool closeByHiding = true;        // close = SetActive(false) vs Destroy()

    private GameObject _instance;

    public GameObject Instance => _instance;

    public TMP_Text minigameText;


    public void SetText(string value)
    {
        if (_instance == null)
        {
            Debug.LogWarning("UI instance not spawned yet.");
            return;
        }

        minigameText.text = value;
    }


    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initial lookup for first scene
        FindRig();

    }


    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find rig in newly loaded scene
        FindRig();

        // Auto-open the UI when loading the MainMenu scene
        if (scene.name == "MainMenu")
        {
            // Delay one frame to ensure the rig + menu canvas are ready
            StartCoroutine(OpenUIOnNextFrame());
        }
    }

    private IEnumerator OpenUIOnNextFrame()
    {
        yield return null;     // wait 1 frame so head pose is stable
        Toggle();              // spawn the menu UI
    }


    void OnEnable()
    {
        if (toggleAction.action != null)
        {
            toggleAction.action.Enable();
            toggleAction.action.performed += _ => Toggle();
        }
    }

    void OnDisable()
    {
        if (toggleAction.action != null)
        {
            toggleAction.action.performed -= _ => Toggle();
            toggleAction.action.Disable();
        }
    }



    private void FindRig()
    {
        if (ovrRig == null)
        {
            GameObject rigObject = GameObject.Find("[BuildingBlock] Camera Rig");
            if (rigObject != null)
            {
                ovrRig = rigObject.GetComponent<OVRCameraRig>();
                Debug.Log("Found OVRCameraRig in scene: " + ovrRig.name);
            }
            else
            {
                Debug.LogWarning("Could not find [BuildingBlock] Camera Rig in scene: " + SceneManager.GetActiveScene().name);
            }
        }
    }

    Transform Head()
    {
        if (ovrRig != null && ovrRig.centerEyeAnchor != null) return ovrRig.centerEyeAnchor;
        if (fallbackHead != null) return fallbackHead;
        return Camera.main != null ? Camera.main.transform : null;
    }

    public void Toggle()
    {
        var head = Head();
        if (head == null || uiPrefab == null) return;

        if (_instance != null)
        {
            float d = Vector3.Distance(head.position, _instance.transform.position);

            if (d > respawnDistance)
            {
                Destroy(_instance);
                _instance = null;
                Spawn(head);
                return;
            }

            if (closeByHiding)
            {
                _instance.SetActive(!_instance.activeSelf);
                if (_instance.activeSelf && faceUser) AlignToUser(head, _instance.transform);
            }
            else
            {
                Destroy(_instance);
                _instance = null;
            }

            return;
        }

        Spawn(head);
    }

    void Spawn(Transform head)
    {
        Vector3 forward = head.forward;
        if (lockUpright)
        {
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
            forward.Normalize();
        }

        Vector3 pos = head.position + forward * spawnDistance;
        pos.y += heightOffset;

        _instance = Instantiate(uiPrefab, pos, Quaternion.identity);

        Transform t = _instance.transform.Find("UI Minigame/CanvasRoot/UIBackplate/Text (TMP)");

        if (t != null)
            minigameText = t.GetComponent<TMP_Text>();
        else
            Debug.LogError("Could not find Text (TMP) inside UI(Clone) - check the path!");

        if (faceUser) AlignToUser(head, _instance.transform);
    }

    void AlignToUser(Transform head, Transform ui)
    {
        Vector3 toUser = head.position - ui.position;
        if (lockUpright) toUser.y = 0f;
        if (toUser.sqrMagnitude < 0.0001f) return;

        ui.rotation = Quaternion.LookRotation(-toUser.normalized, Vector3.up);
    }
}
