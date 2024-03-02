using UnityEngine;
using System.Collections;
using Strobotnik.Klattersynth;

namespace Strobotnik.Klattersynth.Examples {

public class KlattersynthTTS_Example_TalkingRobot : MonoBehaviour
{
    public string textToSpeak;
    public float restartDelay = 0.5f;
    public Transform jaw;
    public float jawTranslateScale = 1.0f;
    public float jawSmoothing = 0.5f;

    Speech speech;
    Coroutine coro;
    Vector3 jawOrgPos;
    float jawY;
    float facingRotation;
    float facingRotationSpeed;

    void Start()
    {
        facingRotation = transform.rotation.eulerAngles.y;
        facingRotationSpeed = Random.Range(1.0f, 16.0f);
        facingRotationSpeed *= Random.Range(0, 2) * 2.0f - 1;
        jawSmoothing = Mathf.Clamp(jawSmoothing, 0, 0.99f);
        speech = GetComponent<Speech>();
        if (speech == null)
            speech = gameObject.AddComponent<Speech>();
        coro = StartCoroutine(speakLoop());
        jawOrgPos = jaw.localPosition;
    }

    IEnumerator speakLoop()
    {
        while (true)
        {
            speech.speak(textToSpeak);
            while (speech.isTalking()) yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(restartDelay);
        }
    }

    void OnDisable()
    {
        if (coro != null)
            StopCoroutine(coro);
        coro = null;
    }

    void FixedUpdate()
    {
        float loudness = speech.getCurrentLoudness();
        jawY = jawSmoothing * jawY + (1 - jawSmoothing) * loudness * jawTranslateScale;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, facingRotation + Time.time * facingRotationSpeed, 0));
        jaw.localPosition = new Vector3(jawOrgPos.x, jawOrgPos.y + jawY, jawOrgPos.z);
    }
}

} // namespace
