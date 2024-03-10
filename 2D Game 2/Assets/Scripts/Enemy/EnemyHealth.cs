using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public float maxHealth = 100f;
    public float currentHealth;

    public HealthSlider enemyHealthSlider;
    public HealthSlider playerHealthSlider;
    
    private HitEffect enemyHitEffect;
    private EnemyController enemyController;


    void Start()
    {
        enemyHealthSlider.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        enemyController = GetComponent<EnemyController>();
        enemyHitEffect = GetComponent<HitEffect>();
    }

    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        Debug.Log("enemy took damage");
        currentHealth -= damage;
        enemyHealthSlider.TakeDamage(damage);
        enemyController.DamageUpdate();

        // Check if the enemy's health has reached zero or below
        if (currentHealth <= 0)
        {
            enemyHitEffect.PlayFinalSound();
            Die();
        }
        
        if(currentHealth > 0)
        {
            enemyHitEffect.PlayNextHitAudio(currentHealth);
            enemyHitEffect.PlayDamageEffect();
        }
    }



    void Die()
    {

        // Destroy the enemy GameObject
        enemyHitEffect.PlayDamageEffect();
    }
}
