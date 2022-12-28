using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleTree : MonoBehaviour
{
    public GameObject apple;

    public LayerMask ground;

    public float range = 2;

    public float appleSpawnRate = 0.15f;

    float appleSpawnDelay;

    // Start is called before the first frame update
    void Start()
    {
        appleSpawnDelay = Random.Range(10, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= appleSpawnDelay)
        {
            Vector3 point = GetSpawnPoint(0);
            var newApple = Instantiate(apple, point, Quaternion.identity, transform);
            appleSpawnDelay = Time.time + appleSpawnRate;
        }
    }

    Vector3 GetSpawnPoint(int tries)
    {
        Vector3 point;

        float randX = Random.Range(-range, range);
        float randZ = Random.Range(-range, range);

        point = new Vector3(randX + transform.position.x, 70, randZ + transform.position.z);

        RaycastHit hit;

        if (tries >= 10)
        {
            return point;
        }

        if (Physics.Raycast(point, Vector3.down, out hit, 200, ground))
        {
            point = new Vector3(randX + transform.position.x, hit.point.y, randZ + transform.position.z);
        }
        else
        {
            tries++;
            point = GetSpawnPoint(tries);
        }

        return point;

    }
}
