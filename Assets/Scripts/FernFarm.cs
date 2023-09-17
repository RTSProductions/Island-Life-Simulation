using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FernFarm : MonoBehaviour
{
    public Transform[] objsToOrient;

    MapGenerator generator;

    // Start is called before the first frame update
    void Start()
    {
        generator = FindObjectOfType<MapGenerator>();

        foreach (Transform obj in objsToOrient)
        {
            ObjectSpawnData data = GroundPoint(obj.position);
            obj.position = data.point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ObjectSpawnData GroundPoint(Vector3 position)
    {
        ObjectSpawnData data = new ObjectSpawnData();
        data.point = new Vector3(position.x, 200, position.z);

        RaycastHit hit;

        if (Physics.Raycast(data.point, Vector3.down, out hit, 200))
        {
            data.point = hit.point;
        }
        else
        {
            data.point = position;
        }

        return data;
    }
}
