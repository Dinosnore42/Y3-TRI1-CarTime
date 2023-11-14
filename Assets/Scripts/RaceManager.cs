using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RaceManager : MonoBehaviour
{
    // Keep track of each car's position
    public struct placingData
    {
        public string carName;
        public int lapsDone;
        public int currentCheckpoint;
        public float distNextCp;
    }

    public List<Transform> waypoints;       // Used for gizmos
    private Transform previousWaypoint;
    public List<Transform> checkpoints;     // Used for the race
    public GameObject carsBundle;
    public List<GameObject> cars;
    public List<placingData> placements;
    public int laps;
    public int lapsCompleted;

    void Start()
    {
        // All cars start on lap 1. Last checkpoint is the start/finish line.

        // Get all checkpoints on the track, in order.
        foreach (Transform child in this.transform)
        {
            checkpoints.Add(child);
        }

        // Get all cars in the race
        foreach (GameObject car in carsBundle.transform)
        {
            cars.Add(car);
        }
    }

    // Requires to be in update to alter visuals in the editor
    void Update()
    {
        waypoints.Clear();

        foreach (Transform child in this.transform)
        {
            waypoints.Add(child);
        }
    }

    private void FixedUpdate()
    {
        placements.Clear();

        #region GetData

        // For each car, get their target waypoint and find the distance between them and it. 
        foreach (GameObject car in cars)
        {
            // If the car is an AI...
            if (car.TryGetComponent<AI_Input>(out AI_Input AI_InputScript))
            {
                Transform target = checkpoints[AI_InputScript.waypointIndex + 1];
                float distanceToTarget = Vector3.Distance(car.transform.position, target.position);
                
                placingData thisCarData;
                thisCarData.carName = car.name;
                thisCarData.lapsDone = AI_InputScript.lapsFinished;
                thisCarData.currentCheckpoint = AI_InputScript.waypointIndex;
                thisCarData.distNextCp = distanceToTarget;

                placements.Add(thisCarData);
            }
            // ...or if it's a player.
            else if (car.TryGetComponent<PlayerInput>(out PlayerInput playerInputScript))
            {
                Transform target = checkpoints[playerInputScript.waypointIndex + 1];
                float distanceToTarget = Vector3.Distance(car.transform.position, target.position);

                placingData thisCarData;
                thisCarData.carName = car.name;
                thisCarData.lapsDone = AI_InputScript.lapsFinished;
                thisCarData.currentCheckpoint = AI_InputScript.waypointIndex;
                thisCarData.distNextCp = distanceToTarget;

                placements.Add(thisCarData);
            }
        }

        #endregion
    }

    // See waypoints, and create a visualisation of the circuit's path
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        previousWaypoint = waypoints[waypoints.Count - 1];

        foreach (Transform waypoint in waypoints)
        {
            Gizmos.DrawWireSphere(waypoint.position, 20f);
            Gizmos.DrawLine(waypoint.position, previousWaypoint.position);

            previousWaypoint = waypoint;
        }
    }
}
