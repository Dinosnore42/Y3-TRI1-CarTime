using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keep track of each car's position
[System.Serializable]
public struct placingData
{
    public GameObject car;
    public int lapsDone;
    public int currentCheckpoint;
    public float distNextCp;
    public int CompareTo(placingData other)
    {
        int result = lapsDone.CompareTo(other.lapsDone);

        if (result != 0)
        {
            return -result;
        }

        result = currentCheckpoint.CompareTo(other.currentCheckpoint);

        if (result != 0)
        {
            return -result;
        }

        return distNextCp.CompareTo(other.distNextCp);
    }
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
        // Better for memory if getting the data and sorting it happens every half-second.

        // Get the car placements
        #region GetData

        placements.Clear();

        // For each car, get their target waypoint and find the distance between them and it.
        foreach (Transform car in cars)
        {
            placingData thisCarData;
            thisCarData.car = car.gameObject;

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

        // Sort car order
        placements.Sort((s1, s2) => s1.CompareTo(s2));
    }

    private void OnGUI()
    {
        // Background box
        GUI.Box(new Rect(80, 300, 300, 140), "");

        int i = 0;

        foreach(placingData entry in placements)
        {
            GUI.Label(new Rect(85, 305 + (15 * i), 300, 30), (i + 1) + ": " + placements[i].car.name + " - Lap: " + (placements[i].lapsDone + 1));
            i++;
        }
    }
}

