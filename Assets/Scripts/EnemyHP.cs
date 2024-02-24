using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHP : MonoBehaviour {
    private ShadowObject shadowObject;
    [SerializeField] AudioClip deathSound;
    private AudioSource shadowAudioSource;

    private void Awake() {
        PlayerBullet.onHit += TakeDamage; // Listen to bullet collision event
    }

    private void Start()
    {
        shadowObject = GetComponent<ShadowObject>();
        shadowAudioSource = GetComponent<AudioSource>();
    }

    private void TakeDamage(PlayerBullet bullet, Collider shadowTarget) {
        List<MeshCollider> currColliders = shadowObject.shadowMap.Values.Select(x => x.shadowCollider).ToList();
        foreach(MeshCollider collider in currColliders)
        {
            if(shadowTarget == collider)
            {
                Debug.Log("Enemy took damage");
                // Kill bullet
                Destroy(bullet.gameObject);

                // play sound
                shadowAudioSource.clip = deathSound;
                shadowAudioSource.Play();

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
