using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int score = 0;
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore()
    {
        score++;
        UpdateUI();
    }

    public void RemoveScore()
    {
        score--;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
}
