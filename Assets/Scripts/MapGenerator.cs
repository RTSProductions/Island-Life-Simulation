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

    public float seaLevel = 5;

    //public float[,] falloffMap;

    [HideInInspector]
    public Transform midPoint;

    [HideInInspector]
    public float maxDist;

    public GameObject ocean;

    Vector2 origin;

    // Start is called before the first frame update
    void Start()
    {
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
        maxDist = Vector3.Distance(transform.position, midPoint.position);
        GenerateMap();
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
}