using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public int maxBounce = 4;
    public float timeoutTimeSec = 10f;
    [SerializeField]
    private int currBounce = 0;
    [SerializeField]
    private float aliveTime = 0f;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed);

    }

    // Update is called once per frame
    void Update()
    {
        if (maxBounce > 0 && currBounce >= maxBounce)
        {
            // delete itself
            Destroy(gameObject);
        }
        else if (timeoutTimeSec > 0f && aliveTime >= timeoutTimeSec)
        {
            Destroy(gameObject);
        }

        aliveTime += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        currBounce++;
    }
}
