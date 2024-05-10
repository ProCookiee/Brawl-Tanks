using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame updat
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * 4f * Time.deltaTime;
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

        if(other.gameObject.tag == "Wall"){
            Vector2 normal = other.contacts[0].normal;
            Vector2 direction = Vector2.Reflect(transform.up, normal);
            transform.up = direction;
        }
    }
}
