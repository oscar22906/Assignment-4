using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public AudioClip[] hitAudioClips;
    public AudioClip[] punchAudioClips; // New array for punch sound effects
    public AudioSource hitAudioSource; // Separate audio source for hit audio clips
    public AudioSource punchAudioSource; // Separate audio source for punch audio clips
    public float minHealthPercent = 0.2f; // Percentage at which to start playing the final audio clip

    public GameObject hitPoint;
    public GameObject damageEffectPrefab; // Assign your particle effect prefab in the Unity Editor

    private EnemyHealth enemyHealth;

    private bool hasPlayedFinalClip = false;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    public void PlayNextHitAudio(float currentHealth)
    {
        // Calculate the health percentage
        float healthPercent = currentHealth / enemyHealth.maxHealth;

        // Determine the index of the audio clip to play based on the inverted health percentage
        int hitClipIndex = Mathf.FloorToInt((1f - healthPercent) * hitAudioClips.Length);
        int punchClipIndex = Mathf.FloorToInt((1f - healthPercent) * punchAudioClips.Length);

        // Ensure the clip index is within the valid range
        hitClipIndex = Mathf.Clamp(hitClipIndex, 0, hitAudioClips.Length - 1);
        punchClipIndex = Mathf.Clamp(punchClipIndex, 0, punchAudioClips.Length - 1);

        // Play the audio clip corresponding to the calculated index using the hit audio source
        hitAudioSource.PlayOneShot(hitAudioClips[hitClipIndex]);
        // Play the audio clip corresponding to the calculated index using the punch audio source
        punchAudioSource.PlayOneShot(punchAudioClips[punchClipIndex]);

        Debug.Log("Played Hit Clip " + hitClipIndex);
        Debug.Log("Played Punch Clip " + punchClipIndex);

        // Check if health is less than or equal to 0
        if (currentHealth <= 0)
        {
            // Play final audio clip if not already played
            if (!hasPlayedFinalClip && hitAudioClips.Length > 0)
            {
                hitAudioSource.PlayOneShot(hitAudioClips[hitAudioClips.Length - 1]);
                hasPlayedFinalClip = true;
            }
        }
    }

    public void PlayFinalSound()
    {
        hitAudioSource.PlayOneShot(hitAudioClips[hitAudioClips.Length - 1]);
        punchAudioSource.PlayOneShot(punchAudioClips[punchAudioClips.Length - 1]);
        hasPlayedFinalClip = true;
    }

    public void PlayDamageEffect()
    {
        // Instantiate and play the damage particle effect
        if (damageEffectPrefab != null)
        {
            Instantiate(damageEffectPrefab, hitPoint.transform.position, hitPoint.transform.rotation);
            Destroy(damageEffectPrefab, 2.0f); // Adjust the time as needed
            Debug.Log("Effect played");
        }

        // You can add additional logic here for other effects or animations
    }
}
