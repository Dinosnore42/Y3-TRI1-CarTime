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
    //public List<CarController> carList = new List<CarController>();

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

        playerCar.InputResponse(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

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
