using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FernFarm : MonoBehaviour
{
    public Transform[] objsToOrient;

    public GameObject[] Ferns;

    public float currentFernAge = 0;

    MapGenerator generator;

    // Start is called before the first frame update
    void Start()
    {
        generator = FindObjectOfType<MapGenerator>();

        StartCoroutine(waitPlaceObjs());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFernAge < 1)
        {
            currentFernAge += Time.deltaTime * 1 / 200;
            foreach (GameObject plant in Ferns)
            {
                plant.transform.localScale = Vector3.one * currentFernAge;
            }
        }
        if (transform.childCount <= 15)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator waitPlaceObjs()
    {
        yield return new WaitForSeconds(1f);

        ObjectSpawnData pos = GroundPoint(transform.position);
        transform.position = pos.point;

        foreach (Transform obj in objsToOrient)
        {
            obj.position = new Vector3(obj.position.x, 210, obj.position.z);
            ObjectSpawnData data = GroundPoint(obj.position);
            obj.position = data.point;
        }
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

    public void Harvest()
    {
        foreach (GameObject plant in Ferns)
        {
            plant.transform.localScale = Vector3.one * 0;
        }
        currentFernAge = 0;
    }
}
