using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimator : MonoBehaviour
{
    EnemyController enemyController;
    Animator animator;

    [Tooltip("trigger = 'hit' on body animator")]
    public bool useTrigger;
    private AnimationClip[] clips;

    void Start()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyController = enemy.GetComponent<EnemyController>();
        animator = GetComponent<Animator>();
        clips = animator.runtimeAnimatorController.animationClips;
    }

    public void PlayAnimation()
    {
        if (useTrigger)
        {
            animator.SetTrigger("Hit");
        }
        if (!useTrigger)
        {
            StartCoroutine(Animate());
        }
    }
    private IEnumerator Animate()
    {
        var randInd = Random.Range(0, clips.Length);

        var randClip = clips[randInd];

        animator.Play(randClip.name);

        yield return new WaitForSeconds(1);
    }


    public void PlayAudio()
    {
        enemyController.PlayAudio();
    }

    public void ResetIdle()
    {
        animator.Play("Idle");
    }
}
