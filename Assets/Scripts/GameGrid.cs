using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates and stores all nodes
public class GameGrid : MonoBehaviour
{

    public static GameGrid instance;
    public Vector2 gridWorldSize;
    public float nodeRadius, nodeSize;
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
    public static event Action<Node> requestingHighlights;

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
        nodeSize = possibleNodeImages[0].bounds.size.x;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeSize);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeSize);
        nodesContainingUnits = new List<Node>();
        CreateGrid();
        Unit.OnUnitDeath += RemoveNodeOnUnitDeath;
    }

    void Start()
    {
        foreach (Node node in grid)
        {
            requestingHighlights(node);
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeSize + nodeRadius) + Vector3.up * (y * nodeSize + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask));

                int tileImageIndex = UnityEngine.Random.Range(0, tileSprites.Length);
                Transform currentTile = Instantiate(tileSprites[tileImageIndex], worldPoint, Quaternion.identity);
                currentTile.transform.SetParent(transform);
                Node currentNode = currentTile.gameObject.AddComponent<Node>();
                currentNode.gameObject.layer = 12;
                BoxCollider collider = currentTile.gameObject.AddComponent<BoxCollider>();
                collider.size = new Vector3(nodeSize, nodeSize, 0);
                collider.isTrigger = true;

                if ((Physics.CheckSphere(worldPoint, nodeRadius / 2, allyMask)))
                {
                    Node.OccupiedByUnit occupiedByUnit = Node.OccupiedByUnit.ally;
                    grid[x, y] = currentNode.SetReferences(
                        walkable, worldPoint, x, y, Node.NodeType.plains, occupiedByUnit,
                        possibleNodeImages[tileImageIndex]);
                    // nodesContainingUnits.Add(grid[x, y]);
                }
                else if ((Physics.CheckSphere(worldPoint, nodeRadius / 2, enemyMask)))
                {
                    Node.OccupiedByUnit occupiedByUnit = Node.OccupiedByUnit.enemy;
                    grid[x, y] = currentNode.SetReferences(
                        walkable, worldPoint, x, y, Node.NodeType.plains, occupiedByUnit,
                        possibleNodeImages[tileImageIndex]);
                    // nodesContainingUnits.Add(grid[x, y]);
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

    public List<Node> GetAttackRange(Node node, Ability.AbilityInfo abilityInfo)
    {
        int range = abilityInfo.attackRange;
        List<Node> nodesWithinAttackRange = new List<Node>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                switch (abilityInfo.targetingBehavior)
                {
                    case Ability.TargetingBehavior.line:
                        CheckRangeResultsLine(nodesWithinAttackRange, x, y, checkX, checkY, abilityInfo);
                        break;
                    case Ability.TargetingBehavior.square:
                        CheckRangeResultsSquare(nodesWithinAttackRange, x, y, checkX, checkY, abilityInfo);
                        break;
                    default:
                        break;
                }
            }
        }
        return nodesWithinAttackRange;
    }

    private void CheckRangeResultsSquare(List<Node> nodesWithinAttackRange, int x, int y,
    int checkX, int checkY, Ability.AbilityInfo attackType)
    {
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            if (x == 0 && y == 0)
            {

            }
            else
            {
                nodesWithinAttackRange.Add(grid[checkX, checkY]);
            }
        }
    }

    private void CheckRangeResultsLine(List<Node> nodesWithinAttackRange, int x, int y,
   int checkX, int checkY, Ability.AbilityInfo attackType)
    {
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            if (x == 0 || y == 0)
            {
                if (x == 0 && y == 0)
                {

                }
                else
                {
                    nodesWithinAttackRange.Add(grid[checkX, checkY]);
                }
            }
        }
    }

    private void AddUnitToNodeStatus(Node node)
    {
        if ((Physics.CheckSphere(node.worldPosition, nodeRadius, allyMask)))
        {
            node.occupiedByUnit = Node.OccupiedByUnit.ally;
        }
        else if ((Physics.CheckSphere(node.worldPosition, nodeRadius, enemyMask)))
        {
            node.occupiedByUnit = Node.OccupiedByUnit.enemy;
        }
        // node.walkable = false;
    }

    private void RemoveUnitFromNodeStatus(Node node)
    {
        node.occupiedByUnit = Node.OccupiedByUnit.noUnit;
    }

    private void CheckIfUnitOnNode(Node node)
    {
        // still isn't an accurate check...
        if (Physics.CheckBox(node.transform.position, new Vector3(nodeSize * 0.9f / 2, nodeSize * 0.9f / 2, 2f),
                Quaternion.identity, 1 << 11))
        {
            TryAddNodeToUnitList(node);
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

    public void TryAddNodeToUnitList(Node node)
    {
        if (!nodesContainingUnits.Contains(node))
        {
            nodesContainingUnits.Add(node);
            AddUnitToNodeStatus(node);
        }
    }

    public void TryRemoveNodeToUnitList(Node node)
    {
        if (nodesContainingUnits.Contains(node))
        {
            RemoveUnitFromNodeStatus(node);
            nodesContainingUnits.Remove(node);
            CheckIfUnitOnNode(node);
        }
    }

    private void RemoveNodeOnUnitDeath(Unit _unit)
    {
        // need to refactor
        nodesContainingUnits.Remove(NodeFromWorldPosition(_unit.transform.position));
        NodeFromWorldPosition(_unit.transform.position).occupiedByUnit = Node.OccupiedByUnit.noUnit;
    }

    public Unit UnitFromNode(Node _selectedNode)
    {
        Unit affectedUnit = null;
        Collider[] hitColliders = Physics.OverlapSphere(_selectedNode.worldPosition, nodeRadius / 2, allyMask);
        foreach (Collider collider in hitColliders)
        {
            affectedUnit = collider.gameObject.GetComponentInParent<Unit>();
            if (affectedUnit != null)
            {
                return affectedUnit;
            }
        }
        return affectedUnit;
    }
}
