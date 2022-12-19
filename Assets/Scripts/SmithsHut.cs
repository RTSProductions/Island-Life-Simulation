using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmithsHut : MonoBehaviour
{
    public ResourceStorage[] storage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
