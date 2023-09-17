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

    public float floatingPower = 15;

    [Range(0, 1)]
    public float foliageMask = 0.25f;

    [HideInInspector]
    public float[,] falloffMap;


    [HideInInspector]
    public float maxDist;

    [HideInInspector]
    public int vertexID = 0;

    [HideInInspector]
    public int xVertexID = 0;

    [HideInInspector]
    public int zVertexID = 0;

    public LayerMask terrain;

    //public float[,] falloffMap;

    [HideInInspector]
    public Transform midPoint;

    public GameObject ocean;

    public FoliageSpawner[] Foliage;

    public FoliageSpawner[] Animals;

    public VillageSpawner villages;

    Vector2 origin;

    Vector2 forestOrigin;

    // Start is called before the first frame update
    void Start()
    {
        if (randomizeSeed == true)
        {
            seed = Random.Range(0, 999999);
        }
        if (randomizeSeaLevel == true)
        {
            seaLevel = Random.Range(5, 36);
        }
        int forestSeed = Random.Range(-seed, seed);
        origin = new Vector2(Mathf.Sqrt(seed), Mathf.Sqrt(seed));
        forestOrigin = new Vector2(Mathf.Sqrt(seed + forestSeed), Mathf.Sqrt(seed + forestSeed));
        int xIslandSize = xSize * chunk.GetComponent<MeshGenerator>().xSize;
        int zIslandSize = zSize * chunk.GetComponent<MeshGenerator>().zSize;

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
        MeshGenerator chunkPart = chunk.GetComponent<MeshGenerator>();
        falloffMap = FalloffMapGenerator.GenerateFalloffMap((xSize * chunkPart.xSize) + 1, (zSize * chunkPart.zSize) + 1);
        int index = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var chunk = Instantiate(this.chunk, transform);

                chunk.transform.position = new Vector3(chunkPart.xSize * x, 0, chunkPart.zSize * z);

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
        yield return new WaitForSeconds(1f);
        PlaceAnimals();
        yield return new WaitForSeconds(1f);
        SpawnVillages();
    }


    void PlaceFoliage()
    {
        foreach(FoliageSpawner foliage in Foliage)
        {
            for (int i = 0; i < foliage.count; i++)
            {
                Vector3 spawnPoint = GetSpawnNoiseMaskPoint();

                var obj = Instantiate(foliage.prefab, spawnPoint, Quaternion.identity);
                float randomScale = Random.Range(0.9f, 2);
                obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            }
        }
    }

    void PlaceAnimals()
    {
        foreach (FoliageSpawner animal in Animals)
        {
            for (int i = 0; i < animal.count; i++)
            {
                ObjectSpawnData spawnPoint = GetSpawnPoint();

                var obj = Instantiate(animal.prefab, spawnPoint.point, Quaternion.identity);
            }
        }
    }

    void SpawnVillages()
    {
        for (int i = 0; i < villages.count; i++)
        {
            ObjectSpawnData villagePoint = GetSpawnPoint();

            int randomName = Random.Range(0, villages.possibleNames.Length - 1);

            var village = new GameObject(villages.possibleNames[randomName]);
            village.transform.position = villagePoint.point;
            Village vil = village.AddComponent<Village>();
            vil.generator = this;
            int index = Random.Range(0, villages.skinColors.Length - 1);
            vil.SkinColor = villages.skinColors[index];
            vil.citizens = new List<Transform>();
            for (int j = 0; j < Random.Range(3, 10); j++)
            {
                ObjectSpawnData housePoint = GetVillagePoint(village.transform.position);
                var house = Instantiate(villages.house, housePoint.point, Quaternion.LookRotation(housePoint.hitNormal));
                house.transform.parent = village.transform;
            }
            for (int j = 0; j < villages.requiredStructures.Length; j++)
            {
                ObjectSpawnData structurePoint = GetVillagePoint(village.transform.position);
                var structure = Instantiate(villages.requiredStructures[j], structurePoint.point, Quaternion.LookRotation(structurePoint.hitNormal));
                structure.transform.parent = village.transform;
            }
            for (int j = 0; j < villages.population; j++)
            {
                Vector3 villagerSpawnPoint = vil.GetVillagePoint();
                var villager = Instantiate(villages.villager, villagerSpawnPoint, Quaternion.identity);
                villager.transform.parent = village.transform;
                villager.GetComponent<Villager>().village = vil;
                villager.GetComponent<Villager>().mesh.sharedMaterial = vil.SkinColor;
                villager.GetComponent<Villager>().id = i + j;
                vil.citizens.Add(villager.transform);
            }
        }
    }

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

    public ObjectSpawnData GetVillagePoint(Vector3 village)
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

    Vector3 GetSpawnNoiseMaskPoint()
    {
        Vector3 point;

        float xIslandSize = xSize * chunk.GetComponent<MeshGenerator>().xSize;
        float zIslandSize = zSize * chunk.GetComponent<MeshGenerator>().zSize;

        float range = ((xIslandSize + zIslandSize) / 2);
        float randX = Random.Range(0, range);
        float randZ = Random.Range(0, range);

        point = new Vector3(randX, 70, randZ);

        float forestChance = Mathf.PerlinNoise((point.x + forestOrigin.x) * .006f, (point.z + forestOrigin.y) * .006f) * 2;

        float valueBarrier = foliageMask * forestChance;

        RaycastHit hit;

        float val = Random.value;

        if (forestChance >= foliageMask)
        {
            if (Physics.Raycast(point, Vector3.down, out hit, 200, terrain) && hit.point.y > seaLevel)
            {
                point = new Vector3(randX, hit.point.y, randZ);
            }
            else
            {
                point = GetSpawnNoiseMaskPoint();
            }
        }
        else
        {
            point = GetSpawnNoiseMaskPoint();
        }

        return point;
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
    public GameObject villager;

    public GameObject house;

    public GameObject[] requiredStructures;

    public string[] possibleNames;

    public int count;

    public int population = 7;

    public float maxDist = 10;

    public float minDist = 3;

    public float waypointRange = 10;

    public Material[] skinColors;
}
public class ObjectSpawnData
{
    public Vector3 point;

    public Vector3 hitNormal;
}