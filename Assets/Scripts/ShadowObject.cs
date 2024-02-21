///////////////////////////////////////////////////////////////////////////////////////////
// source — [TUTORIAL] Interactive Shadows in Unity                                      //
// https://www.youtube.com/watch?v=3MnA8lYQ_P0                                           //
// https://github.com/PixTrick/InteractiveShadowTutorial/blob/main/InteractiveShadows.cs //
///////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Linq;

public class ShadowObject : MonoBehaviour
{
    [SerializeField] private Transform shadowTransform;

    [SerializeField] private Transform lightTransform;
    private LightType lightType;

    [SerializeField] private LayerMask targetLayerMask;

    [SerializeField] private Vector3 extrusionDirection = Vector3.zero;

    [SerializeField]
    private Vector3[] objectVertices;
    [SerializeField]
    private int[] objectTris;

    private Mesh shadowColliderMesh;
    private MeshCollider shadowCollider;
    private MeshFilter shadowColliderFilter;
    [SerializeField] private Material shadowMaterial;

    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 previousScale;

    private bool canUpdateCollider = true;

    [SerializeField] [Range(0.02f, 1f)] private float shadowColliderUpdateTime = 0.08f;

    private void Awake()
    {
        shadowColliderMesh = new Mesh { name="Generated shadow"};
        InitializeShadowCollider();

        lightType = lightTransform.GetComponent<Light>().type;

        objectVertices = transform.GetComponent<MeshFilter>().mesh.vertices.Distinct().ToArray();
        objectTris = transform.GetComponent<MeshFilter>().mesh.triangles;
    }

    private void Update()
    {
        shadowTransform.position = transform.position;
    }

    private void FixedUpdate()
    {
        if (TransformHasChanged() && canUpdateCollider)
        {
            Invoke("UpdateShadowCollider", shadowColliderUpdateTime);
            canUpdateCollider = false;
        }

        previousPosition = transform.position;
        previousRotation = transform.rotation;
        previousScale = transform.localScale;
    }

    private void InitializeShadowCollider()
    {
        GameObject shadowGameObject = shadowTransform.gameObject;
        //shadowGameObject.hideFlags = HideFlags.HideInHierarchy; //OPTIONNAL
        shadowCollider = shadowGameObject.AddComponent<MeshCollider>();
        shadowCollider.convex = true;
        shadowCollider.isTrigger = true;

        // Add the mesh filter
        shadowColliderFilter = shadowGameObject.AddComponent<MeshFilter>();
        shadowColliderFilter.sharedMesh = shadowColliderMesh;
        MeshRenderer shadowRender = shadowGameObject.AddComponent<MeshRenderer>();
        shadowRender.material = shadowMaterial;
    }

    private void UpdateShadowCollider()
    {
        shadowColliderMesh.vertices = ComputeShadowColliderMeshVertices();
        shadowColliderMesh.triangles = ComputeShadowColliderMeshTriangles();
        shadowCollider.sharedMesh = shadowColliderMesh;
        canUpdateCollider = true;
    }

    private Vector3[] ComputeShadowColliderMeshVertices()
    {
        Vector3[] points = new Vector3[2 * objectVertices.Length];

        Vector3 raycastDirection = lightTransform.forward;

        int n = objectVertices.Length;

        for (int i = 0; i < n; i++)
        {
            Vector3 point = transform.TransformPoint(objectVertices[i]);

            if (lightType != LightType.Directional)
            {
                raycastDirection = point - lightTransform.position;
            }

            points[i] = ComputeIntersectionPoint(point, raycastDirection);

            points[n + i] = ComputeExtrusionPoint(point, points[i]);
        }

        // sort by x, then z, then y
        points = points.OrderBy(x => x.y)
                        .ThenByDescending(x => x.x)
                        .ThenByDescending(x => x.z).ToArray();

        return points;
    }

    private int[] ComputeShadowColliderMeshTriangles()
    {
        // the first half of the vertices are on the floor.
        // the second half oteh vertices are extruded upwards
        int[] triangles = new int[objectTris.Length];
        for(int i = 0; i< triangles.Length / 6; i++)
        {
            triangles[i * 6 + 0] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;

            triangles[i * 6 + 3] = i * 2 + 2;
            triangles[i * 6 + 4] = i * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 3;
        }
        return triangles;
    }

    private Vector3 ComputeIntersectionPoint(Vector3 fromPosition, Vector3 direction)
    {
        RaycastHit hit;

        if (Physics.Raycast(fromPosition, direction, out hit, Mathf.Infinity, targetLayerMask))
        {
            return hit.point - transform.position;
        }

        return fromPosition + 100 * direction - transform.position;
    }

    private Vector3 ComputeExtrusionPoint(Vector3 objectVertexPosition, Vector3 shadowPointPosition)
    {
        if (extrusionDirection.sqrMagnitude == 0)
        {
            return objectVertexPosition - transform.position;
        }

        return shadowPointPosition + extrusionDirection;
    }

    private bool TransformHasChanged()
    {
        return previousPosition != transform.position || previousRotation != transform.rotation || previousScale != transform.localScale;
    }
}