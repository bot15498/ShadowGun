using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public Image flarefill;
    public int flarechargemax;
    int currentflares;
    public TextMeshProUGUI flarechargetext;
    void Start()
    {
        canflare = true;
        flaretimer = 0;
        cam = gameObject.transform;
        currentflares = flarechargemax;
    }

    // Update is called once per frame
    void Update()
    {
        flarechargetext.text = currentflares.ToString();
        if (currentflares < flarechargemax)
        {
            flarefill.fillAmount = flaretimer / flarecooldown;
        }
        else
        {
            flarefill.fillAmount = 1;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if(currentflares > 0)
            {
                currentflares -= 1;

                GameObject thrownflare = Instantiate(flare, throwPoint.position, Random.rotation);

                Rigidbody flarerb = thrownflare.GetComponent<Rigidbody>();

                Vector3 forceToadd = cam.forward * throwforce + transform.up * upwardthrowforce;


                flarerb.AddForce(forceToadd, ForceMode.Impulse);
            }
        }

        if (currentflares < flarechargemax)
        {
            flaretimer += Time.deltaTime;

            if(flaretimer >= flarecooldown)
            {
                currentflares += 1;
                flaretimer = 0;
            }
        }
    }
}
