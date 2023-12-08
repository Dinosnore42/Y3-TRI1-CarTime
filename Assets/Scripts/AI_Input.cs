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
    public int lapsFinished = 0;
    public List<float> laptimes = new List<float>();
    public float currentLapLength = 0;
    public int timedLap = 0;
    public GameObject mainBooster;
    public GameObject leftBooster;
    public GameObject rightBooster;
    private GameObject aimTarget;
    private bool aimDirectionFront = true;
    public float damagePenalty;
    public bool invincible = false;

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

        laptimes.Add(0);
        UpdateDestination();
    }

    private void Update()
    {
        // When lap ends, move to the next lap
        if (lapsFinished > timedLap)
        {
            timedLap++;
            currentLapLength = 0;
            laptimes.Add(0);
        }

        // Track time it takes to do a lap
        currentLapLength += Time.deltaTime;
        laptimes[timedLap] = currentLapLength;

        if (lapsFinished >= waypointBundle.GetComponent<RacingManager>().numOfLapsInRace)
        {
            invincible = true;
        }
    }

    void FixedUpdate()
    {
        #region Driving

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
            horizontal += 0.7f;
        }

        // Steer left, towards a waypoint
        if (angle > steeringAcceptance)
        {
            horizontal -= 0.7f;
        }

        // If a wall is on the left, go right
        var leftRay = new Ray(this.transform.position, -this.transform.right + transform.forward);
        if (Physics.Raycast(leftRay, out hit, 7.5f) && hit.collider.tag == "Wall")    // Use layers to not hit certain things
        {
            horizontal += 1f;
        }

        // If wall is on the right, go left
        var rightRay = new Ray(this.transform.position, this.transform.right + transform.forward);
        if (Physics.Raycast(rightRay, out hit, 7.5f) && hit.collider.tag == "Wall")
        {
            horizontal += -1f;
        }

        #endregion

        #region Acceleration and Braking

        // Get the waypoint's target velocity
        targetVelocity = target.transform.localScale.x;

        // If this is the car at the front, travel slower, allowing the rest of the pack to catch up
        if (gameObject == waypointBundle.GetComponent<RacingManager>().placements[0].car)
        {
            targetVelocity -= 4;
        }

        // If the car has hit a wall, reverse
        var forRay = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(forRay, out hit, 9f) && hit.collider.tag == "Wall")    // Use layers to not hit certain things
        {
            recover = true;
        }
        else
        {
            recover = false;
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
            GameObject trueCar = car.transform.parent.gameObject;

            // Stop car from identifying itself
            if (trueCar.name != this.name)
            {
                Vector3 offset = transform.InverseTransformDirection(trueCar.transform.position - this.transform.position);
                Vector3 relativeVelocity = rb.velocity - trueCar.GetComponent<Rigidbody>().velocity;

                // Offset:              x is lateral, z is forward/backward
                // Relative Velocity:   -x is forwards

                // Stop acceleration if car ahead is braking. In combination with steering away froma car ahead, this should allow for easier overtaking
                // Length is up to two car lengths from the front of this car. Width is 2 cars' width.
                if (offset.z >= 3 && offset.z <= 15 && offset.x >= -2.4 && offset.x <= 2.4 && relativeVelocity.x > 0)
                {
                    vertical = -0.05f;
                }

                // Lateral avoidance
                // Length is the car's length, plus another car in front of it. Width is 4 cars' width.

                // Left side detected, steer right
                if (offset.x >= -4.8 && offset.x < 0 && offset.z >= -3 && offset.z <= 12)
                {
                    //Debug.Log(this.name + " seeing " + trueCar.name + " on the left");
                    horizontal += 0.5f;
                }
                // Right side detected, steer left
                if (offset.x > 0 && offset.x <= 4.8 && offset.z >= -3 && offset.z <= 12)
                {
                    //Debug.Log(this.name + " seeing " + trueCar.name + " on the right");
                    horizontal -= 0.5f;
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
        if (recover)
        {
            horizontal = horizontal * -1;
            vertical = -1f;
        }

        aiCar.InputResponse(vertical, horizontal);

        #endregion

        #region BoosterVFX

        // Forwards

        float boosterSize = vertical;

        // Stop booster from firing into car when it reverses
        if (boosterSize < 0)
        {
            boosterSize = 0;
        }

        mainBooster.transform.localScale = new Vector3(1f * boosterSize, 1.5f * boosterSize, 1f * boosterSize);

        // Left/right

        boosterSize = horizontal;

        // Stop booster from firing into car when it goes left
        if (boosterSize < 0)
        {
            boosterSize = -boosterSize;
        }

        // If going left, fire opposite booster. If not, turn it off.
        if (horizontal < 0 && vertical >= 0)
        {
            rightBooster.transform.localScale = new Vector3(0.5f * boosterSize, 0.75f * boosterSize, 0.5f * boosterSize);
        }
        else
        {
            rightBooster.transform.localScale = new Vector3(0, 0, 0);
        }

        // If going right, fire opposite booster. If not, turn it off.
        if (horizontal > 0 && vertical >= 0)
        {
            leftBooster.transform.localScale = new Vector3(0.5f * boosterSize, 0.75f * boosterSize, 0.5f * boosterSize);
        }
        else
        {
            leftBooster.transform.localScale = new Vector3(0, 0, 0);
        }

        #endregion

        #region Shooting

        #region Acquire target

        // Choose target direction
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            aimDirectionFront = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            aimDirectionFront = false;
        }

        // Find the target
        int i = 0;

        List<placingData> positions = waypointBundle.GetComponent<RacingManager>().placements;

        foreach (placingData vehicle in positions)
        {
            if (vehicle.car == this.gameObject)
            {
                // If in first, override target to be car behind, and vice versa for every other position.
                if (i == 0)
                {
                    aimDirectionFront = false;
                }
                else
                {
                    aimDirectionFront = true;
                }

                // Target is the person in front/behind
                if (aimDirectionFront)
                {
                    aimTarget = positions[i - 1].car;
                }
                else
                {
                    aimTarget = positions[i + 1].car;
                }
            }
            else
            {
                i++;
            }
        }

        aiWeapons.target = aimTarget;

        #endregion

        // If the AI has a weapon, then allow it to fire it
        if (aiWeapons.hasWeapon)
        {
            aiWeapons.fireWeapon();
        }

        #endregion
    }

    // See raycasts
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.DrawRay(this.transform.position, (-this.transform.right + transform.forward).normalized * 7.5f);
            Gizmos.DrawRay(this.transform.position, (this.transform.right + transform.forward).normalized * 7.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(this.transform.position, transform.forward.normalized * 9f);
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
            lapsFinished++;
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
