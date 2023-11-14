using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointGizmos : MonoBehaviour
{
    public List<Transform> waypoints;       // Used for gizmos
    private Transform previousWaypoint;

    // Requires to be in update to alter visuals in the editor
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
        previousWaypoint = waypoints[waypoints.Count - 1];

        foreach (Transform waypoint in waypoints)
        {
            Gizmos.DrawWireSphere(waypoint.position, 20f);
            Gizmos.DrawLine(waypoint.position, previousWaypoint.position);

            previousWaypoint = waypoint;
        }
    }
}
