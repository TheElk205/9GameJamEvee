using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackout : MonoBehaviour
{
    public GameObject[] blackouts;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var blackout in blackouts)
        {
            blackout.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Evee"))
        {
            foreach (var blackout in blackouts)
            {
                blackout.SetActive(true);
            }
        }
    }
    
    public void OnTriggerExit2D(Collider2D col)
    {
        foreach (var blackout in blackouts)
        {
            blackout.SetActive(false);
        }
    }
}
