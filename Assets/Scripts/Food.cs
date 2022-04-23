using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Interactable
{
    // Start is called before the first frame update
    public override void interact()
    {
        Debug.Log("Eating");
        isInteracting = true;
    }

    public void Update()
    {
        base.Update();
        if (isFinished)
        {
            Destroy(gameObject);
        }
    }
}
