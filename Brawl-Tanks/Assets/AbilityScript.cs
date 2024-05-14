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

    Prefabs prefabs;

    // Start is called before the first frame update
    void Start()
    {
        prefabs = GameObject.Find("GameManager").GetComponent<Prefabs>();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void doSomething(GameObject player, GameObject ability)
    {
        playerMovement playerMove = player.GetComponent<playerMovement>();

        string abilityType = "";
        if (ability.name == "power_ray")
        {
            abilityType = "ray";
        }
        else if (ability.name == "power_shield")
        {
            abilityType = "shield";
        }
        else if (ability.name == "power_frag")
        {
            abilityType = "frag";
        }
        else if (ability.name == "power_gatling")
        {
            abilityType = "gatling";
        }
        else if(ability.name == "power_rc"){
            abilityType = "rc";
        }
        else if(ability.name == "power_laser"){
            abilityType = "laser";
        }
        playerMove.abilities.Enqueue(abilityType);  // Add ability to the queue

        // Remove the ability from the scene and observable collection
        spawnedAbilities.Remove(ability);
        Destroy(ability);
    }

    public void newAbility(GameObject ability)
    {
        // Debug.Log("New Abilityyyys: " + ability.name);
        // Ta funkcija je klicana iz AbilitySpawning.cs, ko se spawnajo novi abilityji
        // Doda ability v seznam aktivnih abilityjev
        spawnedAbilities.Add(ability);
    }

    public void selectAbility(GameObject player, string ability)
    {
        if (ability == "shield")
        {
            shield(player);
        }
        else if (ability == "ray")
        {
            ray(player);
        }
        else if (ability == "frag")
        {
            frag(player);
        }
        else if (ability == "gatling")
        {
            gatling(player);
        }
        else if (ability == "laser")
        {
            //laser(player);
        }
        else if (ability == "rc")
        {
            //rc(player);
        }
    }

    public void ray(GameObject player)
    {
        playerMovement playerMovement = player.GetComponent<playerMovement>();
        playerMovement.currentAbility = "";
        playerMovement.canShoot = false;
        StartCoroutine(shotLaser(playerMovement));
    }

    IEnumerator shotLaser(playerMovement playerMovement)
    {
        playerMovement.canMove = false;
        yield return new WaitForSeconds(0.1f);
        var laserLine = Instantiate(prefabs.laserLine, playerMovement.firePoint.position, playerMovement.firePoint.rotation);
        yield return new WaitForSeconds(1);
        Destroy(laserLine);
        var laser = Instantiate(prefabs.laserPrefab, playerMovement.firePoint.position, playerMovement.firePoint.rotation);
        yield return new WaitForSeconds(1);
        Destroy(laser);
        playerMovement.canMove = true;
    }

    public void shield(GameObject player)
    {
        playerMovement playerMovement = player.GetComponent<playerMovement>();
        playerMovement.currentAbility = "";
        playerMovement.canShoot = false;
        var newShield = Instantiate(prefabs.shieldPrefab, player.transform.position, player.transform.rotation);
        newShield.name = player.name + "_Shield";
    }

    public void frag(GameObject player)
    {
        playerMovement playerMovement = player.GetComponent<playerMovement>();
        playerMovement.currentAbility = "";
        playerMovement.canShoot = false;
        var fragBomb = Instantiate(prefabs.bulletPrefab, playerMovement.firePoint.position, playerMovement.firePoint.rotation);
        fragBomb.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        StartCoroutine(oneSecondFragExplode(fragBomb));
    }

    IEnumerator oneSecondFragExplode(GameObject fragBomb)
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 18; i++)
        {
            var bullet = Instantiate(prefabs.fragmentPrefab, fragBomb.transform.position, Quaternion.identity);
            bullet.transform.Rotate(0, 0, 20 * i);
            bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.up * 10f, ForceMode2D.Impulse);
            bullet.name = "fragment";
        }
        Destroy(fragBomb);
    }

    public void gatling(GameObject player)
    {
        playerMovement playerMovement = player.GetComponent<playerMovement>();
        playerMovement.currentAbility = "";
        playerMovement.canShoot = false;
        StartCoroutine(gatlingShot(playerMovement));
    }

    IEnumerator gatlingShot(playerMovement playerMovement)
    {
        for (int i = 0; i < 10; i++)
        {
            var miniBullet = Instantiate(prefabs.bulletPrefab, playerMovement.firePoint.position, playerMovement.firePoint.rotation);
            float spread = UnityEngine.Random.Range(-10.0f, 10.0f);
            miniBullet.transform.Rotate(0, 0, spread);
            miniBullet.name = "miniBullet";
            miniBullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            yield return new WaitForSeconds(0.05f);
        }
    }
}