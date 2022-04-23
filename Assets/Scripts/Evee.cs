using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum CollectibleState
{
    IDLE,
    CAN_COLLECT,
    IS_HOLDING
}

public enum InteractionState
{
    IDLE,
    CAN_INTERACT,
    IS_INTERACTING
}

public enum EveeMode
{
    NORMAL,
    MISCHIEF
}

public class Evee : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;

    public float runSpeed = 20.0f;
    public CollectibleState collectibleState = CollectibleState.IDLE;
    [FormerlySerializedAs("interactioNState")] public InteractionState interactionState = InteractionState.IDLE;
    
    public GameObject canCollectState;

    public GameObject toCollect;
    public GameObject interactWith;

    public Transform collectionGrabPoint;
    public EveeMode mode = EveeMode.NORMAL;
    
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
            toCollect.transform.parent = collectionGrabPoint;
            toCollect.transform.localPosition = Vector3.zero;
            Collectible cb = toCollect.GetComponent<Collectible>();
            if (cb != null && !cb.isAllowed)
            {
                mode = EveeMode.MISCHIEF;
            }
        }
        else if (collectibleState == CollectibleState.IS_HOLDING && Input.GetKeyDown(KeyCode.Space))
        {
            collectibleState = CollectibleState.IDLE;
            toCollect.transform.parent = null;
            mode = EveeMode.NORMAL;
        }
        else if (interactionState == InteractionState.CAN_INTERACT && Input.GetKeyDown(KeyCode.Space))
        {
            Interactable it = interactWith.GetComponentInParent<Interactable>();
            Debug.Log("Interacting");
            if (it != null)
            {
                it.interact();
            }
            else
            {
                Debug.Log("Nothing to found to interact with");
            }
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
        if (col.gameObject.CompareTag("Collectible") && collectibleState == CollectibleState.IDLE)
        {
            collectibleState = CollectibleState.CAN_COLLECT;
            canCollectState.SetActive(true);
            toCollect = col.gameObject;
        }
        // We can only interact if we are not holding anything
        else if (col.gameObject.CompareTag("Interactible") && 
                 interactionState == InteractionState.IDLE && 
                 collectibleState != CollectibleState.IS_HOLDING)
        {
            interactionState = InteractionState.CAN_INTERACT;
            interactWith = col.gameObject;
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
        else if (col.gameObject.CompareTag("Interactible") && interactionState == InteractionState.CAN_INTERACT)
        {
            interactionState = InteractionState.IDLE;
            interactWith = null;
        }
    }
}
