using UnityEngine;
using System.Collections;

namespace Strobotnik.Klattersynth.Examples {

public class KlattersynthTTS_Example_RobotCamera : MonoBehaviour
{
    public float speed = 1.0f;
    public float r1 = 1.0f;
    public float r2 = 1.0f;
    public float y = 0.75f;
    public float lookAtTOffset = 2.0f;
    public float lookAtTOffsetVarSpd = 1.0f;
    public float lookAtTOffsetVarAmt = 1.0f;

    void Start()
    {
    }

    void Update()
    {
        float t = Time.time;
        float pt = t * speed;
        Vector3 pos = new Vector3(Mathf.Cos(pt) * r1, y, Mathf.Sin(pt) * r2);
        transform.position = pos;
        float lt = pt + lookAtTOffset + Mathf.Sin(t * lookAtTOffsetVarSpd) * lookAtTOffsetVarAmt;
        Vector3 lookAtPos = new Vector3(Mathf.Cos(lt) * r1, y, Mathf.Sin(lt) * r2);
        transform.LookAt(lookAtPos);
    }
}

} // namespace
