using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class enemySpawning : MonoBehaviour
{
    public GameObject enemyPrefabLvl1;
    public GameObject enemyPrefabLvl2;
    public bool pause = false;

    private int maxEnemy2Count = 3;
    private int maxTotalEnemyCount = 10;
    private int currentEnemy2Count = 0;
    private int currentTotalEnemyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Example: Spawn enemies every 2 seconds
        if (Time.frameCount % 120 == 0) // Assuming 60 frames per second
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (pause || currentTotalEnemyCount >= maxTotalEnemyCount)
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
                if (y == 0)
                {
                    y = -5;
                }
                else
                {
                    y = 5;
                }
                break;
            case 1:
                x = Random.Range(0, 2);
                y = Random.Range(-5, 5);
                if (x == 0)
                {
                    x = -9;
                }
                else
                {
                    x = 9;
                }
                break;
        }

        int rand = Random.Range(0, 3);
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
    }
}
