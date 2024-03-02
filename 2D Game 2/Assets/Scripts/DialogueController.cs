using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public float dialogueSpeed = 0.1f;
    public float clearDelay = 2f;

    private Coroutine typingCoroutine;
    private Coroutine clearCoroutine;

    // Call this function to display and type out text
    public void DisplayText(string text)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = "";
        typingCoroutine = StartCoroutine(TypeText(text));
        if (clearCoroutine != null)
        {
            StopCoroutine(clearCoroutine);
        }
    }

    // Coroutine to type out text
    IEnumerator TypeText(string text)
    {
        foreach (char character in text)
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(dialogueSpeed);
        }

        // Start the clear coroutine after typing finishes
        clearCoroutine = StartCoroutine(ClearTextAfterDelay());
    }

    // Coroutine to clear text after a delay
    IEnumerator ClearTextAfterDelay()
    {
        yield return new WaitForSeconds(clearDelay);
        ClearText();
    }

    // Call this function to clear the text
    public void ClearText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialogueText.text = "";
    }
}
