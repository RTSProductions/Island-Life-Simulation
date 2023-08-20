using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyObject : MonoBehaviour
{
    public float waterDrag = 3;

    public float waterAngularDrag = 1;

    public float airDrag = 0;

    public float airAngularDrag = 0.05f;

    float floatingPower;

    Rigidbody rb;

    float seaLevel;

    bool underWater;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MapGenerator map = FindObjectOfType<MapGenerator>();
        floatingPower = map.floatingPower;
        seaLevel = map.seaLevel;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float diffrence = transform.position.y - seaLevel;

        if (diffrence <= 0)
        {
            rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(diffrence), transform.position, ForceMode.Force);
            if (!underWater)
            {
                underWater = true;
                Drag();
            }
        }
        else if (underWater)
        {
            underWater = false;
            Drag();
        }
    }

    void Drag()
    {
        if (underWater)
        {
            rb.drag = waterDrag;
            rb.angularDrag = waterAngularDrag;
        }
        else
        {
            rb.drag = airDrag;
            rb.angularDrag = airAngularDrag;
        }
    }
}
