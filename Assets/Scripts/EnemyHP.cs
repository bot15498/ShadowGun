using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHP : MonoBehaviour {
    private ShadowObject shadowObject;

    public delegate void OnDeath(GameObject enemy);
    public static OnDeath onDeath;

    private void Awake() {
        PlayerBullet.onHit += TakeDamage; // Listen to bullet collision event
    }

    private void Start()
    {
        shadowObject = GetComponent<ShadowObject>();
    }

    private void TakeDamage(PlayerBullet bullet, Collider shadowTarget) {
        List<MeshCollider> currColliders = shadowObject.shadowMap.Values.Select(x => x.shadowCollider).ToList();
        foreach(MeshCollider collider in currColliders)
        {
            if(shadowTarget == collider)
            {
                // Kill bullet
                Destroy(bullet.gameObject);

                // play sound
                onDeath?.Invoke(gameObject);

                // Prepare enemy for death
                StartCoroutine(shadowObject.DestroyEntity(collider.gameObject));

                // Kill this current shadow
                DestroyShadow shadowScript = collider.gameObject.GetComponent<DestroyShadow>();
                if(shadowScript != null)
                {
                    StartCoroutine(shadowScript.DestroyMesh());
                }
            }
        }
    }
}
