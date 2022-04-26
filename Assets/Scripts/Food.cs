using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Interactable
{
    public float shitAmount = 25;
    // Start is called before the first frame update
    public override void interact()
    {
        Debug.Log("Eating");
        isInteracting = true;
    }

    public void Update()
    {
        base.Update();
    }

    public void LateUpdate()
    {
        
        if (isFinished)
        {
            if (notfyWhenFinished != null)
            {
                notfyWhenFinished(shitAmount);
            }

            Destroy(gameObject);
        }

        notfyWhenFinished = null;
    }
}
