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

    Vector2 origin;

    // Start is called before the first frame update
    void Start()
    {
        origin = new Vector2(Mathf.Sqrt(seed), Mathf.Sqrt(seed));
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
                chunk.GetComponent<MeshGenerator>().octaves = octaves;
                chunk.GetComponent<MeshGenerator>().origin = origin;

                index++;
            }
        }
    }
}