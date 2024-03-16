using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenDamageEvents : MonoBehaviour
{

    EnemyController enemyController;

    public AudioClip[] hitAudioClips;
    public AudioClip[] firstAudioClips;
    public AudioSource audioSource;
    void Start()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyController = enemy.GetComponent<EnemyController>();

    }

    public void HitSound()
    {
        int randomInt = Random.Range(0, hitAudioClips.Length - 1);
        audioSource.PlayOneShot(hitAudioClips[randomInt]);
    }
    public void FirstSound()
    {
        int randomInt = Random.Range(0, firstAudioClips.Length - 1);
        audioSource.PlayOneShot(firstAudioClips[randomInt]);
    }
}
