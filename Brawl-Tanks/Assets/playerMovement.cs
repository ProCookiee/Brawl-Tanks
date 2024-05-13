using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public enum PlayerID { Player1, Player2 }
    public PlayerID playerID;
    
    private float speed = 5f;
    private float rotationSpeed = 1f;
    public Rigidbody2D rb;

    public GameObject bulletPrefab;

    public Transform firePoint;

    public bool canShoot = true;
    private int bulletCount;

    public string currentAbility = "";

    private KeyCode forwardKey, backwardKey, leftKey, rightKey, shootKey;

    void Start()
    {
        AssignControls();
        firePoint = transform.Find("Turret");
        bulletCount = 0;
    }

    void AssignControls()
    {
        if (playerID == PlayerID.Player1)
        {
            forwardKey = KeyCode.W;
            backwardKey = KeyCode.S;
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            shootKey = KeyCode.Q;
        }
        else if (playerID == PlayerID.Player2)
        {
            forwardKey = KeyCode.UpArrow;
            backwardKey = KeyCode.DownArrow;
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            shootKey = KeyCode.Space;
        }
    }

    void Update()
    {
        bool isMoving = false;

        

        if (Input.GetKey(forwardKey))
        {
            rb.MovePosition(rb.position + (speed / 100 * (Vector2)transform.up));
            isMoving = true;
        }
        else if (Input.GetKey(backwardKey))
        {
            rb.MovePosition(rb.position - (speed / 100 * (Vector2)transform.up));
            isMoving = true;
        }

        if (Input.GetKey(leftKey))
        {
            rb.MoveRotation(rb.rotation + 5 * rotationSpeed);
            isMoving = false;
        }
        else if (Input.GetKey(rightKey))
        {
            rb.MoveRotation(rb.rotation - 5 * rotationSpeed);
            isMoving = false;
        }

        if (Input.GetKey(shootKey) && canShoot)
        {
            Debug.Log(currentAbility);
            if (currentAbility == "")
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                canShoot = false;
                bulletCount++;
                Debug.Log(bulletCount);
                StartCoroutine(ShootCooldown());
            }
            else
            {
                AbilityScript abilityScript = GameManager.instance.GetComponent<AbilityScript>();
                abilityScript.selectAbility(gameObject, currentAbility);
                StartCoroutine(ShootCooldown());
            }
        }

        if (!isMoving)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        canShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
            GameManager.instance.PlayerDestroyed((GameManager.PlayerID)playerID);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ability")
        {
            AbilityScript abilityScript = GameManager.instance.GetComponent<AbilityScript>();
            abilityScript.doSomething(gameObject, other.gameObject);
        }
    }
}
