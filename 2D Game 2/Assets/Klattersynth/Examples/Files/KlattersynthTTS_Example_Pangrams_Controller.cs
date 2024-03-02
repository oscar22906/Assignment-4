using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Strobotnik.Klattersynth;

namespace Strobotnik.Klattersynth.Examples {

public class KlattersynthTTS_Example_Pangrams_Controller : MonoBehaviour
{
    public Text statusInfo;
    public float initialDelay;
    public float delayBetweenLines;
    public float speaker1TurnTo;
    public float speaker2TurnTo;
    public Speech speech1;
    public Speech speech2;
    public Speech speech3;
    public Text caption1;
    public Text caption2;
    public Text caption3;
    public Graphic[] dimmed;
    public float dimAlpha = 0.25f;
    public Transform speaker1TopObj;
    public Transform speaker2TopObj;
    public Transform speaker1BobObj;
    public Transform speaker2BobObj;
    public Transform speaker1Mouth;
    public Transform speaker2Mouth;
    public RawImage speaker3MouthStretch;
    public RawImage speaker3MouthBottom;

    public Vector3 speaker1MouthDownscale;
    public Vector3 speaker2MouthDownscale;
    public float speaker3MouthRange;

    public Vector3 speaker1Bob;
    public Vector3 speaker2Bob;
    public float speaker1BobSpeed;
    public float speaker2BobSpeed;

    Vector3 speaker1OrgPos;
    Vector3 speaker2OrgPos;
    float speaker3OrgStretch;
    float speaker3OrgY;
    bool dimFinished;
    bool demoFinished;
    bool clipsGenerated;
    float startTime;
    float mouth3Smooth;

    struct DemoLine
    {
        public int speaker;
        public string line;
    };
    DemoLine[] demoLines = new DemoLine[11] {
        new DemoLine() { speaker = 1, line = "I will be happy to demonstrate text-to-speech abilities:\nA quick brown fox jumps over the lazy dog." },
        new DemoLine() { speaker = 2, line = "Boredom you are demonstrating with that sentence." },
        new DemoLine() { speaker = 3, line = "Oh no, here we go again." },
        new DemoLine() { speaker = 2, line = "Much more fascinating sample would be: The quick onyx goblin jumps over the lazy dwarf." },
        new DemoLine() { speaker = 3, line = "Fantastic." },
        new DemoLine() { speaker = 1, line = "Let's stay serious. Small synthetizer to use embedded without data files or internet connection. Just about 100 KB." },
        new DemoLine() { speaker = 2, line = "Is this serious enough: Pack my box with five dozen liquor jugs." },
        new DemoLine() { speaker = 3, line = "Space exploration is science, science is serious." },
        new DemoLine() { speaker = 1, line = "Liquor or do you mean [likO@] as phonemes?\nI don't have a dictionary." },
        new DemoLine() { speaker = 2, line = "It seems us 3 need some things spelled out." },
        new DemoLine() { speaker = 3, line = "Bored? Craving a pub quiz fix? Why, just keep examining every low bid quoted for zinc etchings." },
    };


    void Start()
    {
        caption1.text = "";
        caption2.text = "";
        caption3.text = "";
        speaker1OrgPos = speaker1BobObj.transform.localPosition;
        speaker2OrgPos = speaker2BobObj.transform.localPosition;
        speaker3OrgStretch = speaker3MouthStretch.rectTransform.sizeDelta.y;
        speaker3OrgY = speaker3MouthBottom.rectTransform.localPosition.y;
        StartCoroutine(pangramsDemo());
    } // Start


    IEnumerator pangramsDemo()
    {
        Text[] captions = new Text[3];
        captions[0] = caption1;
        captions[1] = caption2;
        captions[2] = caption3;
        Speech[] speakers = new Speech[3];
        speakers[0] = speech1;
        speakers[1] = speech2;
        speakers[2] = speech3;

        if (!clipsGenerated)
        {
            string orgStatusInfo = statusInfo.text;
            for (int line = 0; line < demoLines.Length; ++line)
            {
                statusInfo.text = orgStatusInfo + " (" + (line + 1) + "/" + demoLines.Length + ")";
                yield return new WaitForEndOfFrame();
                DemoLine dl = demoLines[line];
                int spkIdx = dl.speaker - 1;
                speakers[spkIdx].pregenerate(dl.line);
            }
            statusInfo.text = orgStatusInfo;
            statusInfo.gameObject.SetActive(false);
            clipsGenerated = true;
            startTime = Time.fixedTime;
        }

        demoFinished = false;
        yield return new WaitForSeconds(initialDelay);

        for (int line = 0; line < demoLines.Length; ++line)
        {
            DemoLine dl = demoLines[line];
            int spkIdx = dl.speaker - 1;
            captions[spkIdx].text = dl.line;
            speakers[spkIdx].speak(dl.line, true);
            while (speakers[spkIdx].isTalking()) yield return new WaitForEndOfFrame();
            captions[spkIdx].text = "";
            yield return new WaitForSeconds(delayBetweenLines);
        }

        demoFinished = true;
    } // pangramsDemo

    void FixedUpdate()
    {
        mouth3Smooth = 0.75f * mouth3Smooth + 0.25f * speech3.getCurrentLoudness() * speaker3MouthRange;
    }

    void Update()
    {
        if (demoFinished)
            StartCoroutine(pangramsDemo());

        Vector3 mouth1 = Vector3.one - speaker1MouthDownscale * speech1.getCurrentLoudness();
        Vector3 mouth2 = Vector3.one - speaker2MouthDownscale * speech2.getCurrentLoudness();
        speaker1Mouth.transform.localScale = mouth1;
        //Debug.Log("mouth1 " + mouth1 + " " + speech1.getCurrentLoudness().ToString("F8"));
        speaker2Mouth.transform.localScale = mouth2;
        Vector2 prevSizeDelta = speaker3MouthStretch.rectTransform.sizeDelta;
        speaker3MouthStretch.rectTransform.sizeDelta = new Vector2(prevSizeDelta.x, speaker3OrgStretch + mouth3Smooth);
        Vector3 prevPos = speaker3MouthBottom.rectTransform.localPosition;
        speaker3MouthBottom.rectTransform.localPosition = new Vector3(prevPos.x, speaker3OrgY - mouth3Smooth, prevPos.z);

        speaker1BobObj.transform.localPosition = speaker1OrgPos + speaker1Bob * Mathf.Sin(Time.fixedTime * speaker1BobSpeed);
        speaker2BobObj.transform.localPosition = speaker2OrgPos + speaker2Bob * Mathf.Sin(Time.fixedTime * speaker2BobSpeed);

        if (clipsGenerated && dimFinished)
        {
            // note, code not properly formulated for angles (no proper wrapping)
            Vector3 angles;
            // turn speaker1 immediately
            float turnBlur1 = 0.95f;
            angles = speaker1TopObj.localRotation.eulerAngles;
            angles.y = angles.y * turnBlur1 + speaker1TurnTo * (1 - turnBlur1);
            speaker1TopObj.localRotation = Quaternion.Euler(angles);
            // wait with speaker2 turning until it starts talking
            if (speech2.isTalking())
            {
                float turnBlur2 = 0.9f;
                angles = speaker2TopObj.localRotation.eulerAngles;
                angles.y = angles.y * turnBlur2 + speaker2TurnTo * (1 - turnBlur2);
                speaker2TopObj.localRotation = Quaternion.Euler(angles);
            }
        }

        if (!dimFinished && clipsGenerated && dimmed != null)
        {
            dimFinished = true;
            for (int a = 0; a < dimmed.Length; ++a)
            {
                Color c = dimmed[a].color;
                float timeOffset = a * 0.5f + startTime;
                c.a = Mathf.Lerp(1, dimAlpha, Time.fixedTime - timeOffset);
                dimmed[a].color = c;
                if (c.a > dimAlpha)
                    dimFinished = false;
            }
        } // dim
    } // Update
}

} // namespace
