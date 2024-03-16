using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinklesBag : MonoBehaviour
{
    Animator animator;
    public EnemyController enemyController;
    public Animator coins;
    public AudioClip woosh;
    public AudioClip[] coinHit;
    public AudioSource source;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Hit()
    {
        animator.SetTrigger("Hit");
    }
    public void Woosh()
    {
        source.PlayOneShot(woosh, 0.3f);
    }
    public void CoinHit()
    {
        int num = Random.Range(0, coinHit.Length - 1);
        source.PlayOneShot(coinHit[num], 0.5f);
        coins.SetTrigger("Hit");
        enemyController.DealDamage();
    }
}
