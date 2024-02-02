using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [System.Serializable]
    public class State
    {
        public Image image;
        public Color fillColor;
        public Color outlineColor;
    }

    public State state { get; private set; }
    public char letter { get; private set; }

    private TextMeshProUGUI text;
    private Image fill;
    private Outline outline;

    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        fill = GetComponent<Image>();
        outline = GetComponent<Outline>();
    }

    public void SetLetter(char letter)
    {
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }

        this.letter = letter;
        text.text = letter.ToString();
        fadeInCoroutine = StartCoroutine(FadeInTextCoroutine(2.0f));
    }

    public void SetState(State state)
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }

        this.state = state;
        fill.color = state.fillColor;
        outline.effectColor = state.outlineColor;


    }

    public IEnumerator FadeInTextCoroutine(float fadeInTime)
    {
        Debug.Log("Start fade in coroutine");
        Color originalColor = text.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);

        // Explicitly set starting alpha value
        originalColor.a = 0.0f;

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeInTime)
        {
            text.color = Color.Lerp(originalColor, targetColor, elapsedTime / fadeInTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the text is fully visible at the end of the coroutine
        text.color = targetColor;
    }


    public IEnumerator FadeOutTextCoroutine(float fadeOutTime)
    {
        Debug.Log("Start fade out coroutine");
        if (text.color.a < 1.0f)
        {
            Debug.Log("Cancel Fade Out");
            yield return null;
        }

        Color originalColor = text.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.0f);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeOutTime)
        {
            text.color = Color.Lerp(originalColor, targetColor, elapsedTime / fadeOutTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the text is fully transparent at the end of the coroutine
        text.color = targetColor;
    }

    public void ResetToNormal()
    {
        // Reset the color to normal
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }

        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        
        Color originalColor = text.color;
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);
    }
}
