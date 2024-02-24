using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField]
    private LayerMask targetHitLayer;

    public delegate void OnHit(EnemyBullet bullet, Collider target);
    public static OnHit onHit;

    private void OnCollisionEnter(Collision collision)
    {
        onHit?.Invoke(this, collision.collider);

        if (collision.collider.gameObject.layer == targetHitLayer)
        {
            onHit?.Invoke(this, collision.collider);
        }
    }
}
