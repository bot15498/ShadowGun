using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 3;
    [SerializeField]
    private int health = 3;


    public delegate void OnDeath(GameObject player);
    public static OnDeath onDeath;
    public delegate void OnHit(GameObject player);
    public static OnHit onHit;

    void Start()
    {
        EnemyBullet.onHit += TakeDamage; // Listen to bullet collision event from an enemy bullet
        EnemyMelee.onHit += TakeDamage; 
        health = maxHealth;
    }

    void Update()
    {

    }

    public void ResetHealth()
    {
        health = maxHealth;
    }

    private void TakeDamage(EnemyBullet enemyBullet, Collider target)
    {
        // decrease health
        health -= 1;

        if (health > 0)
        {
            onHit?.Invoke(gameObject);

            // play some effect?

        }
        else
        {
            // time to die I guess.
            Die();
        }
    }

    private void TakeDamage(EnemyMelee enemyBullet, Collider target)
    {
        // decrease health
        health -= 1;

        if (health > 0)
        {
            onHit?.Invoke(gameObject);

            // play some effect?

        }
        else
        {
            // time to die I guess.
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("YOU DIED");
        // play sound
        onDeath?.Invoke(gameObject);

        // gameover?
    }
}
