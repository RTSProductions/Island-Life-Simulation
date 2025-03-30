using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silo : MonoBehaviour
{
    public FoodStorage[] storage;

    Village village;

    // Start is called before the first frame update
    void Start()
    {
        village = GetComponentInParent<Village>();
    }

    // Update is called once per frame
    void Update()
    {
        if (storage[0].amountSotred >= 35 && !village.HasAdvancement(Advancement.crops))
        {
            village.Achive(Advancement.crops);
            village.Achive(Advancement.axe);
        }

        if (storage[2].amountSotred >= 15 && storage[3].amountSotred >= 15 && !village.HasAdvancement(Advancement.domestication))
        {
            village.Achive(Advancement.domestication);
        }
    }

    public void Add(FoodType foodType, int amount)
    {
        foreach (FoodStorage Stored in storage)
        {
            if (Stored.foodType == foodType)
            {
                Stored.amountSotred += amount;
            }
        }
    }
}
[System.Serializable]
public class FoodStorage
{
    public string name;

    public FoodType foodType;

    public int amountSotred;
}
