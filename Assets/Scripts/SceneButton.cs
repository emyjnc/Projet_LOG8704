using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneButton : MonoBehaviour
{
    [Tooltip("The scene this button should load.")]
    public string targetScene;

    private Button button;
    private TMP_Text label;   // Text on the button

    void Awake()
    {
        button = GetComponent<Button>();
        label = GetComponentInChildren<TMP_Text>(true);
        UpdateState();
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        UpdateState();
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        UpdateState();
    }

    private void UpdateState()
    {
        string current = SceneManager.GetActiveScene().name;
        bool isCurrentScene = current == targetScene;

        // Special rule for MainMenu (keep the old disable logic)
        if (targetScene == "MainMenu")
        {
            button.interactable = !isCurrentScene;

            if (isCurrentScene)
                button.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
            else
                button.GetComponent<Image>().color = Color.white;

            if (label != null)
                label.text = targetScene;

            return;
        }

        // For every other scene:
        if (isCurrentScene)
        {
            // Change text to Restart, keep button enabled
            if (label != null)
                label.text = "Restart";

            button.interactable = true;
            button.GetComponent<Image>().color = Color.white;
        }
        else
        {
            // Normal behavior: show the scene name as label
            if (label != null)
                label.text = targetScene;

            button.interactable = true;
            button.GetComponent<Image>().color = Color.white;
        }
    }

    public void LoadScene()
    {
        string current = SceneManager.GetActiveScene().name;

        if (targetScene == current)
        {
            // Restart current scene
            SceneManager.LoadScene(current);
        }
        else
        {
            // Load the chosen scene
            SceneManager.LoadScene(targetScene);
        }
    }
}
