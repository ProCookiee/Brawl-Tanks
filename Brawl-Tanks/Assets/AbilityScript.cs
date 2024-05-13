using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

public class AbilityScript : MonoBehaviour
{
    //vsi aktivni abilityji kot observable collection da lak v ability spawningu spremljamo spremembe
    public ObservableCollection<GameObject> spawnedAbilities = new();
    playerMovement playerMovement;


    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void doSomething(GameObject player, GameObject ability)
    {
        playerMovement = player.GetComponent<playerMovement>();

        if(ability.name == "power_ray(Clone)"){
            playerMovement.currentAbility = "ray";
        }
        else if(ability.name == "power_shield(Clone)"){
            playerMovement.currentAbility = "shield";
        }        
        //ability je bil uničen zato ga odstrani iz seznama aktivnih abilityjev
        spawnedAbilities.Remove(ability);
        Destroy(ability);
    }

    public void newAbility(GameObject ability){
       // Debug.Log("New Abilityyyys: " + ability.name);
       // Ta funkcija je klicana iz AbilitySpawning.cs, ko se spawnajo novi abilityji
       // Doda ability v seznam aktivnih abilityjev
        spawnedAbilities.Add(ability);
    }

    public void selectAbility(GameObject player, string ability){
        if(ability == "shield"){
            //shield(player, playerID);
        }
        else if(ability == "ray"){
            ray(player);
        }
    }

    public void ray(GameObject player){
        playerMovement playerMovement = player.GetComponent<playerMovement>();
        playerMovement.currentAbility = "";
        playerMovement.canShoot = false;
        Instantiate(playerMovement.laserPrefab, playerMovement.firePoint.position, playerMovement.firePoint.rotation);
    }
}
