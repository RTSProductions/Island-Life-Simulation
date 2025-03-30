using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour
{
    public Rigidbody rb;

    public GameObject food;

    public float speed = 6;

    public float visionRadius = 15;

    public float reproductionDelay = 130;

    public LayerMask obstacleAvoidance;

    public LayerMask vision;

    Vector3 moveAmount;

    Vector3 target;

    [HideInInspector]
    public float reproductionTime = 0;

    float seaLevel;

    // Start is called before the first frame update
    void Start()
    {
        reproductionTime = Random.Range(-reproductionDelay, 1000) + reproductionDelay;
        target = new Vector3(80, 20, 700);
        seaLevel = FindObjectOfType<MapGenerator>().seaLevel;
    }

    // Update is called once per frame
    void Update()
    {
        See();
        if (Time.time >= reproductionTime)
        {
            //Reproduce();
        }
        if (transform.position.y <= -80)
        {
            transform.position = new Vector3(transform.position.x, 70, transform.position.z);
            target = GetMovePoint(0);
            rb.linearVelocity = Vector3.zero;
        }
        float distToTarget = Vector3.Distance(transform.position, target);
        if (distToTarget <= 3)
        {
            target = GetMovePoint(0);
        }

        Vector3 noise = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        Vector3 movementDir = (target - transform.position + noise).normalized;
        moveAmount = movementDir * speed;
    }

    private void FixedUpdate()
    {
        Move();
        FaceTarget();
    }

    void Move()
    {
        Vector3 movementDir = (target - transform.position).normalized;
    
        Ray forwardRay = new Ray(transform.position + Vector3.up * 0.8f, movementDir);
        RaycastHit hit;
    
        if (Physics.Raycast(forwardRay, out hit, 1.5f, obstacleAvoidance))
        {
            Vector3 avoidanceDir = Vector3.zero;

            if (!Physics.Raycast(transform.position + Vector3.up * 0.8f, transform.right, 1.5f, obstacleAvoidance))
            {
                avoidanceDir += transform.right;
            }
            else if (!Physics.Raycast(transform.position + Vector3.up * 0.8f, -transform.right, 1.5f, obstacleAvoidance))
            {
                avoidanceDir -= transform.right;
            }

            if (avoidanceDir != Vector3.zero)
            {
                movementDir = (movementDir + avoidanceDir).normalized;
            }
        }

        moveAmount = movementDir * speed;
        rb.MovePosition(transform.position + moveAmount * Time.fixedDeltaTime);
    }

    void FaceTarget()
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    Vector3 GetMovePoint(int tries)
    {
        Vector3 point;

        float randX = Random.Range(-visionRadius * 3, visionRadius * 3);
        float randZ = Random.Range(-visionRadius * 3, visionRadius * 3);

        point = new Vector3(randX + transform.position.x, 70, randZ + transform.position.z);

        RaycastHit hit;

        if (point.x <= -1)
        {
            point = new Vector3(1, point.y, point.z);
        }
        if (point.z <= -1)
        {
            point = new Vector3(point.x, point.y, 1);
        }

        if (tries >= 10)
        {
            return new Vector3(randX + transform.position.x, 50, randZ + transform.position.z);
        }

        if (Physics.Raycast(point, Vector3.down, out hit, 200))
        {
            float groundY = Mathf.Max(hit.point.y, seaLevel);
            point = new Vector3(randX + transform.position.x, groundY + 1f, randZ + transform.position.z);
        }
        else
        {
            tries++;
            point = GetMovePoint(tries);
        }

        return point;

    }

    void See()
    {
        Collider[] others = Physics.OverlapSphere(transform.position, visionRadius, vision);

        //float lowestFoodDist = 10000;

        foreach (var other in others)
        {
            if (other.gameObject != gameObject)
            {
                //if (runPos.y <= seaLevel)
                //{
                //    runPos = GetMovePoint(0);
                //}
                float dist = Vector3.Distance(transform.position, other.transform.position);
                if (other.gameObject.name == "Giant(Clone)")
                {
                    Vector3 runDirection = (transform.position - other.transform.position).normalized;
                    float escapeDistance = 10f; 
                
                    target = transform.position + (runDirection * escapeDistance);
                    return;
                }
                else if (other.gameObject.name == "Villager(Clone)")
                {
                    Vector3 runDirection = (transform.position - other.transform.position).normalized;
                    float escapeDistance = 10f; 
                
                    target = transform.position + (runDirection * escapeDistance);
                    return;

                }
                else if (other.gameObject.name == "Fox(Clone)")
                {
                    Vector3 runDirection = (transform.position - other.transform.position).normalized;
                    float escapeDistance = 10f; 
                
                    target = transform.position + (runDirection * escapeDistance);
                    return;

                }
                else if (other.TryGetComponent<Food>(out Food food))
                {
                    if (food.foodType == FoodType.Apple || food.foodType == FoodType.Fern)
                    {
                        target = food.transform.position;
                        if (dist <= 3)
                        {
                            Destroy(food.gameObject);
                            Target targetComp = GetComponent<Target>();

                            if (targetComp.health <= 100 - 15)
                            {
                                targetComp.health += 15;
                            }
                        }
                    }

                }
            }
        }
    }

    // void Reproduce()
    // {
    //     var child = Instantiate(this.gameObject, transform.position, Quaternion.identity);
    //     child.GetComponent<Bunny>().reproductionTime = Random.Range(30, 1000) + reproductionDelay;
    //     reproductionTime += Time.time + reproductionDelay;
    //     child.gameObject.name = this.gameObject.name;
    // }
}
