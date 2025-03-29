using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [HideInInspector]
    public bool isFirst = false;
    [HideInInspector]
    public bool isLast = false;
    //[HideInInspector]
    public Waypoint last;
    //[HideInInspector]
    public Waypoint next;
    [HideInInspector]
    public LayerMask notWalkable;

    public static float checkRadius = 0.2f;

    public bool isThroughObj = false;

    // Start is called before the first frame update
    void Start()
    {
        GroundSelf();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckViability()
    {
        Collider[] Obsticles = Physics.OverlapSphere(transform.position, checkRadius);

        return Physics.CheckSphere(transform.position, checkRadius, notWalkable);
    }


    public bool CheckViability(Vector3 direction, float distance)
    {
        Collider[] Obsticles = Physics.OverlapSphere(transform.position + (direction * distance), checkRadius);

        return Obsticles.Length < 1;
    }

    void GroundSelf()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 200))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + checkRadius, transform.position.z);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, checkRadius);

        if (next != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, next.transform.position);
        }
    }
}
