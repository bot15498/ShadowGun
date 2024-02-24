using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotation : MonoBehaviour
{

    Transform cameraTransform;
    Ray RayOrigin;
    RaycastHit HitInfo;
    Vector3 aimpoint;
    public GameObject gun;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        
    }

    // Update is called once per frame
    void Update()
    {


        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out HitInfo, 200.0f))
        {

            Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 200.0f, Color.yellow);
            aimpoint = HitInfo.point;
            gun.transform.LookAt(aimpoint);
        }
        else
        {
            Debug.Log("nothing hit");
            //gun.transform.rotation = Quaternion.identity;
        }


        


    }
}
