using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [SerializeField]
    private string targetTag = "Player";

    public delegate void OnHit(EnemyMelee bat, Collider target);
    public static OnHit onHit;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Enemy Melee hit: {other.gameObject.tag}");
        if (other.gameObject.tag == targetTag)
        {
            onHit?.Invoke(this, other);
        }
    }
}
