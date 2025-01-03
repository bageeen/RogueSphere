using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public int currentWave = 1;
    public int enemiesPerWave = 1;
    public float waveInterval = 5f;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    private int enemiesRemaining;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(waveInterval);
        SpawnWave();
    }

    void SpawnWave()
    {
        enemiesRemaining = enemiesPerWave * currentWave;
        for (int i = 0; i < enemiesRemaining; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public void OnEnemyKilled()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            currentWave++;
            StartCoroutine(StartNextWave());
        }
    }
}
