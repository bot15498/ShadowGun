using UnityEngine;

public class EnemyHP : MonoBehaviour {
    [SerializeField] private Collider hitbox;

    private void Awake() {
        PlayerBullet.onHit += TakeDamage; // Listen to bullet collision event
    }

    private void TakeDamage(PlayerBullet bullet, Collider target) {
        if (target == hitbox)
            Debug.Log("Enemy took damage");
    }
}
