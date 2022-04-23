using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
    public GameObject backgroundObject;
    public GameObject progressObject;

    [Range(0, 100)]
    public float progress = 0.0f;

    public void Update()
    {
        Vector3 newScale = backgroundObject.transform.localScale;
        newScale.x = backgroundObject.transform.localScale.x * (progress / 100);

        progressObject.transform.localScale = newScale;
        progressObject.transform.localPosition = new Vector3(-0.5f + 0.5f * (progress / 100), 0, 0);
    }
}
