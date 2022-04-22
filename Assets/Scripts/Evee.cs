using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evee : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;

    public float runSpeed = 20.0f;

    private float angle = 0;
    void Start ()
    {
        body = GetComponent<Rigidbody2D>(); 
    }

    void Update ()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            Vector2 v = body.velocity;
            angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg - 90.0f; 
        }
    }

    private void FixedUpdate()
    {  
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        
        // Rotate image
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
