using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private MeshFilter spawnRegion;
    [SerializeField]
    private List<GameObject> enemiesToInstantiate;
    [SerializeField]
    private int maxNumEnemies = 5;
    [SerializeField]
    private List<GameObject> currEnemies = new List<GameObject>();
    [SerializeField]
    private Vector3[] spawnLimits;

    private float respawnCheck = 2f;
    private float currRespawnCheck = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnLimits = GetSpawnLimits();
    }

    // Update is called once per frame
    void Update()
    {
        if (currEnemies.Count < maxNumEnemies)
        {
            GameObject enemy = enemiesToInstantiate[Random.Range(0, enemiesToInstantiate.Count)];
            currEnemies.Add(Instantiate(enemy, PickSpawnPoint(), Quaternion.identity).transform.GetChild(0).gameObject);

            EnemyAiBase aiBase = enemy.transform.GetChild(0).GetComponent<EnemyAiBase>();
            aiBase.maxViewDistance = 9999f;
            aiBase.aiType = EnemyAiType.Attack;
        }

        if (currRespawnCheck >= respawnCheck)
        {
            currEnemies = currEnemies.Where(x => x != null).ToList();
            currRespawnCheck = 0;
        }

        currRespawnCheck += Time.deltaTime;
    }

    private Vector3 PickSpawnPoint()
    {
        float x = Random.Range(spawnLimits[0].x, spawnLimits[1].x);
        float y = spawnLimits[0].y;
        float z = Random.Range(spawnLimits[0].z, spawnLimits[1].z);

        return new Vector3(x, y, z);
    }

    private Vector3[] GetSpawnLimits()
    {
        Mesh spawnMesh = spawnRegion.mesh;
        Vector3[] vertices = spawnMesh.vertices;
        for(int i=1; i< vertices.Length; i++)
        {
            vertices[i] = transform.TransformPoint(vertices[i]);
        }
        

        float targetY = vertices.Max(x => x.y);
        vertices = vertices.Where(x => x.y == targetY).OrderBy(x => x.x).OrderBy(x => x.z).ToArray();

        Vector3 corner1 = vertices[0];
        Vector3 corner2 = vertices[vertices.Length - 1];

        return new Vector3[] { corner1, corner2 };
    }
}
