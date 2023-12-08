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
    public GameObject raceManager;

    private void Awake()
    {
        thisCar = GetComponent<CarController>();
        thisRb = GetComponent<Rigidbody>();
    }

    private void OnGUI()
    {
        // Player car info

        // Background box 1
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

        if ((thisCar.braking == 0 && thisCar.tractionControl) && (thisCar.totalForwardSlip >= 1f || thisCar.totalForwardSlip <= -1))
        {
            GUI.Label(new Rect(150, 70, 200, 50), ("TRACTION CONTROL ON"));
        }

        if ((thisCar.braking > 0 && thisCar.antiLockBraking) && (thisCar.totalForwardSlip >= 1f || thisCar.totalForwardSlip <= -1f))
        {
            GUI.Label(new Rect(150, 85, 200, 50), ("ANTI-LOCK BRAKING ON"));
        }

        // Placements

        // Background box 2
        GUI.Box(new Rect(80, 300, 400, 140), "");

        int i = 0;

        List<placingData> placements = raceManager.GetComponent<RacingManager>().placements;

        foreach (placingData entry in placements)
        {
            GUI.Label(new Rect(85, 305 + (15 * i), 400, 30), (i + 1) + ": " + placements[i].car.name + " - Lap: " + (placements[i].lapsDone + 1) + " - Time Penalty: " + placements[i].penalty + " - Lap Time: " + placements[i].bankedLaptimes[placements[i].lapsDone]);
            i++;
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