using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float interactionSpeed = 10.0f;
    public float currentProgress = 0;
    public float resetAfterSeconds = -1;
    public ProgressBar progressBar;
    
    public abstract void interact();
    
    public bool isInteracting = false;
    public bool isFinished = false;
    public void Update()
    {
        if (!isFinished && isInteracting)
        {
            Debug.Log("Updating current progress");
            currentProgress += Time.deltaTime * interactionSpeed;
            isInteracting = false;
            if (progressBar != null)
            {
                progressBar.progress = currentProgress;
            }
            if (currentProgress > 100)
            {
                isFinished = true;
            }
        }
    }
}
