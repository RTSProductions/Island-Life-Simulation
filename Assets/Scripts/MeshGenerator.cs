using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    public int index = 0;

    [HideInInspector]
    public int xIndex;
    [HideInInspector]
    public int zIndex;

    [HideInInspector]
    public int octaves;
    [HideInInspector]
    public Vector2 origin;

    MapGenerator generator;

    // Start is called before the first frame update
    void Start()
    {
        generator = FindObjectOfType<MapGenerator>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = 0;
                float opacity = 1;
                float nosieScale = .006f;
                for (int o = 0; o < octaves; o++)
                {
                    float currentY = Mathf.PerlinNoise((x + transform.position.x + origin.x) * nosieScale, (z + transform.position.z + origin.y) * nosieScale) * 50;
                    currentY /= opacity;
                    y += currentY;
                    opacity *= 2;
                    nosieScale *= 2;
                }
                float distanceFromMiddleFallOff = Vector3.Distance(new Vector3(transform.position.x + x, 0, transform.position.z + z), generator.midPoint.position) / 90;
                distanceFromMiddleFallOff = Mathf.Pow(distanceFromMiddleFallOff, 2);
                //distanceFromMiddleFallOff /= 10;
                y -= distanceFromMiddleFallOff;

                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        
    }

    /* Optionally, draw spheres at each vertex
    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i=0; i<vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
    */

}