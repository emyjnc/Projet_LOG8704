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

    private int score = 0;
    
    void Start()
    {
        Instance = this;
        scoreUI.text = "Score: " + score.ToString();

        if (!spawners.Any() || !items.Any())
            return;

        List<GameObject> availableObjects = new List<GameObject>(items);
        foreach (GameObject spawner in spawners)
        {
            int rand = Random.Range(0, availableObjects.Count);
            Instantiate(availableObjects[rand], spawner.transform);
            availableObjects.Remove(availableObjects[rand]);
        }
    }

    public void AddToScore()
    {
        score++;
        scoreUI.text = "Score: " + score.ToString();
    }
}
