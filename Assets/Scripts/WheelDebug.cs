using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelDebug : MonoBehaviour
{
    [SerializeField] private WheelCollider fl;
    [SerializeField] private WheelCollider fr;
    [SerializeField] private WheelCollider bl;
    [SerializeField] private WheelCollider br;

    private CarController thisCar;
    private Rigidbody thisRb;

    private void Awake()
    {
        thisCar = GetComponent<CarController>();
        thisRb = GetComponent<Rigidbody>();
    }

    private void OnGUI()
    {
        // Background box
        GUI.Box(new Rect(80, 0, 300, 285), "");

        // Wheel data
        WheelDebugUI(fl, 0, -2);
        WheelDebugUI(fr, 1, -2);
        WheelDebugUI(bl, 0, -0.9f);
        WheelDebugUI(br, 1, -0.9f);

        // RPM and gear info
        GUI.Label(new Rect(90, 105, 50, 50), ("Engine RPM: " + ((int)Mathf.Round(thisCar.engineRPM)).ToString()));
        GUI.Label(new Rect(150, 105, 50, 50), ("Gear: " + thisCar.curGear.ToString()));
        GUI.Label(new Rect(210, 105, 50, 50), ("Gear Ratio: " + thisCar.gearVal.ToString() + ":1"));
        GUI.Label(new Rect(270, 105, 50, 50), ("Speed: " + ((int)Mathf.Round(thisRb.velocity.magnitude * 2.237f)).ToString() + "mph"));

        if (thisCar.tractionControl == true && (thisCar.totalForwardSlip >= 1f || thisCar.totalForwardSlip <= -1) && thisCar.braking == 0)
        {
            GUI.Label(new Rect(150, 70, 200, 50), ("TRACTION CONTROL ON"));
        }

        if (thisCar.braking > 0 && (thisCar.totalForwardSlip >= 1f || thisCar.totalForwardSlip <= -1f))
        {
            GUI.Label(new Rect(150, 85, 200, 50), ("ANTI-LOCK BRAKING ON"));
        }

        if (thisCar.automaticGears == true)
        {
            GUI.Label(new Rect(90, 230, 200, 50), ("Z: turn off automatic gears"));
        }
        else
        {
            GUI.Label(new Rect(90, 230, 200, 50), ("Z: turn on automatic gears"));
        }

        if (thisCar.tractionControl == true)
        {
            GUI.Label(new Rect(90, 245, 200, 50), ("X: turn off traction control"));
        }
        else
        {
            GUI.Label(new Rect(90, 245, 200, 50), ("X: turn on traction control"));
        }

        if (thisCar.ABS == true)
        {
            GUI.Label(new Rect(90, 260, 200, 50), ("C: turn off anti-lock braking"));
        }
        else
        {
            GUI.Label(new Rect(90, 260, 200, 50), ("C: turn on anti-lock braking"));
        }
    }

    // Call this for each wheel in OnGUI, with x, y screen offsets
    void WheelDebugUI(WheelCollider wheel, float x, float y)
    {
        wheel.GetGroundHit(out WheelHit hit);
        GUI.Label(new Rect(100 + 150 * x, 300 + 150 * y, 500, 500),
        "RPM = " + ((int)Mathf.Round(wheel.rpm)).ToString("0")
        + "\nForward Slip ="
        + hit.forwardSlip.ToString("0.00")
        + "\nSide Slip ="
        + hit.sidewaysSlip.ToString("0.00")
        + "\nTorque = " + wheel.motorTorque
        );
    }
}