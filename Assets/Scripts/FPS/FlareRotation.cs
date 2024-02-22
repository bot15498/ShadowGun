using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public float lifespan;
    float timer;
    Rigidbody flarerb;

    public float torqueforce;
    public float rotationForce;
    void Start()
    {
        timer = 0;
        flarerb = GetComponent<Rigidbody>();
        Vector3 randomRotation = new Vector3(Random.Range(-torqueforce, torqueforce), Random.Range(-torqueforce, torqueforce), Random.Range(-torqueforce, torqueforce));
        flarerb.AddRelativeTorque(randomRotation * rotationForce, ForceMode.Impulse);

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= lifespan)
        {
            Destroy(gameObject);
        }
    }
}
