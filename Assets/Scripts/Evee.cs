using System;
using UnityEngine;

public enum CollectibleState
{
    IDLE,
    CAN_COLLECT,
    IS_HOLDING
}

public class Evee : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;

    public float runSpeed = 20.0f;
    public CollectibleState collectibleState = CollectibleState.IDLE;
    public GameObject canCollectState;

    public GameObject toCollect;
    
    private float angle = 0;
    void Start ()
    {
        body = GetComponent<Rigidbody2D>(); 
        canCollectState.SetActive(false);
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

        if (collectibleState == CollectibleState.CAN_COLLECT && Input.GetKeyDown(KeyCode.Space))
        {
            collectibleState = CollectibleState.IS_HOLDING;
            toCollect.transform.parent = transform;
        }
        else if (collectibleState == CollectibleState.IS_HOLDING && Input.GetKeyDown(KeyCode.Space))
        {
            collectibleState = CollectibleState.IDLE;
            toCollect.transform.parent = null;
        }
    }

    private void FixedUpdate()
    {  
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        
        // Rotate image
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Collectible"))
        {
            collectibleState = CollectibleState.CAN_COLLECT;
            canCollectState.SetActive(true);
            toCollect = col.transform.gameObject;
        }
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Collectible") && collectibleState == CollectibleState.CAN_COLLECT)
        {
            collectibleState = CollectibleState.IDLE;
            canCollectState.SetActive(false);
            toCollect = null;
        }
    }
}
