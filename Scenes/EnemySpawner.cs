using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemy;
    public float startTime = 2f;
    private float timeBetweenSpawn;
    private bool stopSpawning = false;

    void Start()
    {
        timeBetweenSpawn = startTime;
        if (spawnPoints.Length == 0 || enemy == null)
        {
            Debug.LogError("Enemy or spawn points array is empty! Spawner won't work.");
            stopSpawning = true;
            return;
        }
    }

    void Update()
    {
        if (stopSpawning) return;

        // Spawn an enemy when the timeBetweenSpawn is up
        if (timeBetweenSpawn <= 0f)
        {
            SpawnEnemy();
            timeBetweenSpawn = startTime;  // Reset spawn timer
        }
        else
        {
            timeBetweenSpawn -= Time.deltaTime;  // Decrease spawn timer
        }

        // Debugging to check active enemy count
        Debug.Log($"Spawning timer: {timeBetweenSpawn}");
    }

    void SpawnEnemy()
    {
        // Check if there are spawn points
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Enemy or SpawnPoints array is empty. Cannot spawn enemies.");
            return;
        }

        // Randomly choose a spawn point
        int randomPosition = Random.Range(0, spawnPoints.Length);
        Debug.Log($"Spawning enemy at position {randomPosition}");

        // Instantiate the enemy
        GameObject newEnemy = Instantiate(enemy, spawnPoints[randomPosition].position, Quaternion.identity);

        // Attach the OnDeath listener to handle when the enemy is destroyed
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath += DecrementEnemyCount;  // Subscribe to the OnDeath event
        }

        // Debugging to check after spawning
        Debug.Log("Enemy spawned.");
    }

    // Method to stop spawning
    public void StopSpawning()
    {
        stopSpawning = true;
    }

    // This method is no longer necessary for active enemy count, but keeping it for cleanup
    private void DecrementEnemyCount()
    {
        // No need to decrement active enemy count anymore, as there's no limit
        Debug.Log("Enemy destroyed.");
    }
}
