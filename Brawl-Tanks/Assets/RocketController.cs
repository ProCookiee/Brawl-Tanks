using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RocketController : MonoBehaviour
{
    public ParticleSystem bulletExplosion;
    public AudioSource bulletBounceSound;
    private float rotationSpeed = 350.0f;
    private float thrustForce = 5.0f;
    private float maxSpeed = 5.0f;
    private float drag = 1.0f;
    private float initialSpeed = 1.0f;
    //private float bounceBackDistance = 0.2f;
    private float rotationOffset = 180f;
    public playerMovement.PlayerID playerID;
    public Rigidbody2D rigidBody2;
    private KeyCode leftKey, rightKey, shootKey;
    public float creationTime;
    

    public event Action<RocketController> OnRocketDestroyed;

    void Start()
    {
        AssignControls();
        rigidBody2.drag = drag;
        rigidBody2.velocity = transform.up * initialSpeed;
        creationTime = Time.time;
    }
    void AssignControls()
    {
        if (playerID == playerMovement.PlayerID.Player1)
        {
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            shootKey = KeyCode.Q;
        }
        else if (playerID == playerMovement.PlayerID.Player2)
        {
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            shootKey = KeyCode.Space;
        }
    }

    void Update()
    {
        // Dobim input za rotacijo
        float rotationInput = (Input.GetKey(rightKey) ? 1 : 0) - (Input.GetKey(leftKey) ? 1 : 0);

        // Dokler je pritisnjena tipka za rotacijo, rotiram raketo
        if (rotationInput != 0)
        {
            rigidBody2.angularVelocity = -rotationInput * rotationSpeed;
        }
        else
        {
            rigidBody2.angularVelocity = 0;
        }

        if(Input.GetKey(shootKey) && Time.time - creationTime > 1f){
            Destroy(gameObject);
            OnRocketDestroyed?.Invoke(this);
        }

    }

    void FixedUpdate()
    {
        // Raketi dodam silo, dokler ne doseže največje hitrosti
        if (rigidBody2.velocity.magnitude < maxSpeed)
        {
            rigidBody2.AddForce(transform.up * thrustForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            touchedWallEffect();
            // Zracunam smer od rakete do stene
            Vector2 directionToWall = ((Vector2)collision.contacts[0].point - (Vector2)transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToWall.y, directionToWall.x) * Mathf.Rad2Deg - 90f;

            // Rotiram raketo za 180 stopinj
            targetAngle += rotationOffset;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if(Time.time - creationTime > 0.02f){
                Destroy(gameObject);
                OnRocketDestroyed?.Invoke(this);
            }
        }
    }

    void touchedWallEffect(){
        bulletBounceSound.Play();
        ParticleSystem explosionEffect = Instantiate(bulletExplosion, transform.position, Quaternion.identity);
        explosionEffect.Play();
        Destroy(explosionEffect.gameObject, explosionEffect.main.duration);
    }
}