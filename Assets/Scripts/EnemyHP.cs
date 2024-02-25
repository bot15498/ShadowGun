using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField]
    private int maxHealth =2;
    [SerializeField]
    private int health = 2;

    private ShadowObject shadowObject;

    public delegate void OnDeath(GameObject enemy);
    public static OnDeath onDeath;

    private void Awake()
    {
        PlayerBullet.onHit += TakeDamage; // Listen to bullet collision event
    }

    private void Start()
    {
        shadowObject = GetComponent<ShadowObject>();
        health = maxHealth;
    }

    private void TakeDamage(PlayerBullet bullet, Collider shadowTarget)
    {
        List<MeshCollider> currColliders = shadowObject.shadowMap.Values.Select(x => x.shadowCollider).ToList();
        foreach (MeshCollider collider in currColliders)
        {
            if (shadowTarget == collider)
            {
                health--;

                if (health <= 0)
                {
                    // time to die I guess
                    Die(bullet, shadowTarget);
                }
            }
        }
    }

    private void Die(PlayerBullet bullet, Collider shadowTarget)
    {
        // Kill bullet
        Destroy(bullet.gameObject);

        // Prepare enemy for death
        // Kill all the children
        if (gameObject != null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ShadowObject currChild = transform.GetChild(i).gameObject.GetComponent<ShadowObject>();
                if (currChild != null)
                {
                    StartCoroutine(currChild.DestroyEntity(null));
                }
            }
        }
        StartCoroutine(shadowObject.DestroyEntity(shadowTarget.gameObject));

        // Kill this current shadow
        DestroyShadow shadowScript = shadowTarget.gameObject.GetComponent<DestroyShadow>();
        if (shadowScript != null)
        {
            StartCoroutine(shadowScript.DestroyMesh());
        }
    }
}
