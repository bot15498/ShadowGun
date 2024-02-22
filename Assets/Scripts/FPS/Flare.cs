using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flare : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform cam;
    public Transform throwPoint;
    public GameObject flare;
    public float flarecooldown;
    private float flaretimer;
    bool canflare;
    public float throwforce;
    public float upwardthrowforce;
    void Start()
    {
        canflare = true;
        flaretimer = 0;
        cam = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if(canflare == true)
            {
                canflare = false;

                GameObject thrownflare = Instantiate(flare, throwPoint.position, Random.rotation);

                Rigidbody flarerb = thrownflare.GetComponent<Rigidbody>();

                Vector3 forceToadd = cam.forward * throwforce + transform.up * upwardthrowforce;


                flarerb.AddForce(forceToadd, ForceMode.Impulse);
            }
        }

        if (canflare == false)
        {
            flaretimer += Time.deltaTime;
            if(flaretimer >= flarecooldown)
            {
                canflare = true;
                flaretimer = 0;
            }
        }
    }
}
