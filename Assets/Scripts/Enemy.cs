using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using Random = System.Random;

public class Enemy : MonoBehaviour
{
    [Range(0, 10)]
    public float radius = 5;
    [Range(1, 360)]
    public float angle = 45;

    public float walkingSpeed = 1.0f;
    public float rotationSpeed = 0.05f;

    // Wait for n seconds until we give up and walk to nearest waypint again
    public float waitForSecondsGiveUp = 2.0f;
    
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;
    public LayerMask waypointLayer;

    public Evee playerRef;
    public float playerLastSeen = 0.0f;
    
    public bool canSeePlayer { get; private set; }

    public GameObject currentFocus;
    public GameObject comingFrom;
    public bool walkTowardsWaypoint = false;
    
    public GameObject renderWhenEveeInSight;
    public Rigidbody2D body;
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        
        StartCoroutine(fovCheck());
        renderWhenEveeInSight.SetActive(false);
        findNearestWaypoint();
        walkTowardsWaypoint = true;
    }

    void Update()
    {
        // How will this actor act:
        // Starting with walking to nearest waypoint that is not obstructed
        // If waypoint hit, check if it has an interaction for the enemy
        // If yes, check percentage and execute action or move on
        // If not, move on
        // Move on: Neighbouring waypoints have different possibilities according to a set value.
        // Going back always has the lowest possibility.
        // Walk towards next waypoint
        // Repeat

        if (walkTowardsWaypoint)
        {
            body.velocity = (currentFocus.transform.position - transform.position).normalized * walkingSpeed;
            Vector2 v = body.velocity;
            var rotationAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg - 90.0f; 
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(rotationAngle, Vector3.forward), rotationSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit something");
        Waypoint wp = collision.GetComponent<Waypoint>();
        if (wp == walkTowardsWaypoint)
        {
            Debug.Log("We found a waypoint!");
            // walkTowardsWaypoint = false;
            // body.velocity = Vector2.zero;
            
            Random r = new Random();
            if (wp.neighbourWaypoints.Count == 0)
            {
                Debug.LogError("Waypoint ahs no neighbours.. what the heck");
            }
            // FInd next waypoint
            else
            {
                float[] distances = new float[wp.neighbourWaypoints.Count];
                
                for(int i = 0; i < wp.neighbourWaypoints.Count; i++)
                {
                    if (wp.neighbourWaypoints[i] == comingFrom)
                    {
                        Debug.Log("Setting coming from to -1 to repalce later");
                        distances[i] = -1.0f;
                    }
                    else
                    {
                        distances[i] = Vector2.Distance(transform.position,
                            wp.neighbourWaypoints[i].transform.position);
                    }
                }
                
                float max = distances.Max();
                int lastIndex = distances.ToList().IndexOf(-1);
                if (lastIndex != -1)
                {
                    Debug.Log("Replacing last index");
                    distances[lastIndex] = max + max * 0.5f;
                }

                // Now we have weighted distances, where the last waypoint is always the highest value
                // We now sort their inverse now in ascending order
                List<Tuple<int, float>> tuples = new List<Tuple<int, float>>();
                float sum = distances.Sum();
                float sumInverted = 0.0f;
                for (int i = 0; i < distances.Length; i++)
                {
                    sumInverted += (sum - distances[i]);
                    tuples.Add(new Tuple<int, float>(i, sum - distances[i]));
                }
                
                tuples.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                // Now we calculate a cdf
                float[] cdf = new float[tuples.Count];
                cdf[0] = tuples[0].Item2;
                for (int i = 1; i < tuples.Count; i++)
                {
                    cdf[i] = cdf[i - 1] + tuples[i].Item2;
                }
                
                // I have a value now between 0 and sum
                float random = (float)(r.NextDouble() * cdf.Sum());
                for (int i = 0; i < wp.neighbourWaypoints.Count; i++)
                {
                    random -= cdf[i];
                    if (random <= 0)
                    {
                        this.comingFrom = this.currentFocus;
                        Debug.Log($"Selected index: {tuples[i].Item1}");
                        this.currentFocus = wp.neighbourWaypoints[tuples[i].Item1];
                        break;
                    }
                }
            }
        }
    }
    
    private void findNearestWaypoint()
    {
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, 20, waypointLayer);

        if (rangeCheck.Length == 0)
        {
            Debug.LogError("No waypoint found. We should start working forward now. But this should never happen");
        }

        float shortestDistance = float.MaxValue;
        Waypoint nextWaypoint = null;
        foreach (var hit in rangeCheck)
        {
            Vector2 directionToTarget = (hit.transform.position - transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, hit.transform.position);
            // Is Waypoint behind an obstacle?
            if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
            {
                if (distanceToTarget < shortestDistance)
                {
                    nextWaypoint = hit.GetComponent<Waypoint>();
                    shortestDistance = distanceToTarget;
                }
            }
        }

        this.comingFrom = this.currentFocus;
        this.currentFocus = nextWaypoint.gameObject;
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
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        canSeePlayer = false;
        
        if (rangeCheck.Length > 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            // Check vision triangle
            if (Vector2.Angle(transform.up, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                // Check if it has NOT hit an obstacle
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
                {
                    canSeePlayer = true;
                    playerLastSeen = Time.time;
                }
            }
        }

        renderWhenEveeInSight.SetActive(canSeePlayer);
        if (canSeePlayer && playerRef.mode == EveeMode.MISCHIEF)
        {
            currentFocus = playerRef.gameObject;
        }

        // Releasing current target if player is out of sight
        // and has not been seen for a few seconds
        if (!canSeePlayer && currentFocus == playerRef.gameObject && waitForSecondsGiveUp + playerLastSeen < Time.time)
        {
            currentFocus = null;
            findNearestWaypoint();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius);
        
        Vector3 angle0 = directionFromAngle(-transform.eulerAngles.z, -angle / 2);
        Vector3 angle1 = directionFromAngle(-transform.eulerAngles.z, angle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + angle0 * radius);
        float numberOfLines = 25.0f;
        for (int i = 1; i < numberOfLines; i++)
        {
            Gizmos.color = Color.red;
            Vector3 anglei = directionFromAngle(-transform.eulerAngles.z, -angle / 2 + (i / numberOfLines) * angle);
            var hit = Physics2D.Raycast(transform.position, anglei, radius, obstructionLayer);
            Vector3 endPosition = transform.position + anglei * radius;
            if (hit)
            {
                endPosition = transform.position + anglei * hit.distance;
            }
            Gizmos.DrawLine(transform.position, endPosition);
        }
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + angle1 * radius);

        if (canSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, playerRef.transform.position);
        }
    }

    private Vector2 directionFromAngle(float eulerYValue, float angleInDegrees)
    {
        angleInDegrees += eulerYValue;

        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
