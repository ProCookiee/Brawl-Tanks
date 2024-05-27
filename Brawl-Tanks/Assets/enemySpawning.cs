using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class enemySpawning : MonoBehaviour
{
    public GameObject enemyPrefabLvl1;
    public bool pause = false;

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
        if (!pause)
        {
            var enemy = Instantiate(enemyPrefabLvl1, new Vector2(x, y), Quaternion.identity);
            enemy.name = "Enemy1";
        }
    }
}
