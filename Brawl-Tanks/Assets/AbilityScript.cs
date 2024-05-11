using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
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

        if(ability.name == "power_laser(Clone)")
        {
            Debug.Log("Laser");
            if(player.name == "P1_Tank(Clone)")
            {
                p1.currentAbility = "laser";
            }
            else if(player.name == "P2_Tank(Clone)")
            {
                p2.currentAbility = "laser";
            }
        }
        else if(ability.name == "deathRay")
        {
            //deathRay(player, ability);
        }
        Debug.Log("Player: " + player.name + "Ability" + ability.name);
        Destroy(ability);
    }

}
