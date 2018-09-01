using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Unit))]
public class AStar : MonoBehaviour
{
    GameGrid grid;
    PathRequestManager requestManager;

    void Start()
    {
        // De-clutter our start method
        InitialAssignment();
    }

    private void InitialAssignment()
    {        
        requestManager = FindObjectOfType<PathRequestManager>();
        grid = GameGrid.instance;
    }

    // Our central location for path finding logic.  We call it from multiple locations and do different
    // actions with the results of whether we could find a path within movement range or not.
    public bool PathFindingLogic(bool pathSuccess, Node startNode, Node targetNode, int currentMovementPoints)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();  
        openSet.Add(startNode);

        // Basically we iterate through all nearby nodes in a line to the target. We calculate distance to target and 
        // distance starting point, then use that info to find the best path.  Return true if path is under max moves/turn.
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost)
                {
                    if (openSet[i].hCost < currentNode.hCost)
                        currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                if (currentNode.fCost <= currentMovementPoints)
                {
                    // If we were able to reach the target node, we've found a path.
                    pathSuccess = true;
                    break;
                }
            }

            foreach (Node neighbour in grid.GetNeighbors(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return pathSuccess;
    }

    // This helper function returns an approximation of the distance to use for pathfinding heuristics
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // Each node away is a distance of one, if the node is diagonal, we return a 
        // distance of 2 to simulate square grid movement.
        if (distanceX > distanceY)
        {
            return (2 * distanceY) + (distanceX - distanceY);
        }
        else
        {
            return (2 * distanceX) + (distanceY - distanceX);
        }
    }
}
