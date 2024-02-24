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
}
