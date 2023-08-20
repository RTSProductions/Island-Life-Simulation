using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmithsHut : MonoBehaviour
{
    public ResourceStorage[] storage;

    Village village;

    // Start is called before the first frame update
    void Start()
    {
        village = GetComponentInParent<Village>();
    }

    // Update is called once per frame
    void Update()
    {
        if (storage[0].amountSotred >= 40 && !village.HasAdvancement(Advancement.pickaxe))
        {
            village.Achive(Advancement.pickaxe);
        }
        if (storage[0].amountSotred >= 100 && storage[1].amountSotred >= 300 && !village.HasAdvancement(Advancement.sword))
        {
            village.Achive(Advancement.sword);
        }
    }

    public void Add(ResourceType resourceType, int amount)
    {
        foreach (ResourceStorage Stored in storage)
        {
            if (Stored.resourceType == resourceType)
            {
                Stored.amountSotred += amount;
            }
        }
    }
}
[System.Serializable]
public class ResourceStorage
{
    public string name;

    public ResourceType resourceType;

    public int amountSotred;
}
