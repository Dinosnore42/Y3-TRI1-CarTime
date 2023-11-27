using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(WeaponController))]
public class PlayerInput : MonoBehaviour
{
    private CarController playerCar;
    private WeaponController playerWeapons;
    public List<Transform> waypoints;
    public GameObject waypointBundle;
    public int waypointIndex;
    public Transform target;
    public int lapsFinished = 0;
    public GameObject mainBooster;
    public GameObject leftBooster;
    public GameObject rightBooster;

    // Start is called before the first frame update
    void Start()
    {
        playerCar = GetComponent<CarController>();
        playerWeapons = GetComponent<WeaponController>();
        playerCar.Identity(true);

        foreach (Transform child in waypointBundle.transform)
        {
            waypoints.Add(child);
        }

        UpdateDestination();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region Driving

        // Check if waypoint is reached
        if (Vector3.Distance(transform.position, target.position) <= 20)
        {
            IterateWaypointIndex();
            UpdateDestination();
        }

        // Sends input to car controller
        playerCar.InputResponse(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        #endregion

        #region BoosterVFX

        // Forwards

        float boosterSize = Input.GetAxis("Vertical");

        // Stop booster from firing into car when it reverses
        if (boosterSize < 0)
        {
            boosterSize = 0;
        }

        mainBooster.transform.localScale = new Vector3(1f * boosterSize, 1.5f * boosterSize, 1f * boosterSize);

        // Left/right

        boosterSize = Input.GetAxis("Horizontal");

        // Stop booster from firing into car when it goes left
        if (boosterSize < 0)
        {
            boosterSize = -boosterSize;
        }

        // If going left, fire opposite booster. If not, turn it off.
        if (Input.GetAxis("Horizontal") < 0 && Input.GetAxis("Vertical") >= 0)
        {
            rightBooster.transform.localScale = new Vector3(0.5f * boosterSize, 0.75f * boosterSize, 0.5f * boosterSize);
        }
        else
        {
            rightBooster.transform.localScale = new Vector3(0, 0, 0);
        }

        // If going right, fire opposite booster. If not, turn it off.
        if (Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") >= 0)
        {
            leftBooster.transform.localScale = new Vector3(0.5f * boosterSize, 0.75f * boosterSize, 0.5f * boosterSize);
        }
        else
        {
            leftBooster.transform.localScale = new Vector3(0, 0, 0);
        }

        #endregion

        #region Shooting

        // If the player has a weapon, then allow it to fire it
        if (Input.GetButton("Fire1") && playerWeapons.hasWeapon)
        {
            playerWeapons.fireWeapon();
        }

        #endregion
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
        if (waypointIndex == waypoints.Count)
        {
            waypointIndex = 0;
            lapsFinished++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WeaponPickup"))
        {
            playerWeapons.WeaponSelect();
        }
    }
}
