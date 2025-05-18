using UnityEngine;
using System.Collections.Generic;

public class WaypointNode : MonoBehaviour
{
    [Tooltip("Neighboring nodes the car can travel to from this node")]
    public WaypointNode[] neighbors;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (neighbors == null) return;

        foreach (var neighbor in neighbors)
        {
            if (neighbor != null)
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }

    // Call this to ensure bidirectional links between this node and its neighbors
    public void EnsureBidirectionalNeighbors()
    {
        if (neighbors == null) return;

        foreach (WaypointNode neighbor in neighbors)
        {
            if (neighbor == null) continue;

            List<WaypointNode> neighborNeighbors = new List<WaypointNode>(neighbor.neighbors ?? new WaypointNode[0]);

            // If this node is not already a neighbor of the neighbor, add it
            if (!neighborNeighbors.Contains(this))
            {
                neighborNeighbors.Add(this);
                neighbor.neighbors = neighborNeighbors.ToArray();
#if UNITY_EDITOR
                // Mark neighbor dirty so Unity saves changes (only in editor)
                UnityEditor.EditorUtility.SetDirty(neighbor);
#endif
            }
        }
    }
}
