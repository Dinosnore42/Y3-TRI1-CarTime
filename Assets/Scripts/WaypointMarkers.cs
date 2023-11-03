using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointMarkers : MonoBehaviour
{
    public List<Transform> waypoints;
    private Transform lastWaypoint;

    // Start is called before the first frame update
    void Update()
    {
        waypoints.Clear();

        foreach (Transform child in this.transform)
        {
            waypoints.Add(child);
        }
    }

    // See waypoints, and create a visualisation of the circuit's path
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        lastWaypoint = waypoints[waypoints.Count - 1];

        foreach (Transform waypoint in waypoints)
        {
            Gizmos.DrawWireSphere(waypoint.position, 10f);
            Gizmos.DrawLine(waypoint.position, lastWaypoint.position);

            lastWaypoint = waypoint;
        }
    }
}
