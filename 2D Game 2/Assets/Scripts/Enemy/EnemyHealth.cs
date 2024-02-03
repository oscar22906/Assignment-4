using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public HealthSlider healthSlider;
    public float currentHealth;

    public GameObject damageEffectPrefab; // Assign your particle effect prefab in the Unity Editor

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        Debug.Log("enemy took damage");
        currentHealth -= damage;
        healthSlider.TakeDamage(damage);

        // Check if the enemy's health has reached zero or below
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayDamageEffect();
        }
    }

    void PlayDamageEffect()
    {
        // Instantiate and play the damage particle effect
        if (damageEffectPrefab != null)
        {
            GameObject damageEffect = Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
            Destroy(damageEffect, 2.0f); // Adjust the time as needed
        }

        // You can add additional logic here for other effects or animations
    }

    void Die()
    {

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }
}


