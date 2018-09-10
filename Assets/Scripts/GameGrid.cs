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
    [SerializeField]
    public Node[,] grid;
    [SerializeField]

    int gridSizeX, gridSizeY;
    [SerializeField]
    private Sprite[] possibleNodeImages;
    [SerializeField]
    private Transform[] tileSprites;
    [SerializeField]
    private int pixelsPerUnit = 100;

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
        nodeDiameter = possibleNodeImages[0].bounds.size.x;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        nodesContainingUnits = new List<Node>();
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask));

                int tileImageIndex = UnityEngine.Random.Range(0, tileSprites.Length);
                Transform currentTile = Instantiate(tileSprites[tileImageIndex], worldPoint, Quaternion.identity);
                currentTile.transform.SetParent(transform);                
                Node currentNode = currentTile.gameObject.AddComponent<Node>();
                currentNode.gameObject.layer = 12;

                if ((Physics.CheckSphere(worldPoint, nodeRadius, allyMask)))
                {
                    Node.OccupiedByUnit occupiedByUnit = Node.OccupiedByUnit.ally;
                    grid[x, y] = currentNode.SetReferences(
                        walkable, worldPoint, x, y, Node.NodeType.plains, occupiedByUnit,
                        possibleNodeImages[tileImageIndex]);
                    nodesContainingUnits.Add(grid[x, y]);
                }
                else if ((Physics.CheckSphere(worldPoint, nodeRadius, enemyMask)))
                {
                    Node.OccupiedByUnit occupiedByUnit = Node.OccupiedByUnit.enemy;
                    grid[x, y] = currentNode.SetReferences(
                        walkable, worldPoint, x, y, Node.NodeType.plains, occupiedByUnit,
                        possibleNodeImages[tileImageIndex]);
                    nodesContainingUnits.Add(grid[x, y]);
                }
                else
                {
                    grid[x, y] = currentNode.SetReferences(
                        walkable, worldPoint, x, y, Node.NodeType.plains, Node.OccupiedByUnit.noUnit,
                        possibleNodeImages[tileImageIndex]);
                }
            }
        }
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
                    if (grid[checkX, checkY].occupiedByUnit == Node.OccupiedByUnit.noUnit)
                    {
                        nodesWithinRange.Add(grid[checkX, checkY]);
                    }
                }
            }
        }
        return nodesWithinRange;
    }

    public List<Node> GetAttackRange(Node node, int range)
    {
        List<Node> nodesWithinAttackRange = new List<Node>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    nodesWithinAttackRange.Add(grid[checkX, checkY]);
                }
            }
        }
        return nodesWithinAttackRange;
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
