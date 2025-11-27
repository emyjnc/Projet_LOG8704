using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CompostGameManager : MonoBehaviour
{
    public static CompostGameManager Instance = null;
    [SerializeField]
    private List<GameObject> spawners = new List<GameObject>();
    [SerializeField]
    private List<GameObject> items = new List<GameObject>();
    [SerializeField]
    private TextMeshProUGUI scoreUI = null;

    private WorldUiToggleSpawner uitoggle;

    private bool gameOver = false;

    private int score = 0;
    private int maxScore = 0;
    
    void Start()
    {
        Instance = this;
        uitoggle = FindFirstObjectByType<WorldUiToggleSpawner>();
        scoreUI.text = "Score: " + score.ToString();

        if (!spawners.Any() || !items.Any())
            return;

        CalculateMaxScore();

        List<GameObject> availableObjects = new List<GameObject>(items);
        foreach (GameObject spawner in spawners)
        {
            int rand = Random.Range(0, availableObjects.Count);
            Instantiate(availableObjects[rand], spawner.transform);
            availableObjects.Remove(availableObjects[rand]);
        }
    }

    void Update()
    {
        if (score >= maxScore & !gameOver)
        {

            uitoggle.Toggle();
            uitoggle.SetText("Game Over, Score: " + score.ToString());
            gameOver = true;

        }
    }

    public void AddToScore()
    {
        score++;
        scoreUI.text = "Score: " + score.ToString();
    }

    private void CalculateMaxScore()
    {
        foreach (GameObject item in items)
        {
            if (item.GetComponent<ItemComponent>().type == ItemType.Compostable)
                maxScore++;
        }
    }
}
