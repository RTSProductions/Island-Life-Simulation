using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    [HideInInspector]
    public MapGenerator generator;

    public string cityName;

    public Material SkinColor;

    public List<Transform> citizens; 

    float timeToRequest = 0;
    float requestDelay = 30;

    // Start is called before the first frame update
    void Start()
    {
        generator = FindAnyObjectByType<MapGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeToRequest)
        {
            if (transform.childCount < 5) 
            {
                transform.parent.GetComponent<Village>().RequestConstruction(ConstructionType.house, transform);
                timeToRequest = Time.time + requestDelay;
            }
        }
    }

    public Vector3 GetVillagePoint()
    {
        Vector3 point;

        float randX = Random.Range(-generator.villages.waypointRange, generator.villages.waypointRange);
        float randZ = Random.Range(-generator.villages.waypointRange, generator.villages.waypointRange);

        point = new Vector3(randX + transform.position.x, 70, randZ + transform.position.z);

        RaycastHit hit;

        if (Physics.Raycast(point, Vector3.down, out hit, 200) && hit.point.y > generator.seaLevel)
        {
            point = new Vector3(randX + transform.position.x, hit.point.y, randZ + transform.position.z);
        }
        else
        {
            point = GetVillagePoint();
        }

        return point;
    }
}
