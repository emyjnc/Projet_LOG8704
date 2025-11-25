using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompostGameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> spawners = new List<GameObject>();
    [SerializeField]
    private List<GameObject> items = new List<GameObject>();
    
    void Start()
    {
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
}
