using System.Collections;
using UnityEngine;

public class LeftArm : MonoBehaviour
{
    public float punchScale = 1.5f;
    public float punchTime = 0.5f;
    public float delay = 0.1f;

    [Tooltip("animate the hand, trigger = 'Hit'")]
    public bool handAnimation;
    public Animator animator;
    private AnimationClip[] clips;

    private EnemyController enemyController;
    private EnemyDamageEffects enemyDamageEffects;
    private Vector3 originalScale;
    private RightArm rightArm;
    public bool sprinklesMode = false;

    void Start()
    {
        if (handAnimation)
        {
            clips = animator.runtimeAnimatorController.animationClips;
        }
        enemyController = GetComponentInParent<EnemyController>();
        enemyDamageEffects = GetComponentInParent<EnemyDamageEffects>();
        originalScale = transform.localScale;
        rightArm = transform.parent.GetComponentInChildren<RightArm>();
    }
    private IEnumerator AnimateHand()
    {
        var randInd = Random.Range(0, clips.Length);

        var randClip = clips[randInd];

        animator.Play(randClip.name);

        yield return new WaitForSeconds(1);
    }

    public void PerformPunchAnimation()
    {
        StartCoroutine(PunchAnimation());
    }

    private IEnumerator PunchAnimation()
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

    }
}
