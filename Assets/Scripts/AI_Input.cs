using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(WeaponController))]
public class AI_Input : MonoBehaviour
{
    private CarController aiCar;
    private WeaponController aiWeapons;

    private float vertical;
    private float horizontal;
    public List<Transform> waypoints;
    public GameObject waypointBundle;
    public int waypointIndex;
    public Transform target;
    private Vector3 heading;
    private float angle;
    public Rigidbody rb;
    public float steeringAcceptance;
    public float targetVelocity;
    public bool recover;

    // Start is called before the first frame update
    void Start()
    {
        aiCar = GetComponent<CarController>();
        aiWeapons = GetComponent<WeaponController>();
        aiCar.Identity(false);
        rb = GetComponent<Rigidbody>();

        foreach (Transform child in waypointBundle.transform)
        {
            waypoints.Add(child);
        }

        UpdateDestination();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        horizontal = 0;
        RaycastHit hit;

        // Check if waypoint is reached
        if (Vector3.Distance(transform.position, target.position) <= 20)
        {
            IterateWaypointIndex();
            UpdateDestination();
        }

        // Steering priority:
        // 1. Avoid the walls
        // 2. Seek the next waypoint
        // 3. Avoid other cars

        #region Steering

        // Get direction and angle of target point relative to the car
        heading = transform.InverseTransformDirection(target.position - transform.position);
        angle = Mathf.Atan2(heading.x, heading.z) * Mathf.Rad2Deg * -1;

        // Steer right, towards a waypoint
        if (angle < -steeringAcceptance)
        {
            horizontal += 0.9f;
        }

        // Steer left, towards a waypoint
        if (angle > steeringAcceptance)
        {
            horizontal -= 0.9f;
        }

        // If a wall is on the left, go right
        var leftRay = new Ray(this.transform.position, -this.transform.right + transform.forward);
        if (Physics.Raycast(leftRay, out hit, 10f) && hit.collider.tag == "Wall")    // Use layers to not hit certain things
        {
            horizontal += 1f;
        }

        // If wall is on the right, go left
        var rightRay = new Ray(this.transform.position, this.transform.right + transform.forward);
        if (Physics.Raycast(rightRay, out hit, 10f) && hit.collider.tag == "Wall")
        {
            horizontal += -1f;
        }

        #endregion

        #region Acceleration and Braking

        // Get the waypoint's target velocity
        targetVelocity = target.transform.localScale.x;

        // If the car has hit a wall, reverse
        var forRay = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(forRay, out hit, 6f) && hit.collider.tag == "Wall")    // Use layers to not hit certain things
        {
            recover = true;
        }
        else
        {
            // If the car is above a waypoint's target velocity, brake...
            if (targetVelocity < (rb.velocity.magnitude * 2.237f))
            {
                vertical = -1f;
            }
            // ...else, accelerate.
            else
            {
                vertical = 1f;
            }
        }

        #endregion

        #region Car Avoidance

        // Set up layer mask for detection radius
        int carLayerMask = LayerMask.GetMask("Car");

        foreach (Collider car in Physics.OverlapSphere(this.transform.position, 20, carLayerMask))
        {
            GameObject trueCar = car.transform.root.gameObject;

            // Stop car from identifying itself
            if (trueCar.name != this.name)
            {
                Vector3 offset = transform.InverseTransformDirection(trueCar.transform.position - this.transform.position);
                Vector3 relativeVelocity = trueCar.GetComponent<Rigidbody>().velocity - rb.velocity;

                // Offset:              x is lateral, z is forward/backward
                // Relative Velocity:   -x is forwards

                // Brake if car ahead is braking
                // Length is up to two car lengths from the front of this car. Width is 2 cars' width.
                if (offset.z >= 3 && offset.z <= 15 && offset.x >= -2.4 && offset.x <= 2.4 && -relativeVelocity.x < 0)
                {
                    vertical = -1f;
                }

                // Lateral avoidance
                // Length is the car's length, plus another car in front of it. Width is 4 cars' width.

                // Left side detected, steer right
                if (offset.x >= -4.8 && offset.x < 0 && offset.z >= -3 && offset.z <= 9)
                {
                    //Debug.Log(this.name + " seeing " + trueCar.name + " on the left");
                    horizontal += 0.7f;
                }
                // Right side detected, steer left
                if (offset.x > 0 && offset.x <= 4.8 && offset.z >= -3 && offset.z <= 9)
                {
                    //Debug.Log(this.name + " seeing " + trueCar.name + " on the right");
                    horizontal -= 0.7f;
                }
            }
        }

        #endregion

        #region Cap vertical/horizontal input

        if (vertical > 1)
        {
            vertical = 1;
        }
        else if (vertical < -1)
        {
            vertical = -1;
        }

        if (horizontal > 1)
        {
            horizontal = 1;
        }
        else if (horizontal < -1)
        {
            horizontal = -1;
        }

        #endregion

        // Crash recovery
        if (Physics.Raycast(forRay, out hit, 12f) && hit.collider.tag == "Wall")
        {
            horizontal = horizontal * -1;
            vertical = -1f;
        }
        else
        {
            recover = false;
        }

        aiCar.InputResponse(vertical, horizontal);
    }

    // See raycasts
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.DrawRay(this.transform.position, (-this.transform.right + transform.forward).normalized * 10f);
            Gizmos.DrawRay(this.transform.position, (this.transform.right + transform.forward).normalized * 10f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(this.transform.position, transform.forward.normalized * 6f);
            Gizmos.DrawWireSphere(target.position, 19f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(this.transform.position, 20f);
        }
    }

    // Set destination to the next waypoint
    void UpdateDestination()
    {
        target = waypoints[waypointIndex].transform;
    }

    // If we've done all waypoints, go to first waypoint
    void IterateWaypointIndex()
    {
        waypointIndex++;
        if(waypointIndex == waypoints.Count)
        {
            waypointIndex = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WeaponPickup"))
        {
            aiWeapons.WeaponSelect();
        }
    }
}
