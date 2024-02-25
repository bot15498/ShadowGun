using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField]
    private string targetTag = "Player";

    public delegate void OnHit(EnemyBullet bullet, Collider target);
    public static OnHit onHit;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.gameObject.tag);
        if (collision.collider.gameObject.tag == targetTag)
        {
            onHit?.Invoke(this, collision.collider);
        }
    }
}
