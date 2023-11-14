using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keep track of each car's position
[System.Serializable]
public struct placingData
{
    public string carName;
    public int lapsDone;
    public int currentCheckpoint;
    public float distNextCp;
}

public class RacingManager : MonoBehaviour
{
    public List<Transform> checkpoints;     // Used for the race
    public GameObject carsBundle;
    public List<Transform> cars;
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
        foreach (Transform car in carsBundle.transform)
        {
            cars.Add(car);
        }
    }

    void FixedUpdate()
    {
        #region GetData

        placements.Clear();

        // For each car, get their target waypoint and find the distance between them and it. 
        foreach (Transform car in cars)
        {
            placingData thisCarData;
            thisCarData.carName = car.name;

            // If the car is an AI...
            if (car.TryGetComponent<AI_Input>(out AI_Input AI_InputScript))
            {
                Transform target = checkpoints[AI_InputScript.waypointIndex];
                float distanceToTarget = Vector3.Distance(car.transform.position, target.position);

                thisCarData.lapsDone = AI_InputScript.lapsFinished;
                thisCarData.currentCheckpoint = AI_InputScript.waypointIndex;
                thisCarData.distNextCp = distanceToTarget;

                placements.Add(thisCarData);
            }
            // ...or if it's a player.
            else if (car.TryGetComponent<PlayerInput>(out PlayerInput playerInputScript))
            {
                Transform target = checkpoints[playerInputScript.waypointIndex];
                float distanceToTarget = Vector3.Distance(car.transform.position, target.position);

                thisCarData.lapsDone = playerInputScript.lapsFinished;
                thisCarData.currentCheckpoint = playerInputScript.waypointIndex;
                thisCarData.distNextCp = distanceToTarget;

                placements.Add(thisCarData);
            }


        }

        #endregion

        #region UseData



        #endregion
    }
}

