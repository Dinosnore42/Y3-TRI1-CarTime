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
    public float penalty;
    public List<float> bankedLaptimes;
    public float combinedTime;
    public int RaceCompareTo(placingData other)
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
    public GameObject pauseMenu;
    public int numOfLapsInRace;
    public bool calledEnd = false;
    public float updateTimer = 0;

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

    void Update()
    {
        // Pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void FixedUpdate()
    {
        // Better for memory if getting the data and sorting it happens every half-second.
        // Update placements until the race is over
        if (calledEnd == false && updateTimer >= 0.5)
        {
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
                    thisCarData.bankedLaptimes = AI_InputScript.laptimes;
                    thisCarData.penalty = AI_InputScript.damagePenalty;

                    int i = 0;
                    float totalTime = 0;

                    // Add up the time the car took for each lap
                    while (i < numOfLapsInRace)
                    {
                        totalTime += AI_InputScript.laptimes[i];
                        i++;
                    }

                    // Apply the penalty
                    thisCarData.combinedTime = totalTime += thisCarData.penalty;

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
                    thisCarData.bankedLaptimes = playerInputScript.laptimes;
                    thisCarData.penalty = playerInputScript.damagePenalty;

                    int i = 0;
                    float totalTime = 0;

                    // Add up the time the car took for each lap
                    while (i < numOfLapsInRace)
                    {
                        totalTime += playerInputScript.laptimes[i];
                        i++;
                    }

                    // Apply the penalty
                    thisCarData.combinedTime = totalTime += thisCarData.penalty;

                    placements.Add(thisCarData);
                }
            }

            #endregion

            // Sort the list of placings
            placements.Sort((s1, s2) => s1.RaceCompareTo(s2));

            // Check if everyone's finished the race
            bool allFinished = true;

            foreach (placingData contestant in placements)
            {
                if (contestant.lapsDone < numOfLapsInRace)
                {
                    allFinished = false;
                }
            }

            // If everyone has finished, end the race
            if (allFinished == true)
            {
                calledEnd = true;
            }

            updateTimer = 0;
        }

        updateTimer += Time.deltaTime;
    }
}

