using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootAi : MonoBehaviour
{
    // This script should get put on the Enemy object along with Enemy Ai Base
    // Soundstuff
    public delegate void OnPlayerFire();
    public static OnPlayerFire onEnemyFire;

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

    void Start()
    {
        aiBase = GetComponent<EnemyAiBase>();
    }

    void FixedUpdate()
    {
        if(shootTimer >= shootRateSec)
        {
            // Timer has run out, time to shoot again
            shootTimer = 0f;
        }
        shootTimer = Time.deltaTime;
    }

    private void Shoot()
    {
        // Create bullet and shoot it in the direction of the player
        GameObject newBullet = Instantiate(bullet, shootPoint.position, Quaternion.identity);


        // PLay sound
        onEnemyFire?.Invoke();
    }
}
