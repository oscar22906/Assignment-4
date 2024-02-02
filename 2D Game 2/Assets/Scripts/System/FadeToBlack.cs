using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeToBlack : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public Image fadePanel;
    private float currentAlpha = 0f;

    private void Start()
    {
        fadePanel.color = new Color(0, 0, 0, currentAlpha);

        // fadeToBlack.StartFade(); use in other script

    }
    public void StartFade()
    {
        StartCoroutine(FadeToBlackRoutine());
    }

    private IEnumerator FadeToBlackRoutine()
    {
        float elapsedTime = 0f;
        float startAlpha = currentAlpha;
        float targetAlpha = 1f;

        while (elapsedTime < fadeDuration)
        {
            currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, currentAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentAlpha = targetAlpha;
        fadePanel.color = new Color(0, 0, 0, currentAlpha);
    }
}
