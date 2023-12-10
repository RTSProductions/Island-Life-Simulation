using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public Rigidbody rb;

    public Occupation occupation;

    public int id;

    public float speed = 8;

    public float visionRadius = 30;

    public float attackRate = 0.5f;

    public float damage = 10;

    [HideInInspector]
    public float seaLevel;

    public LayerMask obstacleAvoidance;

    public LayerMask vision;

    public LayerMask giant;

    public Village village;

    public City city;

    public SkinnedMeshRenderer mesh;

    Vector3 moveAmount;

    Vector3 target;

    Transform player;

    float attackDelay;

    // Start is called before the first frame update
    void Start()
    {
        target = village.GetVillagePoint();
        seaLevel = FindObjectOfType<MapGenerator>().seaLevel;
    }

    // Update is called once per frame
    void Update()
    {
        See();
        if (transform.position.y <= -80)
        {
            transform.position = new Vector3(transform.position.x, 70, transform.position.z);
            rb.velocity = Vector3.zero;
            if (city == null)
            {
                target = village.GetVillagePoint();
            }
            else
            {
                target = city.GetVillagePoint();
            }
        }
        float distToTarget = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.x, 0, target.z));
        if (distToTarget <= 2)
        {
            if (LightingManager.night == true)
            {
                if (city == null)
                {
                    target = village.GetVillagePoint();
                }
                else
                {
                    target = city.GetVillagePoint();
                }
            }
            else
            {
                target = GetMovePoint(0);
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
        //Ray forward = new Ray(transform.position + (Vector3.up * 0.8f), transform.forward);
        //Ray right = new Ray(transform.position + (Vector3.up * 0.8f), transform.right);
        //Ray left = new Ray(transform.position + (Vector3.up * 0.8f), -transform.right);
        //if (Physics.Raycast(forward, 0.32f * 2, obstacleAvoidance))
        //{
        //    if (!Physics.Raycast(right, 0.32f * 2, obstacleAvoidance))
        //    {
        //        Vector3 stirAmount = transform.right.normalized;
        //        //Vector3 movementDir = (transform.position + stirAmount - transform.position).normalized;
        //        moveAmount = stirAmount * speed;
        //    }
        //    else if (!Physics.Raycast(left, 0.32f * 2, obstacleAvoidance))
        //    {
        //        Vector3 stirAmount = -transform.right.normalized;
        //        //Vector3 movementDir = (transform.position + stirAmount - transform.position).normalized;
        //        moveAmount = stirAmount * speed;
        //    }
        //}
        rb.MovePosition(transform.position + moveAmount * Time.fixedDeltaTime);
    }

    void FaceTarget()
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
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

        if (Physics.Raycast(point, Vector3.down, out hit, 200) && hit.point.y >= seaLevel + 1.5f)
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
        Collider[] others = Physics.OverlapSphere(transform.position, visionRadius, vision);

        Collider[] giants = Physics.OverlapSphere(transform.position, visionRadius * 10, giant);

        float lowestFoodDist = 10000;

        foreach (var other in others)
        {
            if (other.gameObject != gameObject)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position);
                if (other.gameObject.name == "Giant(Clone)")
                {
                    target = other.transform.position;
                    if (dist <= 12)
                    {
                        Attack(other.GetComponent<Target>());
                    }
                    break;
                }
                else if (other.gameObject.name == "Villager(Clone)" && other.transform.parent != village.transform)
                {
                    //if (occupation == Occupation.basic || occupation == Occupation.knight)
                    //{
                    target = other.transform.position;
                    if (dist <= 3)
                    {
                        Attack(other.GetComponent<Target>());
                    }
                    //}
                }
                else if (other.gameObject.name == "Silo(Clone)" && other.transform.parent != village.transform || other.gameObject.name == "House(Clone)" && other.transform.parent != village.transform && other.transform.parent.parent != village.transform || other.gameObject.name == "Smiths Hut(Clone)" && other.transform.parent != village.transform || other.gameObject.name == "Fence" && other.transform.parent.parent != village.transform)
                {
                    if (occupation == Occupation.basic || occupation == Occupation.knight)
                    {
                        target = other.transform.position;
                        if (dist <= 5)
                        {
                            Target structure = other.GetComponent<Target>();
                            if (structure.health <= damage)
                            {
                                if (other.gameObject.name == "Silo(Clone)" || other.gameObject.name == "Smiths Hut(Clone)")
                                {
                                    TakeOver(other.transform.parent.GetComponent<Village>());
                                    Attack(structure);
                                    village.AddCity(other.transform.parent);
                                }
                            }
                            Attack(structure);
                        }
                    }
                }
                else if (other.gameObject.name == "Bunny(Clone)")
                {
                    if (occupation == Occupation.basic || occupation == Occupation.hunterGatherer)
                    {
                        target = other.transform.position;
                        if (dist <= 3)
                        {
                            Attack(other.GetComponent<Target>());
                        }
                    }

                }
                else if (other.gameObject.name == "Fox(Clone)")
                {
                    if (occupation == Occupation.basic || occupation == Occupation.hunterGatherer)
                    {
                        target = other.transform.position;
                        if (dist <= 3)
                        {
                            Attack(other.GetComponent<Target>());
                        }
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
                    bool continued = false;

                    switch (resource.resourceType)
                    {
                        case ResourceType.Wood:
                            continued = occupation == Occupation.lumberjack;
                            break;
                        case ResourceType.Stone:
                            continued = occupation == Occupation.miner;
                            break;
                    }

                    if (continued == true)
                    {
                        target = resource.transform.position;
                        if (dist <= 5)
                        {
                            SmithsHut smithsHut = village.GetComponentInChildren<SmithsHut>();
                            smithsHut.Add(resource.resourceType, 1);
                            target = smithsHut.transform.position;
                            Destroy(resource.gameObject);
                        }
                    }
                }
                else if (other.name == "Fern Farn(Clone)" && occupation == Occupation.farmer && other.transform.parent == village.transform)
                {
                    target = other.transform.position;
                    if (dist <= 5)
                    {
                        FernFarm farm = other.GetComponent<FernFarm>();
                        if (farm.currentFernAge >= 1)
                        {
                            farm.Harvest();
                            Silo silo = village.GetComponentInChildren<Silo>();
                            silo.Add(food.foodType, 15);
                        }
                    }
                }
            }
        }
        foreach (var other in giants)
        {
            target = other.transform.position;
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist <= 12)
            {
                Attack(other.GetComponent<Target>());
            }
        }
    }

    void TakeOver(Village newTeritory)
    {
        Villager[] newCitizens = newTeritory.gameObject.GetComponentsInChildren<Villager>();
        foreach (Villager villager in newCitizens)
        {
            villager.village = village;
            villager.transform.parent = this.transform.parent;
            Debug.Log("New citizen");
        }
        Silo silo = newTeritory.GetComponentInChildren<Silo>();
        SmithsHut smithsHut = newTeritory.GetComponentInChildren<SmithsHut>();

        if (silo != null)
        {
            Silo ourSilo = village.GetComponentInChildren<Silo>();
            for (int i = 0; i < ourSilo.storage.Length; i++)
            {
                if (ourSilo.storage[i].foodType == silo.storage[i].foodType)
                {
                    ourSilo.storage[i].amountSotred += silo.storage[i].amountSotred;
                }
            }
            Destroy(silo.gameObject);
        }
        if (smithsHut != null)
        {
            SmithsHut smithsHutSilo = village.GetComponentInChildren<SmithsHut>();
            for (int i = 0; i < smithsHutSilo.storage.Length; i++)
            {
                if (smithsHutSilo.storage[i].resourceType == smithsHut.storage[i].resourceType)
                {
                    smithsHutSilo.storage[i].amountSotred += smithsHut.storage[i].amountSotred;
                }
            }
            Destroy(smithsHut.gameObject);
        }
    }

    void Attack(Target attacked)
    {
        if (Time.time >= attackDelay)
        {
            attacked.TakeDamage(damage);
            attackDelay = Time.time + attackRate;
            GetComponentInChildren<Animator>().Play("Guy Attacking");
        }
    }
    public void Occupate(Occupation newOccupation, float maxHealth, float NewDamage)
    {
        int j = village.GetJobID(occupation);
        if (occupation != Occupation.basic)
        {
            if (village.jobs[j].occupents.Contains(this))
            {
                village.jobs[j].occupents.Remove(this);
            }
        }
        Target target = GetComponent<Target>();
        occupation = newOccupation;
        target.maxHealth = maxHealth;
        target.Heal(maxHealth);
        damage = NewDamage;
        Debug.Log("Just got " + newOccupation + " " + occupation);
    }

    public void GoTo(Vector3 position)
    {
        target = position;
    }

}
public enum Occupation
{
    basic, lumberjack, miner, farmer, hunterGatherer, knight,
}
