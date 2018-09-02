using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates and stores all nodes
public class GameGrid : MonoBehaviour
{

    public static GameGrid instance;
    public Vector2 gridWorldSize;
    public float nodeRadius, nodeDiameter;
    public LayerMask obstacleMask;
    public LayerMask enemyMask;
    public LayerMask allyMask;
    public List<Node> nodesContainingUnits;
    public Node[,] grid;
    [SerializeField]

    int gridSizeX, gridSizeY;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        // Set our info for generating the grid
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        nodesContainingUnits = new List<Node>();
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

                if ((Physics.CheckSphere(worldPoint, nodeRadius, allyMask)))
                {
                    Node.OccupiedByUnit occupiedByUnit = Node.OccupiedByUnit.ally;
                    grid[x, y] = new Node(walkable, worldPoint, x, y, Node.NodeType.plains, occupiedByUnit);
                    nodesContainingUnits.Add(grid[x, y]);
                }
                else if ((Physics.CheckSphere(worldPoint, nodeRadius, enemyMask)))
                {
                    Node.OccupiedByUnit occupiedByUnit = Node.OccupiedByUnit.enemy;
                    grid[x, y] = new Node(walkable, worldPoint, x, y, Node.NodeType.plains, occupiedByUnit);
                    nodesContainingUnits.Add(grid[x, y]);
                }
                else
                {
                    grid[x, y] = new Node(walkable, worldPoint, x, y, Node.NodeType.plains, Node.OccupiedByUnit.noUnit);
                }
            }
        }
        // CheckForUnits();
    }

    private void CheckForUnits()
    {
        foreach (Node node in grid)
        {
            if ((Physics.CheckSphere(node.worldPosition, nodeRadius, allyMask)) ||
            (Physics.CheckSphere(node.worldPosition, nodeRadius, enemyMask)))
            {
                nodesContainingUnits.Add(node);
            }
            else
            {
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
        return neighbors;
    }

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

    public void UpdateNodeStatuses()
    {
        foreach (Node node in grid)
        {
            if ((Physics.CheckSphere(node.worldPosition, nodeRadius, allyMask)) ||
            (Physics.CheckSphere(node.worldPosition, nodeRadius, enemyMask)))
            {
                node.occupiedByUnit = Node.OccupiedByUnit.ally;
                nodesContainingUnits.Add(node);
            }
            else
            {
                node.occupiedByUnit = Node.OccupiedByUnit.noUnit;
                if (nodesContainingUnits.Contains(node))
                {
                    nodesContainingUnits.Remove(node);
                }
            }
        }
    }

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
}
