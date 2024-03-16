using System.Collections;
using UnityEngine;

public class RightArm : MonoBehaviour
{
    public float punchScale = 1.5f;
    public float punchTime = 0.5f;
    public float delay = 0.1f;

    [Tooltip("animate the hand, trigger = 'Hit'")]
    public bool handAnimation;
    public Animator animator;
    private AnimationClip[] clips;

    private EnemyDamageEffects enemyDamageEffects;
    private EnemyController enemyController;

    

    private Vector3 originalScale;
    private LeftArm leftArm;
    public bool sprinklesMode = false;

    void Start()
    {
        enemyDamageEffects = GetComponentInParent<EnemyDamageEffects>();
        enemyController = GetComponentInParent<EnemyController>();
        originalScale = transform.localScale;
        leftArm = transform.parent.GetComponentInChildren<LeftArm>();
        if (animator != null )
        {
            clips = animator.runtimeAnimatorController.animationClips;
        }
    }
    private IEnumerator AnimateHand()
    {
        var randInd = Random.Range(0, clips.Length);

        var randClip = clips[randInd];

        animator.Play(randClip.name);

        yield return new WaitForSeconds(1);
    }

    public void PerformPunchAnimation(int repeatCount)
    {
        StartCoroutine(PunchAnimation(repeatCount));
    }

    private IEnumerator PunchAnimation(int repeatCount)
    {
        for (int i = 0; i < repeatCount; i++)
        {
            // Scale up
            float elapsedTime = 0f;
            while (elapsedTime < punchTime / 2f)
            {
                transform.localScale = Vector3.Lerp(originalScale, originalScale * punchScale, elapsedTime / (punchTime / 2f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure final punch scale
            transform.localScale = originalScale * punchScale;
            if (handAnimation)
            {
                StartCoroutine(AnimateHand());
            }
            if (!sprinklesMode)
            {
                enemyController.DealDamage();
            }
            enemyDamageEffects.DamageEffect();

            // Scale down instantly
            yield return new WaitForSeconds(delay); // Adjust the delay if needed
            transform.localScale = originalScale;

            // After the specified number of repetitions, call the left arm's punch animation
            if (i < repeatCount - 1)
            {
                leftArm.PerformPunchAnimation();
                yield return new WaitForSeconds(delay); // Synchronize with the next right arm punch
            }
        }
    }
}
