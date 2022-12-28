using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : MonoBehaviour
{
    public Rigidbody rb;

    public float speed = 20;

    public float visionRadius = 100;

    public float attackRate = 3;

    public float attackForce = 20;

    public float damage = 100;

    public LayerMask objectLayer;

    public SkinnedMeshRenderer mesh;

    Vector3 moveAmount;

    Vector3 target;

    float attackDelay;

    // Start is called before the first frame update
    void Start()
    {
        //target = GetMovePoint();
    }

    // Update is called once per frame
    void Update()
    {
        See();
        if (transform.position.y <= -80)
        {
            transform.position = new Vector3(transform.position.x, 120, transform.position.z);
            rb.velocity = Vector3.zero;
            target = GetMovePoint();
        }
        float distToTarget = Vector3.Distance(transform.position, target);
        if (distToTarget <= 12)
        {
            target = GetMovePoint();
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

        point = new Vector3(randX + transform.position.x, 100, randZ + transform.position.z);

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
            if (other.gameObject != gameObject)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position);
                //if (other.TryGetComponent<Villager>(out Villager villager))
                //{
                //    //target = villager.transform.position;
                //    if (dist <= 40)
                //    {
                //        Attack(villager.GetComponent<Target>());
                //    }
                //}
                if (other.gameObject.name == "House(Clone)")
                {
                    Target structure = other.GetComponent<Target>();
                    target = other.transform.position;
                    if (dist <= 15)
                    {
                        Attack(structure);
                    }
                }
                //else if (other.TryGetComponent<Bunny>(out Bunny bunny))
                //{
                //    target = bunny.transform.position;
                //    if (dist <= 3)
                //    {
                //        Attack(bunny.GetComponent<Target>());
                //    }

                //}
                //else if (other.TryGetComponent<Fox>(out Fox fox))
                //{
                //    target = fox.transform.position;
                //    if (dist <= 3)
                //    {
                //        Attack(fox.GetComponent<Target>());
                //    }

                //}
                //else if (other.TryGetComponent<Food>(out Food food))
                //{
                //    target = food.transform.position;
                //    if (dist <= 5)
                //    {
                //        Silo silo = village.GetComponentInChildren<Silo>();
                //        silo.Add(food.foodType, 1);
                //        target = silo.transform.position;
                //        Destroy(food.gameObject);
                //    }

                //}
                //else if (other.TryGetComponent<Resource>(out Resource resource))
                //{
                //    target = resource.transform.position;
                //    if (dist <= 5)
                //    {
                //        SmithsHut smithsHut = village.GetComponentInChildren<SmithsHut>();
                //        smithsHut.Add(resource.resourceType, 1);
                //        target = smithsHut.transform.position;
                //        Destroy(resource.gameObject, 3);
                //    }
                //}
            }
        }
    }

    void Attack(Target attacked)
    {
        if (Time.time >= attackDelay)
        {
            Collider[] destructables = Physics.OverlapSphere(transform.position, visionRadius);

            foreach (Collider obj in destructables)
            {

                Rigidbody rb = obj.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    if (rb.isKinematic == true)
                    {
                        rb.isKinematic = false;
                        StartCoroutine(StablizeRBs(rb));
                    }
                    rb.AddExplosionForce(attackForce, transform.position, visionRadius, 10);
                }
                if (obj.TryGetComponent<Target>(out Target target))
                {
                    target.TakeDamage(30);
                }
            }
            attacked.TakeDamage(damage);
            attackDelay = Time.time + attackRate;
        }
    }

    IEnumerator StablizeRBs(Rigidbody rb)
    {
        yield return new WaitForSeconds(10);

        if (rb != null)
            rb.isKinematic = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
