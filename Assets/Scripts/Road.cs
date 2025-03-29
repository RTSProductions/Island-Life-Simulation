using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    Vector3 a;
    Vector3 b;

    List<Waypoint> waypoints = new List<Waypoint>();

    public LayerMask notWalkable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateRoad(Vector3 pointA, Vector3 pointB)
    {
        a = pointA;
        b = pointB;
        CalculatePath();
    }

    public void CalculatePath()
    {
        GenerateWaypoints(a, b, 10);

        //for (int i = 0; i < (waypoints.Count - 1); i++)
        //{

        //    if (!checkUnbrokenPath(waypoints[i].transform.position, waypoints[i].next.transform.position))
        //    {
        //        Vector3 newPoint = GetUnbrokenPoint(waypoints[i], waypoints[i].next, _increments, 10);

        //        waypoints[i].transform.position = newPoint;
        //    }
        //}
    }



    void GenerateWaypoints(Vector3 a, Vector3 b, int iterations)
    {
        ClearList();

        int j = 0;

        float distanceToPoint = Vector3.Distance(a, b);

        float distChunk = distanceToPoint / iterations;

        Vector3 dir = (b - a).normalized;

        for (int i = 0; i < iterations; i++)
        {

            if (j == 0)
            {
                Vector3 point = transform.position + (dir * distChunk);

                Waypoint currentPoint = InstantiateWaypoint(point);

                currentPoint.notWalkable = notWalkable;

                waypoints.Add(currentPoint);
            }
            else if (j == 1)
            {
                Vector3 point = waypoints[i - 1].transform.position + (dir * distChunk);

                Waypoint currentPoint = InstantiateWaypoint(point);

                currentPoint.last = waypoints[i - 1];

                waypoints[i - 1].next = currentPoint;

                currentPoint.transform.LookAt(waypoints[i - 1].transform.position);

                currentPoint.notWalkable = notWalkable;

                waypoints.Add(currentPoint);
            }
            else if (j == 2)
            {
                Vector3 point = waypoints[i - 1].transform.position + (dir * distChunk);

                Waypoint currentPoint = InstantiateWaypoint(point);

                currentPoint.last = waypoints[i - 1];

                waypoints[i - 1].next = currentPoint;

                currentPoint.transform.LookAt(waypoints[i - 1].transform.position);

                currentPoint.notWalkable = notWalkable;

                waypoints.Add(currentPoint);

                j = 0;
            }
            j++;
        }
    }

    Waypoint InstantiateWaypoint(Vector3 position)
    {
        GameObject waypoint = new GameObject("Waypoint");

        waypoint.transform.position = position;

        Waypoint waypointComponent = waypoint.AddComponent<Waypoint>();

        waypointComponent.notWalkable = notWalkable;

        return waypointComponent;
    }

    void ClearList()
    {
        foreach (Waypoint point in waypoints)
        {
            Destroy(point.gameObject);
        }

        waypoints.Clear();
    }

    Vector3 MidPoint(Vector3 a, Vector3 b)
    {
        Vector3 midPoint = Vector3.zero;

        float x = 0;

        float y = 0;

        float z = 0;

        x = (a.x + b.x) / 2;

        y = (a.y + b.y) / 2;

        z = (a.z + b.z) / 2;

        midPoint = new Vector3(x, y, z);

        return midPoint;
    }
}

