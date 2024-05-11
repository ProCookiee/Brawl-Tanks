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
        Destroy(ability);
        if(ability.name == "laser")
        {
            laser(player);
        }
        else if(ability.name == "deathRay")
        {
            //deathRay(player, ability);
        }
    }

    public void laser(GameObject player)
    {
        p1.firePoint = player.transform.Find("Turret");
    }
}
