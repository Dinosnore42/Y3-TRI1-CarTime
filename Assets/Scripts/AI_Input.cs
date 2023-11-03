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
    public float vertical;      // Change to priv
    public float horizontal;    // Change to priv
    public List<Transform> waypoints;
    public GameObject waypointBundle;
    public int waypointIndex;
    public Transform target;
    public Vector3 heading;
    public float angle;
    private Rigidbody rb;
    public float steeringAcceptance;

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

        // Steer towards target point
        heading = transform.InverseTransformDirection(target.position - transform.position);
        angle = Mathf.Atan2(heading.x, heading.z) * Mathf.Rad2Deg * -1;

        // Steer right towards a waypoint
        if (angle < -steeringAcceptance)
        {
            horizontal += 0.1f;
        }

        // Steer left towards a waypoint
        if (angle > steeringAcceptance)
        {
            horizontal -= 0.1f;
        }

        RaycastHit hit;
        int layer_mask = LayerMask.GetMask("RaycastCollider");
        layer_mask = ~layer_mask;
        // WARNING: RAYCASTS DO NOT PICK UP OTHER CARS AND I DON'T KNOW WHY

        // Go right
        var leftRay = new Ray(this.transform.position, -this.transform.right + transform.forward);
        if (Physics.Raycast(leftRay, out hit, 10f, layer_mask))    // Use layers to not hit certain things
        {
            horizontal += 0.2f;
        }

        // Go left
        var rightRay = new Ray(this.transform.position, this.transform.right + transform.forward);
        if (Physics.Raycast(rightRay, out hit, 10f, layer_mask))
        {
            horizontal += -0.2f;
        }

        // Go forwards or backwards
        var forRay = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(forRay, out hit, 15f, layer_mask))    // Use layers to not hit certain things
        {
            vertical = -1f;
        }
        else
        {
            vertical = 1f;
        }

        aiCar.InputResponse(vertical, horizontal);
    }

    // See raycasts
    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.DrawRay(this.transform.position, (-this.transform.right + transform.forward).normalized * 10f);
            Gizmos.DrawRay(this.transform.position, (this.transform.right + transform.forward).normalized * 10f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(this.transform.position, transform.forward.normalized * 15f);
            Gizmos.DrawWireSphere(target.position, 9f);
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
