using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    public Rigidbody rb;

    public GameObject food;

    public float speed = 11;

    public float visionRadius = 20;

    public float reproductionDelay = 90;

    public float attackRate = 0.5f;

    public float damage = 15;

    public LayerMask obstacleAvoidance;

    Vector3 moveAmount;

    Vector3 target;

    [HideInInspector]
    public float reproductionTime = 0;

    float attackDelay;

    // Start is called before the first frame update
    void Start()
    {
        reproductionTime = Random.Range(-reproductionDelay, 1000) + reproductionDelay;
    }

    // Update is called once per frame
    void Update()
    {
        See();
        if (Time.time >= reproductionTime)
        {
            Reproduce();
        }
        if (transform.position.y <= -20)
        {
            transform.position = new Vector3(transform.position.x, 70, transform.position.z);
            target = GetMovePoint(0);
            rb.velocity = Vector3.zero;
        }
        float distToTarget = Vector3.Distance(transform.position, target);
        if (distToTarget <= 3)
        {
            target = GetMovePoint(0);
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
        Ray forward = new Ray(transform.position + (Vector3.up * 0.5f), transform.forward);
        Ray right = new Ray(transform.position + (Vector3.up * 0.5f), transform.right);
        Ray left = new Ray(transform.position + (Vector3.up * 0.5f), -transform.right);
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

    Vector3 GetMovePoint(int tries)
    {
        Vector3 point;

        float randX = Random.Range(-visionRadius * 3, visionRadius * 3);
        float randZ = Random.Range(-visionRadius * 3, visionRadius * 3);

        point = new Vector3(randX + transform.position.x, 70, randZ + transform.position.z);

        RaycastHit hit;

        if (tries >= 10)
        {
            return point;
        }

        if (Physics.Raycast(point, Vector3.down, out hit, 200))
        {
            point = new Vector3(randX + transform.position.x, hit.point.y, randZ + transform.position.z);
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
        Collider[] others = Physics.OverlapSphere(transform.position, visionRadius);

        //float lowestFoodDist = 10000;

        foreach (var other in others)
        {
            if (other.gameObject != gameObject)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position); if (other.TryGetComponent<Giant>(out Giant giant))
                {
                    target = new Vector3(-giant.transform.position.x, giant.transform.position.y, -giant.transform.position.z);
                }
                else if (other.TryGetComponent<Villager>(out Villager villager))
                {
                    target = new Vector3(-villager.transform.position.x, villager.transform.position.y, -villager.transform.position.z);

                }
                else if (other.TryGetComponent<Food>(out Food food) && food.foodType == FoodType.Bunny)
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
                else if (other.TryGetComponent<Bunny>(out Bunny bunny))
                {
                    target = bunny.transform.position;
                    if (dist <= 3)
                    {
                        Attack(bunny.GetComponent<Target>());
                    }
                }
            }
        }
    }

    void Reproduce()
    {
        var child = Instantiate(this.gameObject, transform.position, Quaternion.identity);
        child.GetComponent<Fox>().reproductionTime = Random.Range(-reproductionDelay, 1000) + reproductionDelay;
        reproductionTime = Time.time + reproductionDelay;
    }

    void Attack(Target attacked)
    {
        if (Time.time >= attackDelay)
        {
            attacked.TakeDamage(damage);
            attackDelay = Time.time + attackRate;
        }
    }
}
