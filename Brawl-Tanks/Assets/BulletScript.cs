using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody2D rb;
    int hits = 0;
    int maxHits = 10;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * 4f;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += transform.up * 4f * Time.deltaTime;
        //if bullet goes off screen, destroy it
        if (transform.position.x > 10 || transform.position.x < -10 || transform.position.y > 10 || transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {

        if(other.gameObject.tag == "Player"){
            Destroy(gameObject);
        }

        else if (other.gameObject.tag == "Wall")
        {
            hits++;

            if (hits >= maxHits)
            {
                Destroy(gameObject);
            }
        }
    }
}
