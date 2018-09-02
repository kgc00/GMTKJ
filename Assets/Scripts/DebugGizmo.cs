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
        grid = GameGrid.instance;
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }


    void OnDrawGizmos()
    {
        if (grid != null)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(grid.gridWorldSize.x, grid.gridWorldSize.y, 2));
            foreach (Node n in grid.grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (_nodesWithinRange != null)
                {
                    if (_nodesWithinRange.Contains(n) && n.walkable)
                    {
                        Gizmos.color = Color.green;
                    }
                }
                if (path != null && playerRequestingPath)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                if (attackTarget != null & playerRequestingTargetting){
                    if (attackTarget.Contains(n))
                    {
                        Gizmos.color = Color.magenta;
                    }
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (grid.nodeDiameter - .1f));
            }
        }
    }
}
