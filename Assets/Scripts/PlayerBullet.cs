using UnityEngine;
using UnityEngine.Events;

public class PlayerBullet : MonoBehaviour
{
    public delegate void OnHit(PlayerBullet bullet, Collider target);
    public static OnHit onHit;

    private void OnTriggerEnter(Collider other) {
        onHit?.Invoke(this, other);
    }
}
