#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class WaypointNodeBidirectionalFixer
{
    [MenuItem("Tools/Fix Bidirectional Waypoint Links")]
    public static void FixAllNodes()
    {
        WaypointNode[] allNodes = Object.FindObjectsByType<WaypointNode>(FindObjectsSortMode.None);
        foreach (var node in allNodes)
        {
            node.EnsureBidirectionalNeighbors();
        }
        Debug.Log("Fixed bidirectional neighbors on all WaypointNodes.");
    }
}
#endif
