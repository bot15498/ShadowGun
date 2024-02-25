using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObserver : MonoBehaviour
{
    public LightType lightType;

    private Light currLight;
    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 previousScale;

    void Start()
    {
        currLight = GetComponent<Light>();
        lightType = currLight.type;
        ShadowObject.AddLight(this);
    }

    void FixedUpdate()
    {
        previousPosition = transform.position;
        previousRotation = transform.rotation;
        previousScale = transform.localScale;
    }

    private void OnDestroy()
    {
        ShadowObject.DeleteLight(this);
    }

    public bool TransformHasChanged()
    {
        return previousPosition != transform.position || previousRotation != transform.rotation || previousScale != transform.localScale;
    }

    public bool CanSeeObject(GameObject point, LayerMask layerMask, float maxDistance=float.PositiveInfinity)
    {
        if(lightType == LightType.Directional)
        {
            return true;
        }    

        // draw raycast to see if you hit the player
        Vector3 playerDirection = point.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDirection, out hit, maxDistance, layerMask))
        {
            if (hit.collider.gameObject.name == point.gameObject.name)
            {
                return true;
            }
        }
        return false;
    }
}
