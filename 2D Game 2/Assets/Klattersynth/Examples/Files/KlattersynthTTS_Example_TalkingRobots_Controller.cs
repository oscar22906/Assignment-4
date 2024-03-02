using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Strobotnik.Klattersynth.Examples
{

public class KlattersynthTTS_Example_TalkingRobots_Controller : MonoBehaviour
{
    public Text info;
    [TextArea]
    public string webGLAppendInfo;

    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            info.text += webGLAppendInfo;
    }
}

} // namespace
