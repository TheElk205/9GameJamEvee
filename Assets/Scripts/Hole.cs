using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Hole : Interactable
{
    public GameObject holeSmall;
    public GameObject holeBig;

    private bool isBigLast = false;

    public bool isBig = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isBig = isFinished;
        if (isBig != isBigLast)
        {
            holeBig.SetActive(isBig);
            holeSmall.SetActive(!isBig);
            isBigLast = isBig;
        }
        base.Update();
    }
    
    public override void interact()
    {
        Debug.Log("Interacting");
        isInteracting = true;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"Something went in: {col.gameObject.name}");
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (isBig && other.tag.Equals("Collectible") && other.transform.parent == null)
        {
            Debug.Log("Scored!");
            GameObject.Destroy(other.gameObject);
        }
    }
}
