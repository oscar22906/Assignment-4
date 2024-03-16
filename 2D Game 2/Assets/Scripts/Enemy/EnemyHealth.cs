using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public float maxHealth = 100f;
    public float currentHealth;

    public HealthSlider enemyHealthSlider;
    public HealthSlider playerHealthSlider;

    [Header("Death")]
    public float deathFadeDuration = 1.0f;
    public SpriteRenderer[] spriteRenderers;
    
    private HitEffect enemyHitEffect;
    private EnemyController enemyController;
    private Dialogue dialogue;
    private GameObject board;


    void Start()
    {
        board = GameObject.FindGameObjectWithTag("Board");

        GameObject game = GameObject.FindGameObjectWithTag("Game");
        dialogue = game.GetComponent<Dialogue>();

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
    IEnumerator FadeOut()
    {
        Color[] startColors = new Color[spriteRenderers.Length];
        Color[] endColors = new Color[spriteRenderers.Length];

        // Store initial colors and calculate end colors
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            startColors[i] = spriteRenderers[i].color;
            endColors[i] = new Color(startColors[i].r, startColors[i].g, startColors[i].b, 0f);
        }

        float elapsedTime = 0f;
        while (elapsedTime < deathFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / deathFadeDuration);

            // Update colors for all sprite renderers
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = Color.Lerp(startColors[i], endColors[i], t);
            }
            yield return null;
        }

        // Ensure all sprites are fully transparent at the end
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = endColors[i];
        }

        // Optionally, perform any additional actions after fading out
        // For example, deactivate the GameObject
        gameObject.SetActive(false);
    }


    void Die()
    {
        board.SetActive(false);
        dialogue.DoDeath();
        enemyHitEffect.PlayDamageEffect();
        StartCoroutine(FadeOut());
    }
}
