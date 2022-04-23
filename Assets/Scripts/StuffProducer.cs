using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffProducer : MonoBehaviour
{
    public ProgressBar progressBar;
    public GameObject toGeneratePrefab;
    
    public float generationSpeed = 50.0f;
    public float currentProgress = 0.0f;
    public bool isActive = false;
    public bool isFinished = false;
    
    // Update is called once per frame
    public void Start()
    {
        progressBar.gameObject.SetActive(false);
    }
    void Update()
    {
        if (isActive)
        {
            progressBar.gameObject.SetActive(true);
            currentProgress += Time.deltaTime * generationSpeed;
            progressBar.progress = currentProgress;
            if (currentProgress >= 100)
            {
                isActive = false;
                progressBar.gameObject.SetActive(false);
                isFinished = true;
                GameObject generated = GameObject.Instantiate(toGeneratePrefab);
                generated.transform.position = this.transform.position;
            }
        }
    }

    public void reactivate()
    {
        isFinished = false;
    }
}
