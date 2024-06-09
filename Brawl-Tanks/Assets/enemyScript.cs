using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyScript : MonoBehaviour
{
    private float speed = 1f; // Speed at which the enemy moves
    private Transform player; // Reference to the player
    private float enemyHP = 1;
    private float maxHP;

    private GameManager gameManager;
    private int enemyType;

    private Transform firePoint;
    private bool canShoot = true;

    Prefabs prefabs;

    private Transform healthBar;
    private Image healthBarForeground;

    private GameObject currentLaser;
    private GameObject currentLaserLine;

    private Rigidbody2D rb;

    GameState GameState;

    // Start is called before the first frame update
    void Start()
    {
        prefabs = GameObject.Find("GameManager").GetComponent<Prefabs>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        firePoint = transform.Find("Turret");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyType = getEnemyType();
        if (enemyType == 1)
        {
            enemyHP = 2;
        }
        else if (enemyType == 2)
        {
            enemyHP = 3;
        }
        else if (enemyType == 3)
        {
            enemyHP = 3;
        }
        else
        {
            enemyHP = 0;
        }
        maxHP = enemyHP;

        // Initialize health bar
        healthBar = transform.Find("HealthBar");
        if (healthBar != null)
        {
            healthBarForeground = healthBar.Find("currentHP").GetComponent<Image>();
        }

        rb = GetComponent<Rigidbody2D>();
    }

    int getEnemyType()
    {
        if (name == "Enemy1")
        {
            return 1;
        }
        else if (name == "Enemy2")
        {
            return 2;
        }
        else if (name == "Enemy3")
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (enemyType == 1)
            {
                followPlayer();
            }
            else if (enemyType == 2)
            {
                if (Vector2.Distance(transform.position, player.position) > 4)
                {
                    followPlayer();
                }
                else
                {
                    rotateTowardsPlayer();
                    if (canShoot)
                    {
                        canShoot = false;
                        shoot();
                        StartCoroutine(shootCooldown());
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        rb.angularVelocity = 0f;
                    }
                }
            }
            else if (enemyType == 3)
            {
                if (Vector2.Distance(transform.position, player.position) > 6 && canShoot)
                {
                    followPlayer();
                }
                else
                {
                    if (canShoot)
                    {
                        rotateTowardsPlayer();
                        canShoot = false;
                        StartCoroutine(shotLaser());
                        StartCoroutine(laserCooldown());
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        rb.angularVelocity = 0f;
                    }
                }
            }
            else
            {
                //transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
        }
    }

    void followPlayer()
    {
        Vector2 direction = player.position;
        transform.position = Vector2.MoveTowards(transform.position, direction, speed * Time.deltaTime);
        rotateTowardsPlayer();
    }

    void rotateTowardsPlayer()
    {
        Vector3 lookDirection = player.position - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    void shoot()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        var bullet = Instantiate(prefabs.bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.name = "enemy_bullet";
        Debug.Log("shoot");
    }

    IEnumerator shotLaser()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        yield return new WaitForSeconds(0.1f);
        currentLaserLine = Instantiate(prefabs.laserLine, firePoint.position, firePoint.rotation);
        yield return new WaitForSeconds(2);
        Destroy(currentLaserLine);
        currentLaser = Instantiate(prefabs.laserPrefab, firePoint.position, firePoint.rotation);
        yield return new WaitForSeconds(1);
        Destroy(currentLaser);
        currentLaser = null;
    }

    IEnumerator shootCooldown()
    {
        yield return new WaitForSeconds(1f);
        canShoot = true;
    }

    IEnumerator laserCooldown()
    {
        yield return new WaitForSeconds(3f);
        canShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            GameState.playerHP--;
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
                enemyHP--;
                Destroy(other.gameObject);
                if (enemyHP <= 0)
                {
                    CleanupBeforeDeath();
                    giveScore(name);
                    Destroy(gameObject);
                    GameState.survivalScore++;
                }
                else
                {  
                    UpdateHealthBar();
                }
            }
        }
    }

    private void giveScore(string enemyName)
    {
        if (enemyName == "Enemy1")
        {
            GameState.survivalScore += 1;
        }
        else if (enemyName == "Enemy2")
        {
            GameState.survivalScore += 3;
        }
        else if (enemyName == "Enemy3")
        {
            GameState.survivalScore += 5;
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarForeground != null)
        {
            healthBarForeground.fillAmount = enemyHP / maxHP;
        }
    }

    void CleanupBeforeDeath()
    {
        if (currentLaser != null)
        {
            Destroy(currentLaser);
        }
        if (currentLaserLine != null)
        {
            Destroy(currentLaserLine);
        }
    }
}
