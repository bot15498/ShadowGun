///////////////////////////////////////////////////////////////////////////////////////////
// source � [TUTORIAL] Interactive Shadows in Unity                                      //
// https://www.youtube.com/watch?v=3MnA8lYQ_P0                                           //
// https://github.com/PixTrick/InteractiveShadowTutorial/blob/main/InteractiveShadows.cs //
///////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Collections;

[Serializable]
public class ShadowHolder
{
    public Mesh shadowColliderMesh;
    public MeshCollider shadowCollider;
    public MeshFilter shadowColliderFilter;
    public Transform shadowTransform;
    public bool canUpdateCollider =true;
}


public class ShadowObject : MonoBehaviour
{
    // I love essentially global static variables!!!!!!!!!!!!
    private static List<LightObserver> lights = null;
    private static List<ShadowObject> shadowObjects = null;

    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float extrusion = 0.1f;

    [SerializeField] private Dictionary<LightObserver, ShadowHolder> shadowMap = new Dictionary<LightObserver, ShadowHolder>();
    [SerializeField] private List<ShadowHolder> shadowList = new List<ShadowHolder>();
    [SerializeField] private Material shadowMaterial;

    // Information about this object's mesh
    [SerializeField] private Vector3[] objectVertices;
    [SerializeField] private int[] objectTris;

    // variables to detect if the object has moved or not, which is then use to update shadows 
    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 previousScale;

    [SerializeField][Range(0.02f, 1f)] private float shadowColliderUpdateTime = 0.08f;

    public static void AddLight(LightObserver toad)
    {
        lights.Add(toad);
    }

    public static void DeleteLight(LightObserver ded)
    {
        lights.Remove(ded);
        foreach(ShadowObject obj in shadowObjects)
        {
            obj.DeleteShadow(ded);
        }
    }

    private void Awake()
    {
        // create the list of lights for all shadows to use if it doesn't exist. 
        if (lights == null)
        {
            lights = new List<LightObserver>();
        }
        if (shadowObjects == null)
        {
            shadowObjects = new List<ShadowObject>();
        }

        // Save information about this object's mesh for shadow rendering. 
        objectVertices = transform.GetComponent<MeshFilter>().mesh.vertices;
        objectTris = transform.GetComponent<MeshFilter>().mesh.triangles;

        shadowObjects.Add(this);
    }

    private void Update()
    {
        foreach(ShadowHolder shadow in shadowMap.Values)
        {
            shadow.shadowTransform.position = transform.position;
        }
    }

    private void FixedUpdate()
    {
        // prestore if the object has moved or not
        bool currObjectHasChanged = TransformHasChanged();

        // Check each light
        foreach (LightObserver light in lights)
        {
            ShadowHolder currShadow;
            if (!shadowMap.TryGetValue(light, out currShadow))
            {
                // shadow for this light does not exist. Going to create it now
                currShadow = InitializeShadowCollider();
                shadowMap.Add(light, currShadow);
                shadowList.Add(currShadow);
            }

            // light exists, deal with if it has moved or not
            if (currShadow.canUpdateCollider && (currObjectHasChanged || light.TransformHasChanged()))
            {
                StartCoroutine(UpdateShadowCollider(light, currShadow, shadowColliderUpdateTime));
                currShadow.canUpdateCollider = false;
            }
        }

        previousPosition = transform.position;
        previousRotation = transform.rotation;
        previousScale = transform.localScale;
    }

    private ShadowHolder InitializeShadowCollider()
    {
        ShadowHolder shadow = new ShadowHolder();

        GameObject shadowGameObject = new GameObject($"{gameObject.name} Shadow");
        //shadowGameObject.hideFlags = HideFlags.HideInHierarchy; //OPTIONNAL
        shadow.shadowTransform = shadowGameObject.transform;

        // Add collider
        shadow.shadowCollider = shadowGameObject.AddComponent<MeshCollider>();
        shadow.shadowCollider.convex = false;
        shadow.shadowCollider.isTrigger = false;

        // create mesh
        shadow.shadowColliderMesh = new Mesh { name = $"{gameObject.name} Shadow mesh" };

        // Add the mesh filter
        shadow.shadowColliderFilter = shadowGameObject.AddComponent<MeshFilter>();
        shadow.shadowColliderFilter.sharedMesh = shadow.shadowColliderMesh;
        MeshRenderer shadowRender = shadowGameObject.AddComponent<MeshRenderer>();
        shadowRender.material = shadowMaterial;

        return shadow;
    }

    private IEnumerator UpdateShadowCollider(LightObserver light, ShadowHolder shadow, float delay)
    {
        yield return new WaitForSeconds(delay);
        shadow.shadowColliderMesh.vertices = ComputeShadowColliderMeshVertices(light);
        shadow.shadowColliderMesh.triangles = objectTris;
        shadow.shadowCollider.sharedMesh = shadow.shadowColliderMesh;
        shadow.canUpdateCollider = true;
    }

    private Vector3[] ComputeShadowColliderMeshVertices(LightObserver light)
    {
        Vector3[] points = new Vector3[objectVertices.Length];

        Vector3 raycastDirection = light.transform.forward;

        int n = objectVertices.Length;

        for (int i = 0; i < n; i++)
        {
            Vector3 point = transform.TransformPoint(objectVertices[i]);

            if (light.lightType != LightType.Directional)
            {
                raycastDirection = point - light.transform.position;
            }

            points[i] = ComputeIntersectionAndExtrusion(point, raycastDirection, extrusion);
        }

        return points;
    }

    private Vector3 ComputeIntersectionAndExtrusion(Vector3 fromPosition, Vector3 direction, float offset)
    {
        // Use the hit detection information on where to extrude
        RaycastHit hit;

        if (Physics.Raycast(fromPosition, direction, out hit, Mathf.Infinity, targetLayerMask))
        {
            return hit.point - transform.position + hit.normal * offset;
        }

        return fromPosition + 100 * direction - transform.position;
    }

    private bool TransformHasChanged()
    {
        return previousPosition != transform.position || previousRotation != transform.rotation || previousScale != transform.localScale;
    }

    public void DeleteShadow(LightObserver light)
    {
        ShadowHolder toDelete = shadowMap[light];
        shadowMap.Remove(light);
        Destroy(toDelete.shadowTransform.gameObject);
    }

    //private int[] ComputeShadowColliderMeshTriangles()
    //{
    //    // the first half of the vertices are on the floor.
    //    // the second half oteh vertices are extruded upwards
    //    int[] triangles = new int[objectTris.Length];
    //    for (int i = 0; i < triangles.Length / 6; i++)
    //    {
    //        triangles[i * 6 + 0] = i * 2;
    //        triangles[i * 6 + 1] = i * 2 + 1;
    //        triangles[i * 6 + 2] = i * 2 + 2;

    //        triangles[i * 6 + 3] = i * 2 + 2;
    //        triangles[i * 6 + 4] = i * 2 + 1;
    //        triangles[i * 6 + 5] = i * 2 + 3;
    //    }
    //    return triangles;
    //}


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    foreach (Vector3 vert in shadowColliderMesh.vertices)
    //    {
    //        Vector3 worldpos = shadowTransform.TransformPoint(vert);
    //        Gizmos.DrawCube(worldpos, Vector3.one * 0.01f);
    //    }
    //}
}