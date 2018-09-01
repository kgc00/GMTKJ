using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGizmo : MonoBehaviour
{

	Vector2 gridWorldSizeX;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Node> path;
    public List<Node> _nodesWithinRange;

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

    //     if (grid != null)
    //     {
    //         foreach (Node n in grid)
    //         {
    //             Gizmos.color = (n.walkable) ? Color.white : Color.red;
    //             if (_nodesWithinRange != null)
    //             {
    //                 if (_nodesWithinRange.Contains(n) && n.walkable)
    //                 {
    //                     Gizmos.color = Color.green;
    //                 }
    //             }
    //             // if (path != null && playerRequestingPath)
    //             // {
    //             //     if (path.Contains(n))
    //             //     {
    //             //         Gizmos.color = Color.black;
    //             //     }
    //             // }
    //             // Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
    //         }
    //     }
    // }
}
