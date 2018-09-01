using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates and stores all nodes, also responsible for drawing gizmos
public class GameGrid : MonoBehaviour
{

    public Vector2 gridWorldSize;
    public float nodeRadius, nodeDiameter;
    public LayerMask obstacleMask;
    public Node[,] grid;
    [SerializeField]

    int gridSizeX, gridSizeY;
    public bool playerRequestingPath = false;

    public int maxHeapSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void Awake()
    {
        // Set our info for generating the grid
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        // Create an x by y grid
        grid = new Node[gridSizeX, gridSizeY];
        // Store our values in a consistent manner so we can access them later for path requests
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Store our world point so we can access nodes later, and set the value of walkable or not walkable, also create the node
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // A helper method to find connected nodes to the node we are currently processing in ASTAR
    public List<Node> GetNeighbors(Node node)
    {
        // Create a 3x3 list of every surrounding node using math checks
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // eliminate self, and diagonal neighbors from the equation
                if (x == 0 && y == 0 || x == 1 && y != 0 ||
                y == 1 && x != 0 || x == -1 && y != 0 || y == -1 && x != 0)
                {
                    continue;
                }
                else
                {
                    // See if we are inside the grid
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    // If we are, add the node to the list of neighbors
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbors.Add(grid[checkX, checkY]);
                    }
                }
            }
        }
        // This is an efficient check because it solely uses math and a steady coordinate system to check.
        return neighbors;
    }

    // Method to find all nodes which are within a 6 square distance.
    // Returns a grid of 13x13 which we cull down in the ASTAR script.
    public List<Node> GetRange(Node node, int range)
    {        
        List<Node> nodesWithinRange = new List<Node>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    nodesWithinRange.Add(grid[checkX, checkY]);
                }
            }
        }
        return nodesWithinRange;
    }

    // An efficient check which returns wanted nodes based on their vector 3 positions.
    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    // Lists declared for the draw gizmo function
    public List<Node> path;
    public List<Node> _nodesWithinRange;

    // void Update(){
    //     Debug.Log(grid[20, 20].fCost);
    // }

    // Used to help visualize the values of different data/game states.
    // Other possible ways to visualize this include using Unity's Materials/GameObjects/Text.
    // However, I like the fact that each node doesn't come with the memory cost of extending MonoBehavior
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null)
        {
            foreach (Node n in grid)
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
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
