using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silo : MonoBehaviour
{
    public FoodStorage[] storage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
