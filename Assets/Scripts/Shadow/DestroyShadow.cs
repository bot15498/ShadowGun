using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DestroyShadow : MonoBehaviour
{
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

        // Temporarily move the object upwards.
        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {

            int[] indices = M.GetTriangles(submesh);

            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                for (int n = 0; n < 3; n++)
                {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newNormals[n] = normals[index];
                }

                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;

                mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                GameObject GO = new GameObject("Triangle " + (i / 3));
                GO.layer = LayerMask.NameToLayer("ShadowParticle");
                GO.transform.position = transform.position;
                GO.transform.rotation = transform.rotation;
                MeshRenderer particleMeshRender = GO.AddComponent<MeshRenderer>();
                particleMeshRender.material = materials[submesh];
                particleMeshRender.shadowCastingMode = ShadowCastingMode.Off;
                GO.AddComponent<MeshFilter>().mesh = mesh;
                BoxCollider particleBoxCollider = GO.AddComponent<BoxCollider>();
                particleBoxCollider.size = new Vector3(particleBoxCollider.size.x, 0.05f, particleBoxCollider.size.z);
                Vector3 explosionPos = new Vector3(transform.position.x + Random.Range(-10.5f, 10.5f), transform.position.y + Random.Range(0f, 10.5f), transform.position.z + Random.Range(-10.5f, 10.5f));
                GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(500, 2000), explosionPos, 30);
                Destroy(GO, 5 + Random.Range(0.0f, 1.0f));
            }
        }

        // Play smash
        GetComponent<AudioSource>().Play();

        GetComponent<Renderer>().enabled = false;

        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
