using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Hole : MonoBehaviour, Interactable
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
        if (isBig != isBigLast)
        {
            holeBig.SetActive(isBig);
            holeSmall.SetActive(!isBig);
            isBigLast = isBig;
        }
    }

    public void interact()
    {
        isBig = true;
    }
}
