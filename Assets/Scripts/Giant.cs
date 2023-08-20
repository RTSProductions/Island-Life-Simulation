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

    public LayerMask vision;

    public SkinnedMeshRenderer mesh;

    Vector3 moveAmount;

    Vector3 target;

    float attackDelay;

    // Start is called before the first frame update
    void Start()
    {
        //target = GetMovePoint();
        target = new Vector3(300, 40, 400);
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
        if (distToTarget <= 15)
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
        Collider[] others = Physics.OverlapSphere(transform.position, visionRadius, vision);

        //float lowestFoodDist = 10000;

        foreach (var other in others)
        {
            if (other.gameObject != gameObject)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position);

                if (other.gameObject.name == "House(Clone)")
                {
                    Target structure = other.GetComponent<Target>();
                    target = other.transform.position;
                    if (dist <= 20)
                    {
                        Attack(structure);
                    }
                }

            }
        }
    }

    void Attack(Target attacked)
    {
        if (Time.time >= attackDelay)
        {
            StartCoroutine(WaitAttackAnimation(attacked));
        }
    }

    IEnumerator StablizeRBs(Rigidbody rb)
    {
        yield return new WaitForSeconds(30);

        if (rb != null)
            rb.isKinematic = true;
    }

    IEnumerator WaitAttackAnimation(Target attacked)
    {
        GetComponentInChildren<Animator>().Play("Giant Stomp");

        yield return new WaitForSeconds(1);

        Collider[] destructables = Physics.OverlapSphere(transform.position, visionRadius / 2);

        foreach (Collider obj in destructables)
        {
            if (obj.gameObject != gameObject)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    if (rb.isKinematic == true)
                    {
                        rb.isKinematic = false;
                        StartCoroutine(StablizeRBs(rb));
                    }
                    rb.AddExplosionForce(attackForce, transform.position, visionRadius / 2, 20);
                }
                if (obj.TryGetComponent<Target>(out Target target))
                {
                    target.TakeDamage(20);
                }
            }
        }
        if (attacked != null)
            attacked.TakeDamage(damage);
        attackDelay = Time.time + attackRate;
    }

    private void OnTriggerEnter(Collider other)
    {

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (rb.isKinematic == true)
            {
                rb.isKinematic = false;
                StartCoroutine(StablizeRBs(rb));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
