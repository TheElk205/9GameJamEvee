using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public float radius = 1.0f;
    public LayerMask waypointLayer;
    public LayerMask obstructionLayer;
    
    public List<GameObject> neighbourWaypoints;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(fovCheck());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private IEnumerator fovCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            fov();
        }
    }
    
    private void fov()
    {
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, radius, waypointLayer);

        neighbourWaypoints.Clear();
        Debug.Log($"Found {rangeCheck.Length} neighbours");
        
        if (rangeCheck.Length > 0)
        {
            foreach (var waypoint in rangeCheck)
            {
                Transform target = waypoint.transform;
                Vector2 directionToTarget = (target.position - transform.position).normalized;

                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                // Check if it hist an obstacle
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
                {
                    // Waypoint found. Add to neighbours
                    neighbourWaypoints.Add(target.gameObject);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var waypoint in neighbourWaypoints)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, waypoint.transform.position);
        }
    }
}
