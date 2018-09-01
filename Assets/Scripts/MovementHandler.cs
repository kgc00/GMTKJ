using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Unit))]
public class MovementHandler : MonoBehaviour
{
    public Unit actor;
    public Transform target;
    private UnitStateHandler stateHandler;
    GameGrid grid;
    PathRequestManager requestManager;
    AStar aStar;
	DebugGizmo gizmothing;
    // Use this for initialization
    void Start()
    {
        grid = GameGrid.instance;
		actor = GetComponentInParent<Unit>();
        Debug.Assert(aStar = GetComponentInParent<AStar>());
        Debug.Assert(stateHandler = GetComponentInParent<UnitStateHandler>());
    }

    // We make a void so we can call our coroutine more flexibly
    public void StartMovementPathLogic(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(GenerateMovementPath(startPos, targetPos));
        Move();
    }
    // Initiate actual movement of the unit
    public void Move()
    {
        // Keeps statemachine in sync
        stateHandler.SetMoving(true, 0);
        StartMovementPathLogic(actor.transform.position, target.position);
        stateHandler.ResetLists(new List<Node>());
    }

    // This pathfinding method returns an actual array of vector 3's with which to move our actor.
    // It's called by the pathRequestManager & unitActor.
    public IEnumerator GenerateMovementPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos);

        // Make sure we're clicking on valid targets
        if (startNode.walkable && targetNode.walkable && startNode != targetNode)
        {
            pathSuccess = aStar.PathFindingLogic(pathSuccess, startNode, targetNode, actor.currentMovementPoints);
        }
        yield return null;
        if (pathSuccess)
        {
            // Process these waypoints for use
            waypoints = RetracePath(startNode, targetNode);
        }
        else
        {
            // Called if the user selects an invalid target.
            Debug.LogError("Path requested was not valid.");
        }
        // On finishing this method, we let the request manager know, so it can process with it's logic.
        requestManager.FinishedProcessingPath(waypoints, pathSuccess, this);
    }

    // We need to store this path, and retrace it back to our start point.
    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // Iterate through nodes until we reach the end. 
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        // Return an array of vector3's for movement logic
        Vector3[] waypoints = ConvertPath(path);
        // Reverse our waypoints for the proper order of movement logic
        Array.Reverse(waypoints);
        // Set our path to this on draw
        gizmothing.path = path;
        // Return this list for movement logic
        return waypoints;
    }

    // We convert the path into vector3's in the correct order from a list of nodes
    Vector3[] ConvertPath(List<Node> path)
    {
        Vector3[] waypoints = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            waypoints[i] = new Vector3(path[i].worldPosition.x, path[i].worldPosition.y, -1);
        }
        return waypoints;
    }
}
