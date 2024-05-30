using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Runtime.CompilerServices;

public class AIRocketController : MonoBehaviour
{
    public GameObject target;
    private float nextWaypointDistance = 0.5f;
    private float thrustForce = 5.0f;
    private float maxSpeed = 10.0f;
    private float rotationSpeed = 400.0f;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    public playerMovement.PlayerID playerID;
    private float initialSpeed = 1.0f;
    public float creationTime;

    Seeker seeker;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        //prvi parameter je cas predn zacne, drugi je cas med ponovitvami
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        rb.velocity = transform.up * initialSpeed;
        creationTime = Time.time;

    }
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.transform.position, OnPathComplete);
        }
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Raketi dodam silo, dokler ne doseže največje hitrosti
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.up * thrustForce);
        }
        if (path == null)
        {
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        //Vector2 force = speed * Time.deltaTime * direction;

        //rb.AddForce(force);

        // Rotate the rocket to face the direction
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float angle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.deltaTime);
        rb.rotation = angle;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        float rotationOffset = 180;
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Zracunam smer od rakete do stene
            Vector2 directionToWall = ((Vector2)collision.contacts[0].point - (Vector2)transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToWall.y, directionToWall.x) * Mathf.Rad2Deg - 90f;

            targetAngle -= rotationOffset;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if(Time.time - creationTime > 0.02f){
                Destroy(gameObject);
            }
        }
    }
}
