using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyAiType
{
    Idle,
    Lazy,
    Attack,
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
    private float waitTimeSec = 3;
    [SerializeField]
    private int currWaypointIndex;
    private bool isIncreasingIndex = false;
    [SerializeField]
    private bool canSeeBullet = false;
    [SerializeField]
    private bool isAlerted = false;
    private bool playedAlertSound = false;

    // SOund stuff
    [SerializeField] 
    private AudioClip[] alertSounds;
    [SerializeField] 
    private AudioSource enemyAudioSource;

    void Start()
    {
        enemyAudioSource = GetComponent<AudioSource>();
        enemyAudioSource.loop = false;

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
        if (GetComponent<SphereCollider>() == null)
        {
            CreateSphereTrigger();
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
            case EnemyAiType.Attack:
                // Stay stationary, and move towards player if you see them.
                if (CanSeePlayer(maxViewDistance) || canSeeBullet)
                {
                    if(!playedAlertSound)
                    {
                        enemyAudioSource.clip = alertSounds[Random.Range(0, alertSounds.Length - 1)];
                        enemyAudioSource.Play();
                        playedAlertSound = true;
                    }
                    isAlerted = true;
                }

                if (isAlerted && !canSeeBullet && !CanSeePlayer())
                {
                    // if enemy is alerted, but can't see the player or any bullets, start timer to return home
                    Invoke("DelayAlertedByPlayer", waitTimeSec);
                }

                // if you can still see the player after getting noticed, keep chasing him
                if (isAlerted)
                {
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    if (Vector3.Distance(transform.position, home) > 1)
                    {
                        // go home
                        agent.SetDestination(home);
                    }
                }

                break;
            case EnemyAiType.Patrol:
                // Use the predefined list of waypoints to move betwen, and if you see player, move towards them.
                if (CanSeePlayer(maxViewDistance))
                {
                    enemyAudioSource.clip = alertSounds[Random.Range(0, alertSounds.Length - 1)];
                    enemyAudioSource.Play();
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    Vector3 currWaypoint = waypoints[currWaypointIndex].position;
                    // normalize distance in 2d only
                    currWaypoint = new Vector3(currWaypoint.x, transform.position.y, currWaypoint.z);
                    if (Vector3.Distance(transform.position, currWaypoint) > 0.5f)
                    {
                        agent.SetDestination(currWaypoint);
                    }
                    else
                    {
                        // delay starting a new point to give it the illusion of thinking
                        if (!isIncreasingIndex)
                        {
                            StartCoroutine(DelayIndexIncrease(waitTimeSec));
                        }
                    }
                }
                break;
        }
    }

    private bool CanSeePlayer(float maxView = float.PositiveInfinity)
    {
        if (player == null)
        {
            return false;
        }
        // draw raycast to see if you hit the player
        Vector3 playerDirection = player.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDirection, out hit, Mathf.Infinity, playerLayerMask))
        {
            if (hit.collider.gameObject.name == "Player" && (hit.point - transform.position).magnitude < maxView)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            canSeeBullet = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            canSeeBullet = false;
        }
    }

    private void CreateSphereTrigger()
    {
        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = maxViewDistance;
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

    private void DelayAlertedByPlayer()
    {
        // This is invoked after a timer, and checks to see if we should still be chasing or not.
        if (isAlerted && !canSeeBullet && !CanSeePlayer())
        {
            // if you can't see the bullet and the player isn't visible anymore, return home.
            agent.SetDestination(home);
            isAlerted = false;
            playedAlertSound = false;
        }
    }
}
