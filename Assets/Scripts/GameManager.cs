using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int score = 0;
    public static GameManager Instance;

    private float timer = 0;
    private bool timerRunning = false;

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
        if(timerRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }
    public void startTimer()
    {
        if (!timerRunning)
        {
            timer = 0f;
            timerRunning = true;

        }
    }

    public void stopTimer()
    {
        timerRunning = false; 
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = timer.ToString("F2") + "s";
        }
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

    public void CheckEnd()
    {
        TrashItem[] items = FindObjectsOfType<TrashItem>(true);
        foreach(var item in items)
        {
            if (item.gameObject.activeSelf)
            {
                return;
            }
        }
        stopTimer();

    }
}
