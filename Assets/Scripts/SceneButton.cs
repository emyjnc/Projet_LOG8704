using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [Tooltip("The scene this button should load.")]
    public string targetScene;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
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

        // Disable interaction
        button.interactable = !isCurrentScene;

        // Optional: visually mark the active scene
        if (isCurrentScene)
            button.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f); // gray out
        else
            button.GetComponent<Image>().color = Color.white;
    }

    public void LoadScene()
    {
        if (button.interactable)
            SceneManager.LoadScene(targetScene);
    }
}
