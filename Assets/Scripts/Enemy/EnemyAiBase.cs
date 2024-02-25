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
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    public GameObject player;
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

    // Enemy attack stuff
    private IEnemyActionAi[] enemyActions;

    // Audio events
    public delegate void OnAlert(GameObject enemy);
    public static OnAlert onAlert;

    void Start()
    {
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (home == Vector3.zero)
        {
            //make it here
            home = transform.position;
        }

        // Get all the enemy actions on this enemy, and sort by max view distance descending
        enemyActions = GetComponents<IEnemyActionAi>();
        enemyActions = enemyActions.OrderBy(x => x.MaxActionRange).Reverse().ToArray();
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
                if (CanSeePlayer(maxViewDistance) || canSeeBullet)
                {
                    if (!playedAlertSound)
                    {
                        onAlert?.Invoke(gameObject);
                        playedAlertSound = true;
                    }
                    isAlerted = true;
                    DoActionCheck();
                    FacePlayer();
                }

                if (isAlerted && !canSeeBullet && !CanSeePlayer())
                {
                    // if enemy is alerted, but can't see the player or any bullets, start timer to return home
                    Invoke("DelayAlertedByPlayer", waitTimeSec);
                }
                break;
            case EnemyAiType.Attack:
                // Stay stationary, and move towards player if you see them.
                if (CanSeePlayer(maxViewDistance) || canSeeBullet)
                {
                    if(!playedAlertSound)
                    {
                        onAlert?.Invoke(gameObject);
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
                    if (Vector3.Distance(transform.position, player.transform.position) > 1)
                    {
                        agent.SetDestination(player.transform.position);
                    }
                    DoActionCheck();
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
                if (CanSeePlayer(maxViewDistance) || canSeeBullet)
                {
                    onAlert?.Invoke(gameObject);
                    agent.SetDestination(player.transform.position);
                    DoActionCheck();
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

    private void DoActionCheck()
    {
        IEnemyActionAi actionToDo = null;
        foreach (IEnemyActionAi action in enemyActions)
        {
            if (action.PlayerInRange(player.transform))
            {
                if (actionToDo != null)
                {
                    actionToDo.isAgressive = false;
                }
                actionToDo = action;
            }
        }

        // If we are in range to do something, tell the controller to go do it.
        if (actionToDo != null)
        {
            actionToDo.isAgressive = true;
        }
        else
        {
            // otherwise turn them all off
            foreach (IEnemyActionAi action in enemyActions)
            {
                action.isAgressive = false;
            }
        }
    }

    public bool CanSeePlayer(float maxView = float.PositiveInfinity)
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
            if (hit.collider.gameObject.tag == "Player" && (hit.point - transform.position).magnitude < maxView)
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
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, maxViewDistance);
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

    private void FacePlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y + 90, 0f);
    }
}
