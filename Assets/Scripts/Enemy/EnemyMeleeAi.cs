using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAi : MonoBehaviour, IEnemyActionAi
{
    // Soundstuff
    public delegate void OnEnemySwing();
    public static OnEnemySwing onEnemySwing;

    [SerializeField]
    private bool _isAgressive = false;
    [SerializeField]
    private float _maxActionRange;

    // swing range and rate
    [SerializeField]
    private float swingRangeSec = 1.0f;
    [SerializeField]
    private float swingTimer = 0f;

    private EnemyAiBase aiBase;
    private Animator anime;

    public bool isAgressive { get => _isAgressive; set => _isAgressive = value; }
    public float MaxActionRange { get => _maxActionRange; set => _maxActionRange = value; }


    // This script should get put on the Enemy object along with Enemy Ai Base
    void Start()
    {
        aiBase = GetComponent<EnemyAiBase>();
        anime = GetComponent<Animator>();
    }

    void Update()
    {
        if (_isAgressive)
        {
            if (swingTimer >= swingRangeSec)
            {
                // Timer has run out, time to shoot again
                Swing();
                swingTimer = 0f;
            }
            swingTimer += Time.deltaTime;
        }
        else
        {
            swingTimer = 0f;
        }
    }

    private void Swing()
    {
        // Play sound
        onEnemySwing?.Invoke();

        // tell animator to do it's thing
        anime.SetTrigger("EnemyTakeSwing");
    }

    public bool PlayerInRange(Transform player)
    {
        return Vector3.Distance(player.position, transform.position) < MaxActionRange;
    }

}
