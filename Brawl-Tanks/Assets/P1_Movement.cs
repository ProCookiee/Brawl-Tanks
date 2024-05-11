using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1_Movement : MonoBehaviour
{
    public AbilityScript AbilityScr;


    public GameManager.PlayerID playerID; // Assign PlayerID in the inspector
    
    private float speed = 5f;
    private float rotationSpeed = 1f;
    public Rigidbody2D rb;
    
    public GameObject bulletPrefab;
    public Transform firePoint;

    public bool canShoot = true;
    private int bulletCount;

    void Start()
    {
        firePoint = transform.Find("Turret");
        bulletCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
    bool isMoving = false;

    if (Input.GetKey(KeyCode.W))
    {
        rb.MovePosition(rb.position + (speed / 100 * (Vector2)transform.up));
        isMoving = true;
    }
    else if (Input.GetKey(KeyCode.S))
    {
        rb.MovePosition(rb.position - (speed / 100 * (Vector2)transform.up));
        isMoving = true;
    }

    if (Input.GetKey(KeyCode.A))
    {
        rb.MoveRotation(rb.rotation + 5 * rotationSpeed);
        isMoving = true;
    }
    else if (Input.GetKey(KeyCode.D))
    {
        rb.MoveRotation(rb.rotation - 5 * rotationSpeed);
        isMoving = true;
    }

    if(Input.GetKey(KeyCode.Q) && canShoot){
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        canShoot = false;
        bulletCount++;
        Debug.Log(bulletCount);
        StartCoroutine(ShootCooldown());
    }

    if (!isMoving)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
    }

    IEnumerator ShootCooldown(){
        yield return new WaitForSeconds(0.2f);
        canShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Bullet"){
            Destroy(gameObject);
            // Access the GameManager instance and inform that this player is destroyed
            GameManager.instance.PlayerDestroyed(playerID);
        }
        if(other.gameObject.tag == "Ability"){
            Destroy(other.gameObject);
            AbilityScr.doSomething(gameObject, other.gameObject);
            // Access the GameManager instance and inform that this player is destroyed
        }
    }

}
