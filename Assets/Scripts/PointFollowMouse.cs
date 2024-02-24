using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointFollowMouse : MonoBehaviour
{
    public Rigidbody rb;
    public float z;

    void start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));
    }
}
