using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        [Range(0, 100)]
        public float spawnProbability; // Probability to spawn this enemy (0 to 100)
    }

    public List<EnemySpawnInfo> enemiesToSpawn = new List<EnemySpawnInfo>();

    void Start()
    {
        // Normalize probabilities to ensure they add up to 100
        NormalizeProbabilities();
    }

    private void NormalizeProbabilities()
    {
        float totalProbability = 0f;

        // Calculate the total probability
        foreach (EnemySpawnInfo enemy in enemiesToSpawn)
        {
            totalProbability += enemy.spawnProbability;
        }

        // Normalize probabilities so that they add up to 100
        if (totalProbability > 0)
        {
            foreach (EnemySpawnInfo enemy in enemiesToSpawn)
            {
                enemy.spawnProbability = (enemy.spawnProbability / totalProbability) * 100f;
            }
        }
    }

    public GameObject SpawnRandomEnemy(int level)
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeProbability = 0f;

        Transform parent = transform.parent.parent.GetChild(3);

        foreach (EnemySpawnInfo enemy in enemiesToSpawn)
        {
            cumulativeProbability += enemy.spawnProbability;
            if (randomValue < cumulativeProbability)
            {
                // If enemyPrefab is null, it means we decided to spawn no enemy
                if (enemy.enemyPrefab != null)
                {
                    GameObject enemyObject = Instantiate(enemy.enemyPrefab, transform.position, Quaternion.identity, parent);
                    StartCoroutine(ApplyLevelToEntity(enemyObject, level));
                    return enemyObject;
                }
                break;
            }
        }
        return null;
    }

    IEnumerator ApplyLevelToEntity(GameObject enemyObject, int level)
    {
        yield return null;
        yield return null;
        enemyObject.GetComponent<SpawnLevelManager>().ApplyLevels(level);
    }
}
