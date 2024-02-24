using UnityEngine;
using UnityEngine.Events;

public class PlayerBullet : MonoBehaviour
{
    public delegate void OnHit(PlayerBullet bullet, Collider target);
    public static OnHit onHit;

    private void OnCollisionEnter(Collision collision)
    {
        onHit?.Invoke(this, collision.collider);
    }
}
