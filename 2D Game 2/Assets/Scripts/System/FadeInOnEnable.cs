using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOnEnable : MonoBehaviour
{
    public Graphic[] graphics; // Use Graphic type to include both TextMeshProUGUI and Image components
    public float fadeInTime = 1.0f;

    void OnEnable()
    {
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeInCoroutine()
    {
        foreach (Graphic graphic in graphics)
        {
            if (graphic == null)
                continue;

            Color originalColor = graphic.color;
            Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);

            float elapsedTime = 0.0f;

            while (elapsedTime < fadeInTime)
            {
                graphic.color = Color.Lerp(originalColor, targetColor, elapsedTime / fadeInTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the graphic is fully visible at the end of the coroutine
            graphic.color = targetColor;
        }
    }
}
