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
    P1_Movement p1;
    P2_Movement p2;


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
        p1 = player.GetComponent<P1_Movement>();
        p2 = player.GetComponent<P2_Movement>();

        if(ability.name == "power_ray(Clone)")
        {
            if(player.name == "P1_Tank(Clone)")
            {
                p1.currentAbility = "ray";
            }
            else if(player.name == "P2_Tank(Clone)")
            {
                p2.currentAbility = "ray";
            }
        }
        else if(ability.name == "laser")
        {
            //deathRay(player, ability);
        }
        //Debug.Log("Player: " + player.name + "Ability" + ability.name);
        
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



}
