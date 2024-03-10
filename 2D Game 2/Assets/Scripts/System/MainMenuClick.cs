using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuClick : MonoBehaviour
{
    public Graphic titleGraphic;
    public Graphic[] otherGraphicsToFadeIn; // Array of other graphics to fade in after the first click
    public float fadeInTime;
    public int clickCount = 0;


    void Start()
    {
        // Set all other graphics to 0 alpha at the beginning
        SetAlpha(titleGraphic, 0f);

        foreach (Graphic graphic in otherGraphicsToFadeIn)
        {
            SetAlpha(graphic, 0f);
        }





    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;

            if (clickCount == 1)
            {
                StartCoroutine(FadeInGraphic(titleGraphic));
            }
            else if (clickCount == 2)
            {
                foreach (Graphic graphic in otherGraphicsToFadeIn)
                {
                    StartCoroutine(FadeInGraphic(graphic));
                }
            }
        }
    }
    IEnumerator FadeInGraphic(Graphic graphic)
    {
        // Debug.Log("Start fade in coroutine");
        Color originalColor = graphic.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeInTime)
        {
            graphic.color = Color.Lerp(originalColor, targetColor, elapsedTime / fadeInTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the text is fully visible at the end of the coroutine
        graphic.color = targetColor;
    }

    void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic != null)
        {
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}
