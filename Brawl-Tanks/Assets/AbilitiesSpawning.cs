using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections.Specialized;



public class AbilitiesSpawning : MonoBehaviour
{

    public AbilityScript abilityScript;

    public float prefabSize = 0.08f;
    public float minSpawnRate = 3.0f;
    public float maxSpawnRate = 15.0f;
    public int maxAbilities = 6;
    public List<GameObject> abilityPrefabs;
    

    private float nextSpawnTime;
    //shranjuje lokacije, kjer so že abilityji
    private List<Vector3> busyLocations = new();
    //število trenutno spawnanih abilityjev
    private int currentAbilityCount = 0;

    void Start()
    {
        // Set the initial spawn time
        nextSpawnTime = Time.time + Random.Range(minSpawnRate, maxSpawnRate);
        // Subscribe to the CollectionChanged event
        abilityScript.spawnedAbilities.CollectionChanged += SpawnedAbilities_CollectionChanged;
    }

    // to je listener lista ki je v AbilityScript.cs, ko shranjuje aktivne abilityje
    // This method will be called whenever an item is added or removed from spawnedAbilities
    private void SpawnedAbilities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        //abilityji so dodani
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach(GameObject newItem in e.NewItems)
            {
                currentAbilityCount++;
                busyLocations.Add(newItem.transform.position);
            }
        }
        //abilityi so odstranjeni
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach(GameObject oldItem in e.OldItems)
            {
                currentAbilityCount--;
                busyLocations.Remove(oldItem.transform.position);
            }
        }
        Debug.Log("Current ability count: " + currentAbilityCount);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if it's time to spawn a new ability
        if (Time.time >= nextSpawnTime && currentAbilityCount < maxAbilities)
        {
            // Calculate spawn rate based on the number of existing abilities
            float spawnRate = Mathf.Lerp(minSpawnRate, maxSpawnRate, currentAbilityCount / (float)maxAbilities); 
            nextSpawnTime = Time.time + spawnRate;
            SpawnAbility();
        }
        
    }

    void SpawnAbility()
    {
        Vector3 spawnLocation;
        // Keep generating a new spawn location until a free one is found
        do{
            spawnLocation = GameManager.instance.GenerateSpawnLocation(3);
        } while (busyLocations.Contains(spawnLocation));
        // Select a random ability prefab
        GameObject abilityPrefab = abilityPrefabs[Random.Range(0, abilityPrefabs.Count)];
        //GameObject abilityPrefab = abilityPrefabs[2]; // 0 = laser, 1 = ray, 2 = frag 3 = gatling gun, 4 = rc, 5 = shield
        GameObject abilityInstance = Instantiate(abilityPrefab, spawnLocation, Quaternion.identity);
        // Spremeni velikost abilityja (lak nastavlas v unityu)
        abilityInstance.transform.localScale = new Vector3(prefabSize, prefabSize, 1);
        abilityInstance.name = abilityPrefab.name;
        // Dodaj ability v listo aktivnih abilityjev v AbilityScript.cs
        abilityScript.newAbility(abilityInstance); 
    }
}

