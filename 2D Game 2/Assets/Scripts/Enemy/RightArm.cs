using System.Collections;
using UnityEngine;

public class RightArm : MonoBehaviour
{
    public float punchScale = 1.5f;
    public float punchTime = 0.5f;
    public float delay = 0.1f;

    private EnemyDamageEffects enemyDamageEffects;
    private EnemyController enemyController;

    private Vector3 originalScale;
    private LeftArm leftArm;

    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        enemyDamageEffects = GetComponentInParent<EnemyDamageEffects>();
        originalScale = transform.localScale;
        leftArm = transform.parent.GetComponentInChildren<LeftArm>();
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
            enemyDamageEffects.DamageEffect();
            enemyController.DealDamage();

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
