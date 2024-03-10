using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public HealthSlider enemyHealthSlider;
    public EnemyHealth enemyHealthScript;

    public HealthSlider playerHealthSlider;
    private float currentHealth;

    public int healthStage = 1;
    public bool increaseMovementWithStage = true;
    public float changePercentage = 0.7f;

    public GameObject damageEffectPrefab; // Assign your particle effect prefab in the Unity Editor
    private Animation _animation;

    void Start()
    {
        playerHealthSlider.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        _animation = GetComponentInChildren<Animation>();
    }

    void Update()
    {

    }


    public void TakeDamage(float damage) // take damage from the players health
    {
        Debug.Log("player took damage");
        currentHealth -= damage;
        playerHealthSlider.TakeDamage(damage);

        DamageUpdate();

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

    public void AttackAnim() // player fist attack anim
    {
        Debug.Log("Hit Animation");
        // _animation.Play("Punch15");
        _animation.Play("Punch" + Random.Range(1, _animation.GetClipCount()));
    }

    public void DamageUpdate() // update player health stage
    {
        float healthPercentage = (float)currentHealth / maxHealth;

        if (healthPercentage >= 1f)
        {
            healthStage = 1;
        }
        else if (healthPercentage >= 2f / 3f)
        {
            healthStage = 2;
        }
        else
        {
            healthStage = 3;
        }
    }

    void PlayDamageEffect() // blood effect on hit
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

        // Destroy GameObject
    }
}
