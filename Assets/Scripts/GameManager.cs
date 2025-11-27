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
    public int remainingTrash = 0;

    private WorldUiToggleSpawner uitoggle;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        uitoggle = FindFirstObjectByType<WorldUiToggleSpawner>();
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

    public void RegisterTrash()
    {
        remainingTrash++;
    }

    public void TrashDisposed()
    {
        remainingTrash--;
        if(remainingTrash <= 0 & timerRunning)
        {
            stopTimer();
            uitoggle.Toggle();
            uitoggle.SetText("Game Over, Score: " + score.ToString());
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
            if(timerRunning == false)
            {
                timerText.text = timerText.text;
            }
            else
            {
                timerText.text = "Timer : " + timer.ToString("F2") + "s";

            }
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
            scoreText.text =score.ToString() + remainingTrash.ToString();
        }
    }

    
}
