using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo 
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // Is this wheel attached to motor?
    public bool steering; // Does this wheel apply steer angle?
}

public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // The information about each individual axle
    public float maxMotorTorque; // Maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // Maximum steer angle the wheel can have
    public float engineRPM; // Last RPM of the car
    public List<float> gears; // List of gear RPMs
    public int curGear = 1;
    public float gearVal;
    private Rigidbody rb;
    public float totalWheelRPM; // Total RPM of drive wheels
    public float freeWheelRPM; // Total RPM of non-driving wheels
    public float totalForwardSlip; // Total forward slip of all wheels
    public bool automaticGears = true;
    public bool tractionControl = true;
    public float braking;

    // Car wheels
    [SerializeField] private WheelCollider fl;
    [SerializeField] private WheelCollider fr;
    [SerializeField] private WheelCollider bl;
    [SerializeField] private WheelCollider br;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        FindObjectOfType<AudioController>().Play("Idle");
    }

    // Finds the corresponding visual wheel
    // Correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void Update()
    {
        // Gear shifts up
        if (Input.GetKeyDown(KeyCode.UpArrow) && curGear < 5)
        {
            curGear++;
        }

        // Gear shifts down
        if (Input.GetKeyDown(KeyCode.DownArrow) && curGear > 1)
        {
            curGear--;
        }

        #region Toggles
        // Toggle automatic gears
        if (Input.GetKeyDown("z"))
        {
            if (automaticGears == true)
            {
                automaticGears = false;
            }
            else
            {
                automaticGears = true;
            }
        }

        // Toggle traction control
        if (Input.GetKeyDown("x"))
        {
            if (tractionControl == true)
            {
                tractionControl = false;
            }
            else
            {
                tractionControl = true;
            }
        }
        #endregion
    }

    // Applies motion to the wheels
    public void FixedUpdate()
    {
        #region Variable Values
        float stepper = 4f; // Stepping up gear between gearbox and driveshaft
        gearVal = gears[curGear]; // Sets gear ratio to that of current gear value
        
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal"); // Input intensity of steering
        int driveWheelNum = 0; // Number of drive wheels
        float forwardVelocity = Vector3.Dot(rb.velocity, transform.forward); // The direction the car is driving in

        totalWheelRPM = 0;
        freeWheelRPM = 0;
        totalForwardSlip = 0;

        float motorPower = 0;
        braking = 0f;
        #endregion

        // If input direction and velocity direction match...
        if ((forwardVelocity >= 0 && Input.GetAxis("Vertical") > 0) || (forwardVelocity <= 0 && Input.GetAxis("Vertical") < 0))
        {
            // ...Motor power becomes input intensity up to maximum motor torque * stepper motor * current gear's gear ratio
            motorPower = maxMotorTorque * Input.GetAxis("Vertical") * stepper * gearVal;

            #region Automatic Shifting
            float speed = rb.velocity.magnitude * 2.237f;

            if (automaticGears == true)
            {
                if (0 < speed && speed < 10)
                {
                    curGear = 1;
                }
                else if (10 <= speed && speed < 25)
                {
                    curGear = 2;
                }
                else if (25 <= speed && speed < 40)
                {
                    curGear = 3;
                }
                else if (40 <= speed && speed < 60)
                {
                    curGear = 4;
                }
                else if (60 <= speed)
                {
                    curGear = 5;
                }
            }

            // If the car is reversing...
            if ((forwardVelocity <= 0 && Input.GetAxis("Vertical") < 0))
            {
                // ...Set it into reverse gear
                curGear = 0;
            }
            // If the car is going forwards and is in reverse gear...
            else if (curGear == 0)
            {
                // ...Set it into a forward gear
                curGear = 1;
            }

            gearVal = gears[curGear];

            #endregion
        }
        // If input direction and velocity direction don't match...
        else if ((forwardVelocity >= 0 && Input.GetAxis("Vertical") < 0) || (forwardVelocity <= 0 && Input.GetAxis("Vertical") > 0))
        {
            // ...The car is braking
            braking = 10000; // Newton Meters
        }

        // Rev Limiter 
        if (Mathf.Abs(engineRPM) > 6000)
        {
            motorPower = 0;
        }

        // Wheel Slip
        fl.GetGroundHit(out WheelHit flData);
        fr.GetGroundHit(out WheelHit frData);
        bl.GetGroundHit(out WheelHit blData);
        br.GetGroundHit(out WheelHit brData);

        totalForwardSlip += flData.forwardSlip + frData.forwardSlip + blData.forwardSlip + brData.forwardSlip;

        // For each axle in the car...
        foreach (AxleInfo axleInfo in axleInfos)
        {
            #region Steering
            // If the wheel can steer, apply the steering angle
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
                
                freeWheelRPM += axleInfo.leftWheel.rpm;
                freeWheelRPM += axleInfo.rightWheel.rpm;
            }
            #endregion

            #region Motor
            if (axleInfo.motor)
            {
                driveWheelNum += 2;

                totalWheelRPM += axleInfo.leftWheel.rpm;
                totalWheelRPM += axleInfo.rightWheel.rpm;

                // Traction Control
                // If the total slip value exceeds the threshold, and the car is not braking, then motor power is zero
                if (tractionControl == true && (totalForwardSlip >= 1f || totalForwardSlip <= -1) && braking == 0)
                {
                    motorPower = 0;
                }

                axleInfo.leftWheel.motorTorque = motorPower;
                axleInfo.rightWheel.motorTorque = motorPower;
            }
            #endregion

            #region Apply braking torque
            if (braking != 0 && (totalForwardSlip >= 1f || totalForwardSlip <= -1f))
            {
                axleInfo.leftWheel.brakeTorque = 0;
                axleInfo.rightWheel.brakeTorque = 0;
            }
            else
            {
                axleInfo.leftWheel.brakeTorque = braking;
                axleInfo.rightWheel.brakeTorque = braking;
            }
            #endregion

            // Update the visual wheels
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        // Engine RPM
        float driveshaftRPM = totalWheelRPM / driveWheelNum; // driveshaft rpm = average wheel RPM
        engineRPM = driveshaftRPM * stepper * gearVal;
    }
}