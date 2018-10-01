using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGizmo : MonoBehaviour
{

    public List<Node> path;
    public List<Node> attackTarget;
    public List<Node> _nodesWithinRange;
    GameGrid grid;
    public static DebugGizmo instance;
    public bool playerRequestingPath;
    public bool playerRequestingTargetting;

    void Awake()
    {
        grid = GetComponent<GameGrid>();
    }


    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node n in grid.nodesContainingUnits)
            {

                Gizmos.color = Color.magenta;

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (grid.nodeDiameter - .1f));
            }
        }
    }
}
