using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyAiType
{
    Idle,
    Lazy,
    Alert,
    Patrol,
    MAX_ENEMY_AI_TYPE
}

public class EnemyAiBase : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private EnemyAiType aiType;
    [SerializeField]
    private Vector3 home;
    [SerializeField]
    private LayerMask playerLayerMask;
    [SerializeField]
    private float maxViewDistance = 20f;
    [SerializeField]
    private Transform[] waypoints;
    [SerializeField]
    private float waypointWaitTimeSec = 3;
    [SerializeField]
    private int currWaypointIndex;
    private bool isIncreasingIndex = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (home == Vector3.zero)
        {
            //make it here
            home = transform.position;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectsWithTag("Player").FirstOrDefault();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (aiType)
        {
            case EnemyAiType.Idle:
                // do nothing
                break;
            case EnemyAiType.Lazy:
                // Don't move, just shoot at player.
                break;
            case EnemyAiType.Alert:
                // Stay stationary, and move towards player if you see them.
                if (CanSeePlayer())
                {
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    // go home
                    if (Vector3.Distance(transform.position, home) > 1)
                    {
                        agent.SetDestination(home);
                    }
                }
                break;
            case EnemyAiType.Patrol:
                // Use the predefined list of waypoints to move betwen, and if you see player, move towards them.
                if (CanSeePlayer())
                {
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    Vector3 currWaypoint = waypoints[currWaypointIndex].position;
                    // normalize distance in 2d only
                    currWaypoint = new Vector3(currWaypoint.x, transform.position.y, currWaypoint.z);
                    if ( Vector3.Distance(transform.position, currWaypoint) > 0.5f)
                    {
                        agent.SetDestination(currWaypoint);
                    }
                    else
                    {
                        // delay starting a new point to give it the illusion of thinking
                        if (!isIncreasingIndex)
                        {
                            StartCoroutine(DelayIndexIncrease(waypointWaitTimeSec));
                        }
                    }
                }
                break;
        }
    }

    private bool CanSeePlayer()
    {
        if(player == null)
        {
            return false;
        }
        // draw raycast to see if you hit the player
        Vector3 playerDirection = player.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDirection, out hit, Mathf.Infinity, playerLayerMask))
        {
            if (hit.collider.gameObject.name == "Player" && (hit.point - transform.position).magnitude < maxViewDistance)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (player != null)
        {
            Vector3 playerDirection = player.transform.position - transform.position;
            Gizmos.DrawRay(transform.position, playerDirection);
        }
    }

    private IEnumerator DelayIndexIncrease(float seconds)
    {
        isIncreasingIndex = true;
        yield return new WaitForSeconds(seconds);
        currWaypointIndex = (currWaypointIndex + 1) % waypoints.Length;
        isIncreasingIndex = false;
    }
}
