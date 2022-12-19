using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public Rigidbody rb;

    public float speed = 8;

    public float visionRadius = 30;

    public LayerMask obstacleAvoidance;

    public Village village;

    public SkinnedMeshRenderer mesh;

    Vector3 moveAmount;

    Vector3 target;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        target = village.GetVillagePoint();
    }

    // Update is called once per frame
    void Update()
    {
        See();
        float distToTarget = Vector3.Distance(transform.position, target);
        if (distToTarget <= 5)
        {
            if (LightingManager.night == true)
            {
                target = village.GetVillagePoint();
            }
            else
            {
                target = GetMovePoint();
            }
        }

        Vector3 movementDir = (target - transform.position).normalized;
        moveAmount = movementDir * speed;
    }

    private void FixedUpdate()
    {
        Move();
        FaceTarget();
    }

    void Move()
    {
        Ray forward = new Ray(transform.position + (Vector3.up * 0.8f), transform.forward);
        Ray right = new Ray(transform.position + (Vector3.up * 0.8f), transform.right);
        Ray left = new Ray(transform.position + (Vector3.up * 0.8f), -transform.right);
        if (Physics.Raycast(forward, 0.32f * 2, obstacleAvoidance))
        {
            if (!Physics.Raycast(right, 0.32f * 2, obstacleAvoidance))
            {
                Vector3 stirAmount = transform.right;
                //Vector3 movementDir = (transform.position + stirAmount - transform.position).normalized;
                moveAmount = stirAmount * speed;
            }
            else if (!Physics.Raycast(left, 0.32f * 2, obstacleAvoidance))
            {
                Vector3 stirAmount = -transform.right;
                //Vector3 movementDir = (transform.position + stirAmount - transform.position).normalized;
                moveAmount = stirAmount * speed;
            }
        }
        rb.MovePosition(transform.position + moveAmount * Time.fixedDeltaTime);
    }

    void FaceTarget()
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    Vector3 GetMovePoint()
    {
        Vector3 point;

        float randX = Random.Range(-visionRadius * 3, visionRadius * 3);
        float randZ = Random.Range(-visionRadius * 3, visionRadius * 3);

        point = new Vector3(randX + transform.position.x, 70, randZ + transform.position.z);

        RaycastHit hit;

        if (Physics.Raycast(point, Vector3.down, out hit, 200))
        {
            point = new Vector3(randX + transform.position.x, hit.point.y, randZ + transform.position.z);
        }
        else
        {
            point = GetMovePoint();
        }

        return point;

    }

    void See()
    {
        Collider[] others = Physics.OverlapSphere(transform.position, visionRadius);

        //float lowestFoodDist = 10000;

        foreach (var other in others)
        {
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (other.gameObject.name == "Silo(Clone)" && other.transform.parent != this.transform.parent || other.gameObject.name == "House(Clone)" && other.transform.parent != this.transform.parent || other.gameObject.name == "Smiths Hut(Clone)" && other.transform.parent != this.transform.parent)
            {
                target = other.transform.position;
                if (dist <= 5)
                {
                    if (other.gameObject.name == "Silo(Clone)" || other.gameObject.name == "Smiths Hut(Clone)")
                    {
                        TakeOver(other.transform.parent.GetComponent<Village>());
                        Destroy(other.transform.parent.gameObject);
                    }
                    Destroy(other.gameObject);
                }
            }
            else if (other.TryGetComponent<Food>(out Food food))
            {
                target = food.transform.position;
                if (dist <= 5)
                {
                    Silo silo = village.GetComponentInChildren<Silo>();
                    silo.Add(food.foodType, 1);
                    target = silo.transform.position;
                    Destroy(food.gameObject);
                }

            }
            else if (other.TryGetComponent<Resource>(out Resource resource))
            {
                target = resource.transform.position;
                if (dist <= 5)
                {
                    SmithsHut smithsHut = village.GetComponentInChildren<SmithsHut>();
                    smithsHut.Add(resource.resourceType, 1);
                    target = smithsHut.transform.position;
                    Destroy(resource.gameObject, 3);
                }
            }
        }
    }

    void TakeOver(Village newTeritory)
    {
        Villager[] newCitizens = newTeritory.GetComponentsInChildren<Villager>();
        foreach (Villager villager in newCitizens)
        {
            villager.village = village;
            villager.mesh.sharedMaterial = village.SkinColor;
        }
        for (int i = 0; i < newTeritory.transform.childCount; i++)
        {
            newTeritory.transform.GetChild(i).parent = transform.parent;
        }
    }

}
