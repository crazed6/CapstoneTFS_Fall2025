using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform player;
    private List<WorkerBehav> enemies = new List<WorkerBehav>();

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }

    void Update()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                
            }
        }
    }

    public void RegisterEnemy(WorkerBehav enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            Debug.Log($"Enemy {enemy.name} registered.");
        }
    }

    public void UnregisterEnemy(WorkerBehav enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Debug.Log($"Enemy {enemy.name} unregistered.");
        }
    }
}

