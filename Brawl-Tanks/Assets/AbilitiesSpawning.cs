using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine.SceneManagement;


public class AbilitiesSpawning : MonoBehaviour
{

    public AbilityScript abilityScript;

    public float prefabSize = 0.08f;
    public float minSpawnRate = 5.0f;
    public float maxSpawnRate = 15.0f;
    public int maxAbilities = 4;
    public List<GameObject> abilityPrefabs;


    private float nextSpawnTime;
    //shranjuje lokacije, kjer so že abilityji
    private List<Vector3> busyLocations = new();
    //število trenutno spawnanih abilityjev
    private int currentAbilityCount = 0;

    GameManager gameManager;
    public int chosenAbility = -1;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "DeathMatch")
        {
            // Set the initial spawn time
            nextSpawnTime = Time.time + Random.Range(minSpawnRate, maxSpawnRate);
            // Subscribe to the CollectionChanged event
            abilityScript.spawnedAbilities.CollectionChanged += SpawnedAbilities_CollectionChanged;
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (gameManager.currentModifier == 2)
            {
                maxSpawnRate *= 0.5f;
                minSpawnRate *= 0.5f;
                maxAbilities = 8;
            }
            else if (gameManager.currentModifier == 4)
            {
                chosenAbility = Random.Range(0, abilityPrefabs.Count);
            }
        }
    }

    // to je listener lista ki je v AbilityScript.cs, ko shranjuje aktivne abilityje
    // This method will be called whenever an item is added or removed from spawnedAbilities
    private void SpawnedAbilities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        //abilityji so dodani
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (GameObject newItem in e.NewItems)
            {
                currentAbilityCount++;
                busyLocations.Add(newItem.transform.position);
            }
        }
        //abilityi so odstranjeni
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (GameObject oldItem in e.OldItems)
            {
                currentAbilityCount--;
                busyLocations.Remove(oldItem.transform.position);
            }
        }
        Debug.Log("Current ability count: " + currentAbilityCount + " / " + maxAbilities);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "DeathMatch")
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
    }

    void SpawnAbility()
    {
        Vector3 spawnLocation;
        // Keep generating a new spawn location until a free one is found
        do
        {
            spawnLocation = GameManager.instance.GenerateSpawnLocation(3);
        } while (busyLocations.Contains(spawnLocation));
        // Select a random ability prefab
        /*
        GameObject abilityPrefab = null;
        if (chosenAbility == -1)
        {
            abilityPrefab = abilityPrefabs[Random.Range(0, abilityPrefabs.Count)];
        }
        else if (chosenAbility == 0)
        {
            abilityPrefab = abilityPrefabs[0];
        }
        else if (chosenAbility == 1)
        {
            abilityPrefab = abilityPrefabs[1];
        }
        else if (chosenAbility == 2)
        {
            abilityPrefab = abilityPrefabs[2];
        }
        else if (chosenAbility == 3)
        {
            abilityPrefab = abilityPrefabs[3];
        }
        else if (chosenAbility == 4)
        {
            abilityPrefab = abilityPrefabs[4];
        }
        else if (chosenAbility == 5)
        {
            abilityPrefab = abilityPrefabs[5];
        }
        */
        // Select a random ability prefab
        Debug.Log(chosenAbility);

        GameObject abilityPrefab;
        if (chosenAbility == -1)
        {
            abilityPrefab = abilityPrefabs[Random.Range(0, abilityPrefabs.Count)];
        }
        else if (chosenAbility >= 0 && chosenAbility < abilityPrefabs.Count)
        {
            abilityPrefab = abilityPrefabs[chosenAbility];
        }
        else
        {
            Debug.LogError("Chosen ability index is out of range!");
            return;
        }
        //GameObject abilityPrefab = abilityPrefabs[3]; // 0 = laser, 1 = ray, 2 = frag 3 = gatling gun, 4 = rc, 5 = shield
        GameObject abilityInstance = Instantiate(abilityPrefab, spawnLocation, Quaternion.identity);
        // Spremeni velikost abilityja (lak nastavlas v unityu)
        abilityInstance.transform.localScale = new Vector3(prefabSize, prefabSize, 1);
        abilityInstance.name = abilityPrefab.name;
        // Dodaj ability v listo aktivnih abilityjev v AbilityScript.cs
        abilityScript.newAbility(abilityInstance);
    }
}

