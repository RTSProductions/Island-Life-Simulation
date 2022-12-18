using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject chunk;

    public int xSize = 9;

    public int zSize = 9;

    [Range(1, 7)]
    public int octaves = 1;

    [Range(0, 999999)]
    public int seed = 0000000;

    public bool randomizeSeed;

    public bool randomizeSeaLevel;

    public float seaLevel = 5;
    [HideInInspector]
    public float maxDist;

    public LayerMask terrain;

    //public float[,] falloffMap;

    [HideInInspector]
    public Transform midPoint;

    public GameObject ocean;

    public FoliageSpawner[] Foliage;

    public VillageSpawner villages;

    Vector2 origin;

    // Start is called before the first frame update
    void Start()
    {
        if (randomizeSeed == true)
        {
            seed = Random.Range(0, 999999);
        }
        if (randomizeSeaLevel == true)
        {
            seaLevel = Random.Range(5, 23);
        }
        origin = new Vector2(Mathf.Sqrt(seed), Mathf.Sqrt(seed));
        int xIslandSize = xSize * chunk.GetComponent<MeshGenerator>().xSize;
        int zIslandSize = zSize * chunk.GetComponent<MeshGenerator>().zSize;

        //falloffMap = new float[xIslandSize, zIslandSize];
        //for (int i = 0; i < xIslandSize; i++)
        //{
        //    for (int j = 0; j < zIslandSize; j++)
        //    {
        //        float x = i / (float)xIslandSize * 2 - 1;
        //        float z = j / (float)zIslandSize * 2 - 1;

        //        float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
        //        falloffMap[i, j] = Evaluate(value);
        //    }
        //}
        float midX = xIslandSize / 2;
        float midZ = zIslandSize / 2;
        midPoint = new GameObject("Mid Point").transform;
        midPoint.position = new Vector3(midX, 0, midZ);
        ocean.transform.position = new Vector3(midX, seaLevel, midZ);
        maxDist = Vector3.Distance(transform.position, midPoint.position);
        GenerateMap();
        StartCoroutine(waitPlaceObjs());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateMap()
    {
        int index = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var chunk = Instantiate(this.chunk, transform);

                chunk.transform.position = new Vector3(this.chunk.GetComponent<MeshGenerator>().xSize * x, 0, this.chunk.GetComponent<MeshGenerator>().zSize * z);

                chunk.GetComponent<MeshGenerator>().index = index;
                chunk.GetComponent<MeshGenerator>().xIndex = x;
                chunk.GetComponent<MeshGenerator>().zIndex = z;
                chunk.GetComponent<MeshGenerator>().octaves = octaves;
                chunk.GetComponent<MeshGenerator>().origin = origin;

                index++;
            }
        }
    }
    IEnumerator waitPlaceObjs()
    {
        yield return new WaitForSeconds(1f);
        PlaceFoliage();
        SpawnVillages();
    }


    void PlaceFoliage()
    {
        foreach(FoliageSpawner foliage in Foliage)
        {
            for (int i = 0; i < foliage.count; i++)
            {
                ObjectSpawnData spawnPoint = GetSpawnPoint();

                var obj = Instantiate(foliage.prefab, spawnPoint.point, Quaternion.Euler(spawnPoint.hitNormal));
                float randomScale = Random.Range(0.9f, 2);
                obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            }
        }
    }

    void SpawnVillages()
    {
        for (int i = 0; i < villages.count; i++)
        {
            ObjectSpawnData villagePoint = GetSpawnPoint();
            var village = new GameObject("Village " + i);
            village.transform.position = villagePoint.point;
            for (int j = 0; j < Random.Range(3, 10); j++)
            {
                ObjectSpawnData housePoint = GetVillagePoint(village.transform.position);
                var house = Instantiate(villages.house, housePoint.point, Quaternion.LookRotation(housePoint.hitNormal));
                house.transform.parent = village.transform;

            }
        }
    }

    //public float FallOffMap(int x, int y)
    //{
    //    float falloff = falloffMap[x, y];

    //    return falloff;
    //}

    public static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    ObjectSpawnData GetSpawnPoint()
    {
        ObjectSpawnData data = new ObjectSpawnData();

        float xIslandSize = xSize * chunk.GetComponent<MeshGenerator>().xSize;
        float zIslandSize = zSize * chunk.GetComponent<MeshGenerator>().zSize;

        float range = ((xIslandSize + zIslandSize) / 2);
        float randX = Random.Range(0, range);
        float randZ = Random.Range(0, range);

        data.point = new Vector3(randX, 70, randZ);

        RaycastHit hit;

        if (Physics.Raycast(data.point, Vector3.down, out hit, 200, terrain) && hit.point.y > seaLevel)
        {
            data.point = new Vector3(randX, hit.point.y, randZ);
            data.hitNormal = new Vector3(hit.normal.z, hit.normal.x, hit.normal.y);
        }
        else
        {
            data = GetSpawnPoint();
        }

        return data;
    }

    ObjectSpawnData GetVillagePoint(Vector3 village)
    {
        ObjectSpawnData data = new ObjectSpawnData();

        float xIslandSize = xSize * chunk.GetComponent<MeshGenerator>().xSize;
        float zIslandSize = zSize * chunk.GetComponent<MeshGenerator>().zSize;

        float range = ((xIslandSize + zIslandSize) / 2);
        float randX = Random.Range(0, range);
        float randZ = Random.Range(0, range);

        data.point = new Vector3(randX, 70, randZ);

        RaycastHit hit;
        float distToVillage = Vector3.Distance(new Vector3(village.x, 0, village.z), new Vector3(randX, 0, randZ));

        if (Physics.Raycast(data.point, Vector3.down, out hit, 200, terrain) && hit.point.y > seaLevel && distToVillage <= villages.maxDist && distToVillage >= villages.minDist)
        {
            data.point = new Vector3(randX, hit.point.y, randZ);
            data.hitNormal = hit.normal;//new Vector3(hit.normal.z, hit.normal.x, hit.normal.y);
        }
        else
        {
            data = GetVillagePoint(village);
        }

        return data;
    }
}
[System.Serializable]
public class FoliageSpawner
{
    public string name;

    public GameObject prefab;

    public int count;
}
[System.Serializable]
public class VillageSpawner
{
    public GameObject house;

    public int count;

    public float maxDist = 10;

    public float minDist = 3;
}
public class ObjectSpawnData
{
    public Vector3 point;

    public Vector3 hitNormal;
}