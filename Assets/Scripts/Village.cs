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

    bool atWar = false;

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
        for (int i = 0; i < citizens.Count; i++)
        {
            if (citizens[i] == null)
            {
                citizens.RemoveAt(i);
            }
        }
        foreach (Job job in jobs)
        {
            for (int i = 0; i < job.occupents.Count; i++)
            {
                if (job.occupents[i] == null)
                {
                    job.occupents.RemoveAt(i);
                }
            }
        }
        for (int i = 0; i < farms.Count; i++)
        {
            if (farms[i] == null)
            {
                farms.RemoveAt(i);
            }
        }

        if (citizens.Count < (generator.villages.population - 15) + (5 * societyLevel) && atWar == false)
        {
            NewVillager();
        }
    }

    public Vector3 GetVillagePoint()
    {
        Vector3 point = Vector3.one * 1000;

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
                jobs.Add(new Job(Occupation.lumberjack, 100, 15, 50));
                int newLumberjacks = citizens.Count / 4;
                AssingOccupation(Occupation.lumberjack, newLumberjacks, 100, 15, 50);
            }
            else if (advancement == Advancement.pickaxe)
            {
                jobs.Add(new Job(Occupation.miner, 100, 15, 20));
                int newMiners = citizens.Count / 4;
                AssingOccupation(Occupation.miner, newMiners, 100, 15, 20);
            }
            else if (advancement == Advancement.crops)
            {
                jobs.Add(new Job(Occupation.farmer, 80, 8, 12));
                int newFarmers = citizens.Count / 8;
                AssingOccupation(Occupation.farmer, newFarmers, 80, 8, 12);
                jobs.Add(new Job(Occupation.hunterGatherer, 110, 20, 30));
                AssingOccupation(Occupation.hunterGatherer, newFarmers, 110, 20, 30);
            }
            else if (advancement == Advancement.sword)
            {
                jobs.Add(new Job(Occupation.knight, 130, 40, 60));
                int newKnights = citizens.Count / 5;
                AssingOccupation(Occupation.knight, newKnights, 130, 40, 60);
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

    void AssingOccupation(Occupation occupation, int newOccupents, float maxHealth, float NewDamage, float newVisionRadius)
    {
        int j = GetJobID(occupation);
        int rand = Random.Range(0, citizens.Count - (newOccupents + 1));
        for (int i = rand; i < citizens.Count; i++)
        {
            Villager villager = citizens[i].GetComponent<Villager>();
            villager.Occupate(occupation, maxHealth, NewDamage, newVisionRadius);
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
        ObjectSpawnData townHallPoint = generator.GetVillagePoint(city.transform.position);     
        var townHall = Instantiate(generator.villages.townHall, townHallPoint.point, Quaternion.LookRotation(townHallPoint.hitNormal));
        townHall.transform.parent = city.transform;
        vil.enabled = false;
        if (societyLevel == 0)
        {
            societyLevel++;
        }
    }

    public void AddCity(GameObject newCity)
    {
        cities.Add(newCity.transform);
        newCity.transform.parent = transform;
        City city = newCity.GetComponent<City>();
        foreach(Transform citizen in city.citizens)
        {
            city.citizens.Add(citizen);
        }
        ObjectSpawnData townHallPoint = generator.GetVillagePoint(city.transform.position);     
        var townHall = Instantiate(generator.villages.townHall, townHallPoint.point, Quaternion.LookRotation(townHallPoint.hitNormal));
        townHall.transform.parent = city.transform;
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
            StartCoroutine(NewCity(Random.Range(5, 12)));
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

        ObjectSpawnData firePoint = generator.GetVillagePoint(city.transform.position);
        var fire = Instantiate(generator.villages.requiredStructures[1], firePoint.point, Quaternion.LookRotation(firePoint.hitNormal));
        fire.transform.parent = city.transform;

        ObjectSpawnData townHallPoint = generator.GetVillagePoint(city.transform.position);     
        var townHall = Instantiate(generator.villages.townHall, townHallPoint.point, Quaternion.LookRotation(townHallPoint.hitNormal));
        townHall.transform.parent = city.transform;

        for (int i = 0; i < houseAmount; i++)
        {

            if (smithsHut.storage[0].amountSotred < 15 && silo.storage[0].amountSotred < 3)
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

                vil.Occupate(jobs[randomJob].occupation, jobs[randomJob].maxHealth, jobs[randomJob].damage, jobs[randomJob].visionRadius);
                if (silo.storage[1].amountSotred >= 1)
                {
                    silo.storage[1].amountSotred -=1;
                }
                else if (silo.storage[0].amountSotred >= 1)
                {
                    silo.storage[0].amountSotred -=1;
                }
                else if (silo.storage[2].amountSotred >= 1)
                {
                    silo.storage[2].amountSotred -=1;
                }
                else if (silo.storage[3].amountSotred >= 1)
                {
                    silo.storage[3].amountSotred -=1;
                }
                else 
                {
                    break;
                }
            }

        }

    }

    void CreateHouse (Transform city)
    {
        Job lumberjacks = GetJobClass(Occupation.lumberjack);
        SmithsHut smithsHut = GetComponentInChildren<SmithsHut>();
        ObjectSpawnData housePoint = generator.GetVillagePoint(city.transform.position);

        foreach (Villager lumberjack in lumberjacks.occupents)
        {
            lumberjack.GoTo(housePoint.point);
        }

        var house = Instantiate(generator.villages.house, housePoint.point, Quaternion.LookRotation(housePoint.hitNormal));
        house.transform.parent = city.transform;

        smithsHut.storage[0].amountSotred -= 20;
    }

    void NewVillager()
    {
        Silo silo = GetComponentInChildren<Silo>();

        if (silo.storage[1].amountSotred >= 1)
        {
            silo.storage[1].amountSotred -=1;
        }
        else if (silo.storage[0].amountSotred >= 1)
        {
            silo.storage[0].amountSotred -=1;
        }
        else if (silo.storage[2].amountSotred >= 1)
        {
            silo.storage[2].amountSotred -=1;
        }
        else if (silo.storage[3].amountSotred >= 1)
        {
            silo.storage[3].amountSotred -=1;
        }
        else 
        {
            return;
        }

        List<Transform> possibleCities = new List<Transform>();
        for (int i = 0; i < cities.Count; i++)
        {
            possibleCities.Add(cities[i]);
        }
        possibleCities.Add(transform);

        int randCity = Random.Range(0, cities.Count - 1);
        if (randCity != possibleCities.Count - 1)
        {
            City cityComp = possibleCities[randCity].GetComponent<City>();

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

            vil.Occupate(jobs[randomJob].occupation, jobs[randomJob].maxHealth, jobs[randomJob].damage, jobs[randomJob].visionRadius);
        }
        else 
        {
            Vector3 villagerSpawnPoint = GetVillagePoint();
            var villager = Instantiate(generator.villages.villager, new Vector3(villagerSpawnPoint.x, 100, villagerSpawnPoint.z), Quaternion.identity);
            villager.transform.parent = transform;
            Villager vil = villager.GetComponent<Villager>();
            vil.village = this;
            vil.mesh.sharedMaterial = SkinColor;
            citizens.Add(villager.transform);
            int randomJob = Random.Range(0, jobs.Capacity - 1);

            vil.Occupate(jobs[randomJob].occupation, jobs[randomJob].maxHealth, jobs[randomJob].damage, jobs[randomJob].visionRadius);
        }
    }

    void NewFarm()
    {
        SmithsHut smithsHut = GetComponentInChildren<SmithsHut>();

        Silo silo = GetComponentInChildren<Silo>();

        if (smithsHut.storage[0].amountSotred < 5 && silo.storage[0].amountSotred < 15 || !HasAdvancement(Advancement.crops))
        {
            return;
        }
        Vector3 newFarmPoint = CreateCityPoint();
        var farm = Instantiate(generator.villages.fernFarm, new Vector3(newFarmPoint.x, 210, newFarmPoint.z), Quaternion.identity, transform);

        Job farmers = GetJobClass(Occupation.farmer);

        foreach (Villager farmer in farmers.occupents)
        {
            farmer.GoTo(farm.transform.position);
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

    public void RequestConstruction(ConstructionType type, Transform city)
    {
        switch (type)
        {
            case ConstructionType.house:
                if (atWar)
                {
                    break;
                }
                Job lumberjacks = GetJobClass(Occupation.lumberjack);
                SmithsHut smithsHut = GetComponentInChildren<SmithsHut>();
                if (lumberjacks.occupents.Count <= 0)
                {
                    break;
                }
                if (smithsHut.storage[0].amountSotred < 20)
                {
                    break;
                }
                CreateHouse(city);
                break;
        }
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
    void OnDrawGizmosSelected() 
    {
        for (int i = 0; i < cities.Count; i++)
        {
            Debug.DrawLine(transform.position, cities[i].transform.position, Color.blue);
        }
    }
}
public enum Advancement
{
    axe, pickaxe, crops, domestication, sword, armor
}

public enum ConstructionType
{
    house
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

    [HideInInspector]
    public float visionRadius;

    public Job(Occupation newOccupation, float newMaxHealth, float newDamage, float newVisionRadius)
    {
        occupation = newOccupation;
        maxHealth = newMaxHealth;
        damage = newDamage;
        visionRadius = newVisionRadius;
        occupents = new List<Villager>();
    }
}
