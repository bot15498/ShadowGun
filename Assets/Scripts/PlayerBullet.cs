using UnityEngine;
using UnityEngine.Events;

public class PlayerBullet : MonoBehaviour
{
    public delegate void OnHit(PlayerBullet bullet, Collider target);
    public static OnHit onHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Shadow"))
        {
            onHit?.Invoke(this, collision.collider);
        }
    }
}
