using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour
{
    private float speed = 1f; // Speed at which the enemy moves
    private Transform player; // Reference to the player
    private float enemyHP = 1;

    private GameManager gameManager;
    private int enemyType;

    private Transform firePoint;
    private bool canShoot = true;

    Prefabs prefabs;

    // Start is called before the first frame update
    void Start()
    {
        prefabs = GameObject.Find("GameManager").GetComponent<Prefabs>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        firePoint = transform.Find("Turret");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyType = getEnemyType();
        if(enemyType == 1 || enemyType == 2){
            enemyHP = 1;
        }
        else{
            enemyHP = 0;
        }
    }

    int getEnemyType(){
        if(name == "Enemy1"){
            return 1;
        }
        else if(name == "Enemy2"){
            return 2;
        }
        else{
            return 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if(enemyType == 1){
                followPlayer();
            }
            else if(enemyType == 2){
                //če je v blizini playerja se ustavi
                if(Vector2.Distance(transform.position, player.position) > 4){
                    followPlayer();
                }
                else{
                    rotateTowardsPlayer();
                    if(canShoot){
                        canShoot = false;
                        shoot();
                        StartCoroutine(shootCooldown());
                    }
                }
            }
            else{
                //transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
        }
    }

    void followPlayer(){
        Vector2 direction = player.position;
        transform.position = Vector2.MoveTowards(transform.position, direction, speed * Time.deltaTime);
        // Rotate the enemy to face the player
        rotateTowardsPlayer();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    void rotateTowardsPlayer(){
        Vector3 lookDirection = player.position - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    void shoot(){
        var bullet = Instantiate(prefabs.bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.name = "enemy_bullet";
        Debug.Log("shoot");
    }

    IEnumerator shootCooldown(){
        yield return new WaitForSeconds(1f);
        canShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            gameManager.playerHP--;
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Bullet")
        {
            BulletScript bullet = other.gameObject.GetComponent<BulletScript>();
            Debug.Log(bullet.creationTime);
            Debug.Log(Time.time);
            Debug.Log(Time.time - bullet.creationTime);
            if (Time.time - bullet.creationTime > 0.02f)
            {
                Destroy(other.gameObject);
                if(enemyHP <= 0){
                    Destroy(gameObject);
                }
                else{
                    enemyHP--;
                }
            }
        }
    }
}
