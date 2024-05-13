using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Bullet"){
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
