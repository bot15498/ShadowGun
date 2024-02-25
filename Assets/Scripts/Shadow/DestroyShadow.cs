using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DestroyShadow : MonoBehaviour
{
    public Material opaqueMaterial;

    // Audio events
    public delegate void OnShatter(GameObject shadow);
    public static OnShatter onShatter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator DestroyMesh()
    {
        // https://discussions.unity.com/t/script-to-break-mesh-into-smaller-pieces/143755/2\
        if (GetComponent<MeshFilter>() == null || GetComponent<SkinnedMeshRenderer>() == null)
        {
            yield return null;
        }

        if (GetComponent<MeshCollider>())
        {
            GetComponent<MeshCollider>().enabled = false;
        }

        Mesh M = new Mesh();
        if (GetComponent<MeshFilter>())
        {
            M = GetComponent<MeshFilter>().mesh;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        Material[] materials = new Material[0];
        if (GetComponent<MeshRenderer>())
        {
            materials = GetComponent<MeshRenderer>().materials;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            materials = GetComponent<SkinnedMeshRenderer>().materials;
        }

        int numVerticesPerParticle = 6;
        int objectSkipCount = 3;
        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {

            int[] indices = M.GetTriangles(submesh);

            // Loop over all triangles in groups of numVerticesPerParticle
            for (int i = numVerticesPerParticle-1; i < indices.Length; i += numVerticesPerParticle * objectSkipCount)
            {
                Vector3[] newVerts = new Vector3[numVerticesPerParticle];
                Vector3[] newNormals = new Vector3[numVerticesPerParticle];
                for (int n = 0; n < numVerticesPerParticle; n++)
                {
                    int index = indices[i - n];
                    newVerts[n] = verts[index];
                    newNormals[n] = normals[index];
                }

                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;

                mesh.triangles = new int[] { 0, 1, 2, 
                                            1, 2, 3,
                                            2, 3, 4,
                                            0, 2, 4,
                                            3, 4, 5,

                                            5, 4, 3,
                                            4, 2, 0,
                                            4, 3, 2,
                                            3, 2, 1, 
                                            2, 1, 0 };

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                GameObject GO = new GameObject("Triangle " + (i / numVerticesPerParticle));
                GO.layer = LayerMask.NameToLayer("ShadowParticle");
                GO.transform.position = transform.position;
                GO.transform.rotation = transform.rotation;
                MeshRenderer particleMeshRender = GO.AddComponent<MeshRenderer>();
                particleMeshRender.material = opaqueMaterial;
                particleMeshRender.shadowCastingMode = ShadowCastingMode.Off;
                GO.AddComponent<MeshFilter>().mesh = mesh;
                BoxCollider particleBoxCollider = GO.AddComponent<BoxCollider>();
                particleBoxCollider.size = new Vector3(particleBoxCollider.size.x, 0.05f, particleBoxCollider.size.z);
                Vector3 explosionPos = new Vector3(transform.position.x + Random.Range(-3.5f, 3.5f), transform.position.y + Random.Range(0f, 10.5f), transform.position.z + Random.Range(-3.5f, 3.5f));
                GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(750, 2000), explosionPos, 30);
                StartCoroutine(DelayedDestroy(GO, 1 + Random.Range(0.0f, 0.5f)));
            }
        }

        // Play smash
        onShatter?.Invoke(gameObject);

        GetComponent<Renderer>().enabled = false;

        yield return new WaitForSeconds(7.0f);
        Destroy(gameObject);
    }

    private IEnumerator DelayedDestroy(GameObject go, float delay)
    {
        MeshRenderer shadowParticleRender = go.GetComponent<MeshRenderer>();
        float time = 0f;
        while (time < delay)
        {
            float newAlpha = Mathf.Lerp(0f, 1f, time / delay);
            shadowParticleRender.material.color = new Color(shadowParticleRender.material.color.r,
                                                            shadowParticleRender.material.color.g,
                                                            shadowParticleRender.material.color.b,
                                                            1 - newAlpha);

            time += Time.deltaTime;
            yield return null;
        }

        // Now destroy the game object
        Destroy(go);
    }
}
