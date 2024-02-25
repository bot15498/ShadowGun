using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootAi : MonoBehaviour, IEnemyActionAi
{
    // This script should get put on the Enemy object along with Enemy Ai Base
    // Soundstuff
    public delegate void OnEnemyFire();
    public static OnEnemyFire onEnemyFire;

    // This is the toggle for whether to do stuff or not. 
    [SerializeField]
    private bool _isAgressive = false;
    [SerializeField]
    private float _maxActionRange;

    // shooty object stuff
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform shootPoint;

    // Shoot range and rate
    [SerializeField]
    private float shootRange = 20f;
    [SerializeField]
    private float shootRateSec = 1.0f;
    [SerializeField]
    private float shootTimer = 0f;
    
    private EnemyAiBase aiBase;

    public bool isAgressive { get => _isAgressive; set => _isAgressive = value; }
    public float MaxActionRange { get => _maxActionRange; set => _maxActionRange = value; }

    void Start()
    {
        aiBase = GetComponent<EnemyAiBase>();
    }

    void FixedUpdate()
    {
        if (_isAgressive)
        {
            if (shootTimer >= shootRateSec)
            {
                // Timer has run out, time to shoot again
                Shoot();
                shootTimer = 0f;
            }
            shootTimer += Time.deltaTime;
        }
        else
        {
            shootTimer = 0f;
        }
    }

    private void Shoot()
    {
        // Create bullet and shoot it in the direction of the player
        Vector3 direction = aiBase.player.transform.position - shootPoint.position;
        //Quaternion playerRotation = Quaternion.FromToRotation(shootPoint.position, aiBase.player.transform.position);
        Quaternion playerRotation = Quaternion.LookRotation(direction, Vector3.up);
        GameObject newBullet = Instantiate(bullet, shootPoint.position, playerRotation);

        // PLay sound
        onEnemyFire?.Invoke();
    }

    public bool PlayerInRange(Transform player)
    {
        return Vector3.Distance(player.position, transform.position) < MaxActionRange;
    }
}
