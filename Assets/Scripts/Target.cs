using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 100;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (TryGetComponent<Bunny>(out Bunny bunny))
        {
            Instantiate(bunny.food, transform.position, transform.rotation);
        }
        else if (TryGetComponent<Fox>(out Fox fox))
        {
            Instantiate(fox.food, transform.position, transform.rotation);
        }
        Destroy(gameObject, 0.1f);
    }
}