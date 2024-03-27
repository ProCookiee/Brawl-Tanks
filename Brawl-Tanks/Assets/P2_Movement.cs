using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2_Movement : MonoBehaviour
{
    private float speed = 5f;
    private float rotationSpeed = 1f;
    public Rigidbody2D rb;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    bool isMoving = false;

    if (Input.GetKey(KeyCode.UpArrow))
    {
        rb.MovePosition(rb.position + (speed / 100 * (Vector2)transform.up));
        isMoving = true;
    }
    else if (Input.GetKey(KeyCode.DownArrow))
    {
        rb.MovePosition(rb.position - (speed / 100 * (Vector2)transform.up));
        isMoving = true;
    }

    if (Input.GetKey(KeyCode.LeftArrow))
    {
        rb.MoveRotation(rb.rotation + 5 * rotationSpeed);
        isMoving = true;
    }
    else if (Input.GetKey(KeyCode.RightArrow))
    {
        rb.MoveRotation(rb.rotation - 5 * rotationSpeed);
        isMoving = true;
    }

    if (!isMoving)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
    }
}
