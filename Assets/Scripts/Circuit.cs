using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    public List<GameObject> waypoints;

    private void OnDrawGizmosSelected()
    {
        if (waypoints.Count > 1)
        {
            Vector3 prev = waypoints[0].transform.position;
            for (int c = 1; c < waypoints.Count; c++)
            {
                Vector3 next = waypoints[c].transform.position;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.DrawLine(prev, waypoints[0].transform.position);
        }
    }
    
}
