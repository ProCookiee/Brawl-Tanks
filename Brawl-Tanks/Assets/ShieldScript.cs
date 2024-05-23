using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    playerMovement playerMove;
    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.name == "P1_Tank_Shield"){
            playerMove = GameObject.Find("P1_Tank").GetComponent<playerMovement>();
        }
        else if(gameObject.name == "P2_Tank_Shield"){
            playerMove = GameObject.Find("P2_Tank").GetComponent<playerMovement>();
        }
        playerMove.shieldActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(gameObject.name == "P1_Tank_Shield"){
            transform.position = GameObject.Find("P1_Tank").transform.position;
        }
        else if(gameObject.name == "P2_Tank_Shield"){
            transform.position = GameObject.Find("P2_Tank").transform.position;
        }

        StartCoroutine(TenSecondTimer());
    }

    IEnumerator TenSecondTimer()
    {
        yield return new WaitForSeconds(10);
        playerMove.shieldActive = false;
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Bullet") {
            BulletScript bullet = other.gameObject.GetComponent<BulletScript>();
            if (bullet != null && Time.time - bullet.creationTime > 0.1f)
            {
                playerMove.shieldActive = false;
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }
        else if(other.gameObject.tag == "rcRocket"){
            RocketController rocketController = other.gameObject.GetComponent<RocketController>();
            if (rocketController != null && Time.time - rocketController.creationTime > 0.1f)
            {
                playerMove.shieldActive = false;
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }
        else if(other.gameObject.tag == "DeathRay"){
            playerMove.shieldActive = false;
            Destroy(gameObject);
        }
    }
}
