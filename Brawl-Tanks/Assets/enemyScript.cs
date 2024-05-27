using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour
{
    private float speed = 1f; // Speed at which the enemy moves
    private Transform player; // Reference to the player
    private float enemyHP = 1;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        if(name == "Enemy1"){
            enemyHP = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector2 direction = player.position;
            transform.position = Vector2.MoveTowards(transform.position, direction, speed * Time.deltaTime);
            // Rotate the enemy to face the player
            Vector3 lookDirection = player.position - transform.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            gameManager.playerHP--;
            Destroy(gameObject);
        }
        if(other.gameObject.tag == "Bullet") {
            enemyHP--;
            Destroy(other.gameObject);
            if(enemyHP <= 0){
                gameManager.score++;
                Destroy(gameObject);
                
            }
        }
    }
}
