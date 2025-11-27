using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionButton : MonoBehaviour
{
    public int optionIndex;

    private Button btn;
    private Image img;
    private bool autoClicked = false; // prevents multiple auto-clicks

    void Awake()
    {
        btn = GetComponent<Button>();
        img = GetComponent<Image>();
        btn.onClick.AddListener(OnClick);
    }

    void OnEnable()
    {
        // Always refresh visuals
        RefreshVisuals();

        // Try auto-click (delayed so UI has time to spawn)
        StartCoroutine(TryAutoClick());
    }

    private IEnumerator TryAutoClick()
    {
        // Wait a frame so UI is present
        yield return null;

        // Only the selected option should auto-click itself
        if (!autoClicked && UIManager.Instance.selectedOption == optionIndex)
        {
            autoClicked = true;

            Debug.Log("Auto-clicking: " + name);

            btn.onClick.Invoke();  // fires real UnityEvents
        }
    }

    private void OnClick()
    {
        UIManager.Instance.SetOption(optionIndex);
        ApplyOption();
        RefreshVisuals();
    }

    public void ApplyOption()
    {
        Debug.Log("Option applied: " + optionIndex);
        // Add your real option logic here
    }

    public void RefreshVisuals()
    {
        bool isActive = UIManager.Instance.selectedOption == optionIndex;

        btn.interactable = !isActive;
    }
}
