using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyDamageEffects : MonoBehaviour
{
    [Header("Screen Effect")]
    [Tooltip("Trigger = 'Damage'")]
    public bool animatedScreenEffect;
    public GameObject animatedScreenEffectObject;
    public bool fadeScreenEffect;
    public TextMeshProUGUI screenFadePrefab;
    public float fadeInTime = 2.0f;
    public float waitTime = 0.5f;
    public float fadeOutTime = 2.0f;

    [Header("Hand Effect")]
    public bool handEffect;
    // public GameObject damageEffectPrefab;
    public ParticleSystem _particleSystem;
    public GameObject spawnPoint;
    public float handEffectDestoryTime = 2.0f;

    private Animator animator;


    void Start()
    {
        animator = animatedScreenEffectObject.GetComponent<Animator>();
    }

    public void DamageEffect()
    {
        Debug.Log("heyyy");
        StopAllCoroutines();
        if (handEffect)
        {
            if (_particleSystem != null)
            {
                _particleSystem.Play();
                // GameObject enemyDamageEffect = Instantiate(damageEffectPrefab, spawnPoint.transform, worldPositionStays:false);
                // Destroy(enemyDamageEffect, handEffectDestoryTime); // Adjust the time as needed
            }
        }

        if (animatedScreenEffect && fadeScreenEffect)
        {
            if (screenFadePrefab != null)
            {
                Appear();
            }
        }

        if (animatedScreenEffect && !fadeScreenEffect)
        {
            if (animator != null)
            {
                animator.SetTrigger("Damage");
            }
        }
    }


    public void Appear()
    {
        StartCoroutine(FadeInAndOutCoroutine(fadeInTime, fadeOutTime));
        // Debug.Log("Text appears and disappears");
    }

    IEnumerator FadeInAndOutCoroutine(float fadeInTime, float fadeOutTime)
    {
        // Fade in
        yield return StartCoroutine(FadeInTextCoroutine(fadeInTime));

        // Wait for a short duration (you can adjust this as needed)
        yield return new WaitForSeconds(waitTime);

        // Fade out
        yield return StartCoroutine(FadeOutTextCoroutine(fadeOutTime));
        // Debug.Log("Text disappears");
    }

    IEnumerator FadeInTextCoroutine(float fadeInTime)
    {
        // Debug.Log("Start fade in coroutine");
        Color originalColor = screenFadePrefab.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeInTime)
        {
            screenFadePrefab.color = Color.Lerp(originalColor, targetColor, elapsedTime / fadeInTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the text is fully visible at the end of the coroutine
        screenFadePrefab.color = targetColor;
    }

    IEnumerator FadeOutTextCoroutine(float fadeOutTime)
    {
        // Debug.Log("Start fade out coroutine");
        Color originalColor = screenFadePrefab.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.0f);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeOutTime)
        {
            screenFadePrefab.color = Color.Lerp(originalColor, targetColor, elapsedTime / fadeOutTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the text is fully transparent at the end of the coroutine
        screenFadePrefab.color = targetColor;
    }
}
