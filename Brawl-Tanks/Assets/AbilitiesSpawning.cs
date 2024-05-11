using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class AbilitiesSpawning : MonoBehaviour
{
    private struct AbilityData 
    {
        public GameObject abilityObject;
        public Vector3 location;
    }
    //public Vector3 topLeftCorner;
    //public Vector3 bottomRightCorner;
    //public GameObject abilityPrefab_laser;
    //public GameObject abilityPrefab_deathRay;
    //public GameObject wallPrefab;
    //public GameObject player1;
    //public GameObject player2;

    public float prefabSize = 0.08f;
    public float minSpawnRate = 3.0f;
    public float maxSpawnRate = 15.0f;
    public int maxAbilities = 6;
    public List<GameObject> abilityPrefabs;

    private float nextSpawnTime;
    private List<Vector3> spawnLocations; // Add missing variable declaration
    private int currentAbilityCount = 0;

    void Start()
    {
        // Set the initial spawn time
        nextSpawnTime = Time.time + Random.Range(minSpawnRate, maxSpawnRate);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if it's time to spawn a new ability
        if (Time.time >= nextSpawnTime)
        {
            SpawnAbility();
            // Calculate spawn rate based on the number of existing abilities
            float spawnRate = Mathf.Lerp(minSpawnRate, maxSpawnRate, currentAbilityCount / (float)maxAbilities); 
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnAbility()
    {
        Vector3 spawnLocation = GameManager.instance.GenerateSpawnLocation(3);
        GameObject abilityPrefab = abilityPrefabs[Random.Range(0, abilityPrefabs.Count)];
        GameObject abilityInstance = Instantiate(abilityPrefab, spawnLocation, Quaternion.identity);
        abilityInstance.transform.localScale = new Vector3(prefabSize, prefabSize, 1); // replace with your desired size

        // Increment ability count
        currentAbilityCount++; 
        // Attach a OnDestroy listener to the spawned ability
        abilityInstance.AddComponent<AbilityCleanup>().onDestroyCallback = OnAbilityDestroyed;     
    }
    private void OnAbilityDestroyed()
    {
        // Decrement ability count
        currentAbilityCount--;
    }
}

// Helper component to trigger an event when an ability GameObject is destroyed
public class AbilityCleanup : MonoBehaviour
{
    public System.Action onDestroyCallback; 

    private void OnDestroy()
    {
        if (onDestroyCallback != null)
        {
            onDestroyCallback();
        }
    }
}
