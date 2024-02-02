using System.Collections;
using UnityEngine;

public class LeftArm : MonoBehaviour
{
    public float punchScale = 1.5f;
    public float punchTime = 0.5f;
    public float delay = 0.1f;

    private Vector3 originalScale;
    private RightArm rightArm;

    void Start()
    {
        originalScale = transform.localScale;
        rightArm = transform.parent.GetComponentInChildren<RightArm>();
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

        // Scale down instantly
        yield return new WaitForSeconds(delay); // Adjust the delay if needed
        transform.localScale = originalScale;

    }
}
