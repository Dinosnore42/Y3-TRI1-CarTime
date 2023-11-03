using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class AI_Input : MonoBehaviour
{
    private CarController aiCar;
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
    public List<GameObject> carsInRadius;

    // Start is called before the first frame update
    void Start()
    {
        aiCar = GetComponent<CarController>();
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

        // Check if waypoint is reached
        if (Vector3.Distance(transform.position, target.position) < 10)
        {
            IterateWaypointIndex();
            UpdateDestination();
        }

        // Set up layer mask for raycast to only hit walls
        RaycastHit hit;
        int wallLayerMask = LayerMask.GetMask("RaycastCollider");
        wallLayerMask = ~wallLayerMask;

        // Set up layer mask for detection radius
        int carLayerMask = LayerMask.GetMask("Car");

        carsInRadius.Clear();

        foreach (Collider car in Physics.OverlapSphere(this.transform.position, 20, carLayerMask))
        {
            carsInRadius.Add(car.transform.root.gameObject);
        }





        #region Steering

        // Steer towards target point
        heading = transform.InverseTransformDirection(target.position - transform.position);
        angle = Mathf.Atan2(heading.x, heading.z) * Mathf.Rad2Deg * -1;

        // Steer right towards a waypoint
        if (angle < -steeringAcceptance)
        {
            horizontal += 0.5f;
        }

        // Steer left towards a waypoint
        if (angle > steeringAcceptance)
        {
            horizontal -= 0.5f;
        }

        // If wall is on the left, go right
        var leftRay = new Ray(this.transform.position, -this.transform.right + transform.forward);
        if (Physics.Raycast(leftRay, out hit, 10f, wallLayerMask))    // Use layers to not hit certain things
        {
            horizontal += 0.6f;
        }

        // If wall is on the right, go left
        var rightRay = new Ray(this.transform.position, this.transform.right + transform.forward);
        if (Physics.Raycast(rightRay, out hit, 10f, wallLayerMask))
        {
            horizontal += -0.6f;
        }

        #endregion

        #region Acceleration and Braking

        // Get the waypoint's target velocity
        targetVelocity = target.transform.localScale.x;

        // If the car has hit a wall, reverse
        var forRay = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(forRay, out hit, 15f, wallLayerMask))    // Use layers to not hit certain things
        {
            vertical = -1f;
        }
        else
        {
            // If above waypoint target velocity, brake...
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
            Gizmos.DrawRay(this.transform.position, transform.forward.normalized * 15f);
            Gizmos.DrawWireSphere(target.position, 14f);
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
}
