using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    [HideInInspector]
    public MapGenerator generator;

    public string villageName;

    public int societyLevel = 0;

    public Material SkinColor;

    public List<Advancement> advancements;

    public List<Transform> cities;

    public List<Transform> farms;

    public List<Transform> citizens;

    public List<Job> jobs;

    // Start is called before the first frame update
    void Start()
    {
        advancements = new List<Advancement>();
        cities = new List<Transform>();
        farms = new List<Transform>();

        jobs = new List<Job>(0);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetVillagePoint()
    {
        Vector3 point;

        float randX = Random.Range(-generator.villages.waypointRange, generator.villages.waypointRange);
        float randZ = Random.Range(-generator.villages.waypointRange, generator.villages.waypointRange);

        point = new Vector3(randX + transform.position.x, 70, randZ + transform.position.z);

        RaycastHit hit;

        if (Physics.Raycast(point, Vector3.down, out hit, 200) && hit.point.y > generator.seaLevel)
        {
            point = new Vector3(randX + transform.position.x, hit.point.y, randZ + transform.position.z);
        }
        else
        {
            point = GetVillagePoint();
        }

        return point;
    }

    public bool HasAdvancement(Advancement advancement)
    {
        return advancements.Contains(advancement);
    }

    public void Achive(Advancement advancement)
    {
        if (!HasAdvancement(advancement))
        {
            advancements.Add(advancement);

            societyLevel += 1;

            if (advancement == Advancement.axe)
            {
                jobs.Add(new Job(Occupation.lumberjack, 100, 15));
                int newLumberjacks = citizens.Count / 4;
                AssingOccupation(Occupation.lumberjack, newLumberjacks, 100, 15);
            }
            else if (advancement == Advancement.pickaxe)
            {
                jobs.Add(new Job(Occupation.miner, 100, 15));
                int newMiners = citizens.Count / 4;
                AssingOccupation(Occupation.miner, newMiners, 100, 15);
            }
            else if (advancement == Advancement.crops)
            {
                jobs.Add(new Job(Occupation.farmer, 80, 8));
                jobs.Add(new Job(Occupation.hunterGatherer, 110, 20));
                int newFarmers = citizens.Count / 8;
                AssingOccupation(Occupation.farmer, newFarmers, 80, 8);
                AssingOccupation(Occupation.hunterGatherer, newFarmers, 110, 20);
            }
            else if (advancement == Advancement.sword)
            {
                jobs.Add(new Job(Occupation.knight, 130, 40));
                int newKnights = citizens.Count / 5;
                AssingOccupation(Occupation.knight, newKnights, 130, 40);
            }
            LevelUp();


            //switch(advancement)
            //{
            //    case Advancement.axe:
            //        int newLumberjacks = citizens.Count / 4;
            //        AssingOccupation(Occupation.lumberjack, newLumberjacks, 100, 15);
            //        break;
            //    case Advancement.pickaxe:
            //        int newMiners = citizens.Count / 4;
            //        AssingOccupation(Occupation.miner, newMiners, 100, 15);
            //        break;
            //    case Advancement.crops:
            //        int newFarmers = citizens.Count / 8;
            //        AssingOccupation(Occupation.farmer, newFarmers, 80, 8);
            //        AssingOccupation(Occupation.hunterGatherer, newFarmers, 110, 20);
            //        break;
            //    case Advancement.sword:
            //        int newKnights = citizens.Count / 5;
            //        AssingOccupation(Occupation.knight, newKnights, 130, 40);
            //        break;
            //    case Advancement.armor:
            //        foreach (Transform villager in citizens)
            //        {
            //            Villager villager1 = villager.GetComponent<Villager>();
            //            if (villager1.occupation == Occupation.knight)
            //            {

            //            }
            //        }
            //        break;

            //}
            Debug.Log("Advancement Unlocked: '" + advancement + "'.");
        }
    }

    void AssingOccupation(Occupation occupation, int newOccupents, float maxHealth, float NewDamage)
    {
        int j = GetJobID(occupation);
        int rand = Random.Range(0, citizens.Count - (newOccupents + 1));
        for (int i = rand; i < citizens.Count; i++)
        {
            Villager villager = citizens[i].GetComponent<Villager>();
            villager.Occupate(occupation, maxHealth, NewDamage);
            Debug.Log("New Guy " + occupation + " " + i);
            if (!jobs[j].occupents.Contains(citizens[i].GetComponent<Villager>()))
            {
                jobs[j].occupents.Add(citizens[i].GetComponent<Villager>());
            }
        }
    }

    public void AddCity(Transform village)
    {
        cities.Add(village);
        village.parent = transform;
        Village vil = village.GetComponent<Village>();
        City city = village.gameObject.AddComponent<City>();
        city.cityName = vil.villageName;
        foreach(Transform citizen in vil.citizens)
        {
            city.citizens.Add(citizen);
        }
        city.SkinColor = vil.SkinColor;

        Destroy(village.GetComponent<Village>());

        if (societyLevel == 0)
        {
            societyLevel++;
        }
    }

    public void LevelUp()
    {
        societyLevel++;

        for (int i = 0; i < 1; i++)
        {
            StartCoroutine(NewCity(Random.Range(3, 10)));
        }
        for (int i = 0; i < societyLevel; i++)
        {
            NewFarm();
        }
    }

    IEnumerator NewCity(int houseAmount)
    {
        Vector3 newCityPoint = CreateCityPoint();
        int randomName = Random.Range(0, generator.villages.possibleNames.Length - 1);
        var city = new GameObject(generator.villages.possibleNames[randomName]);
        city.transform.position = newCityPoint;
        City cityComp = city.AddComponent<City>();
        cityComp.cityName = city.name;
        cityComp.SkinColor = SkinColor;
        cityComp.generator = generator;
        cityComp.citizens = new List<Transform>();
        city.transform.parent = transform;
        cities.Add(city.transform);

        Job lumberjacks = GetJobClass(Occupation.lumberjack);

        float timeToBuild = 30 * houseAmount;

        foreach (Villager lumberjack in lumberjacks.occupents)
        {
            lumberjack.GoTo(cityComp.GetVillagePoint());
        }

        float lessDelay = lumberjacks.occupents.Count / 2;

        timeToBuild /= lessDelay;

        SmithsHut smithsHut = GetComponentInChildren<SmithsHut>();

        Silo silo = GetComponentInChildren<Silo>();

        for (int i = 0; i < houseAmount; i++)
        {
            if (smithsHut.storage[0].amountSotred < 20 && silo.storage[0].amountSotred < 10)
            {
                break;
            }

            ObjectSpawnData housePoint = generator.GetVillagePoint(city.transform.position);

            foreach (Villager lumberjack in lumberjacks.occupents)
            {
                lumberjack.GoTo(housePoint.point);
            }

            yield return new WaitForSeconds(timeToBuild / houseAmount);

            var house = Instantiate(generator.villages.house, housePoint.point, Quaternion.LookRotation(housePoint.hitNormal));
            house.transform.parent = city.transform;

            smithsHut.storage[0].amountSotred -= 20;

            for (int j = 0; j < 3; j++)
            {
                Vector3 villagerSpawnPoint = cityComp.GetVillagePoint();
                var villager = Instantiate(generator.villages.villager, new Vector3(villagerSpawnPoint.x, 100, villagerSpawnPoint.z), Quaternion.identity);
                villager.transform.parent = transform;
                Villager vil = villager.GetComponent<Villager>();
                vil.village = this;
                vil.city = cityComp;
                vil.mesh.sharedMaterial = SkinColor;
                cityComp.citizens.Add(villager.transform);
                citizens.Add(villager.transform);

                int randomJob = Random.Range(0, jobs.Capacity - 1);

                vil.Occupate(jobs[randomJob].occupation, jobs[randomJob].maxHealth, jobs[randomJob].damage);
            }

            silo.storage[0].amountSotred -= 10;
        }

    }

    void NewFarm()
    {
        SmithsHut smithsHut = GetComponentInChildren<SmithsHut>();

        Silo silo = GetComponentInChildren<Silo>();

        if (smithsHut.storage[0].amountSotred < 5 && silo.storage[0].amountSotred >= 15 && HasAdvancement(Advancement.crops))
        {
            return;
        }
        Vector3 newFarmPoint = CreateCityPoint();
        var farm = Instantiate(generator.villages.fernFarm, new Vector3(newFarmPoint.x, 210, newFarmPoint.z), Quaternion.identity, transform);

        Job farmers = GetJobClass(Occupation.farmer);

        foreach (Villager lumberjack in farmers.occupents)
        {
            lumberjack.GoTo(farm.transform.position);
        }
        farms.Add(farm.transform);

    }

    public Vector3 CreateCityPoint()
    {
        Vector3 point;

        float randX = Random.Range(-generator.villages.waypointRange * 3, generator.villages.waypointRange * 3);
        float randZ = Random.Range(-generator.villages.waypointRange * 3, generator.villages.waypointRange * 3);

        point = new Vector3(randX + transform.position.x, 70, randZ + transform.position.z);

        RaycastHit hit;

        if (Physics.Raycast(point, Vector3.down, out hit, 200) && hit.point.y > generator.seaLevel)
        {
            point = new Vector3(randX + transform.position.x, hit.point.y, randZ + transform.position.z);
        }
        else
        {
            point = CreateCityPoint();
        }

        return point;
    }

    public Job GetJobClass(Occupation occupation)
    {
        Job currentJob = null;
        foreach (Job job in jobs)
        {
            if (job.occupation == occupation)
            {
                currentJob = job;
                break;
            }
        }
        return currentJob;
    }
    public int GetJobID(Occupation occupation)
    {
        int j = 0;
        foreach (Job job in jobs)
        {
            if (job.occupation == occupation)
            {
                break;
            }
            j++;
        }
        return j;
    }
}
public enum Advancement
{
    axe, pickaxe, crops, domestication, sword, armor

}
[System.Serializable]
public class Job
{
    public Occupation occupation;

    public List<Villager> occupents;

    [HideInInspector]
    public float maxHealth;

    [HideInInspector]
    public float damage;

    public Job(Occupation newOccupation, float newMaxHealth, float newDamage)
    {
        occupation = newOccupation;
        maxHealth = newMaxHealth;
        damage = newDamage;
        occupents = new List<Villager>();
    }
}
