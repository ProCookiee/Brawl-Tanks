using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawning : MonoBehaviour
{
    public GameObject enemyPrefabLvl1;
    public GameObject enemyPrefabLvl2;
    public GameObject enemyPrefabLvl3;
    public bool pause = false;

    private int maxEnemy2Count = 3;
    private int maxEnemy3Count = 2;
    private int maxTotalEnemyCount = 10;
    private int currentEnemy2Count = 0;
    private int currentEnemy3Count = 0;
    private int currentTotalEnemyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    // Coroutine to spawn enemies at a 2-second interval
    IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if (!pause)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        if (currentTotalEnemyCount >= maxTotalEnemyCount)
        {
            return;
        }

        int pos = Random.Range(0, 2);
        float x = 100;
        float y = 100;
        switch (pos)
        {
            case 0:
                x = Random.Range(-9, 9);
                y = Random.Range(0, 2);
                y = y == 0 ? -5 : 5;
                break;
            case 1:
                x = Random.Range(0, 2);
                y = Random.Range(-5, 5);
                x = x == 0 ? -9 : 9;
                break;
        }

        int rand = Random.Range(0, 4);
        if (rand == 0)
        {
            var enemy = Instantiate(enemyPrefabLvl1, new Vector2(x, y), Quaternion.identity);
            enemy.name = "Enemy1";
            currentTotalEnemyCount++;
        }
        else if (rand == 1 && currentEnemy2Count < maxEnemy2Count)
        {
            var enemy = Instantiate(enemyPrefabLvl2, new Vector2(x, y), Quaternion.identity);
            enemy.name = "Enemy2";
            currentEnemy2Count++;
            currentTotalEnemyCount++;
        }
        else if (rand == 1 && currentEnemy2Count >= maxEnemy2Count)
        {
            var enemy = Instantiate(enemyPrefabLvl1, new Vector2(x, y), Quaternion.identity);
            enemy.name = "Enemy1";
            currentTotalEnemyCount++;
        }
        else if (rand == 2 && currentEnemy3Count < maxEnemy3Count) // Spawning level 3 enemies without extra constraints
        {
            var enemy = Instantiate(enemyPrefabLvl3, new Vector2(x, y), Quaternion.identity);
            enemy.name = "Enemy3";
            currentEnemy3Count++;
            currentTotalEnemyCount++;
        }
        else if (rand == 2 && currentEnemy3Count >= maxEnemy3Count)
        {
            var enemy = Instantiate(enemyPrefabLvl1, new Vector2(x, y), Quaternion.identity);
            enemy.name = "Enemy1";
            currentTotalEnemyCount++;
        }
    }

    public void OnEnemyDestroyed(string enemyName)
    {
        if (enemyName == "Enemy1")
        {
            currentTotalEnemyCount--;
        }
        else if (enemyName == "Enemy2")
        {
            currentTotalEnemyCount--;
            currentEnemy2Count--;
        }
        else if (enemyName == "Enemy3")
        {
            currentTotalEnemyCount--;
        }
    }
}
