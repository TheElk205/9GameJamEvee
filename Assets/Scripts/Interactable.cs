using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float interactionSpeed = 10.0f;
    [Range(-1, 100)]
    public float currentProgress = 0.0f;
    public float resetAfterSeconds = -1;
    public ProgressBar progressBar;

    public Sprite actionIcon;
    public abstract void interact();
    
    public bool isInteracting = false;
    public bool isFinished = false;

    public IsFinished notfyWhenFinished;
    
    public void Start()
    {
        progressBar.gameObject.SetActive(false);
    }
    public void Update()
    {
        progressBar.gameObject.SetActive(currentProgress>0.0f);
        if (!isFinished && isInteracting)
        {
            Debug.Log("Updating current progress");
            currentProgress += Time.deltaTime * interactionSpeed;
            isInteracting = false;
        }
        if (currentProgress >= 100)
        {
            isFinished = true;
        }
        else if (currentProgress < 100)
        {
            isFinished = false;
        }
        if (progressBar != null)
        {
            progressBar.progress = currentProgress;
        }
        progressBar.gameObject.SetActive(!(isFinished || currentProgress <= 0.0f));
    }
    public delegate void IsFinished(float amount);
}
