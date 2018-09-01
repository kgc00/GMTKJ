using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Unit))]
public class MovementHandler : MonoBehaviour
{
    public Unit unit;
    public Transform target;
    GameGrid grid;
    PathRequestManager requestManager;
    AStar aStar;
    DebugGizmo gizmothing;
    InputHandler inputHandler;
    TargetingInformation targetInfo;
    private Vector3[] path;
    UnitStateHandler unitStateHandler;
    private int targetIndex;
    private float speed = 5f;

    // Use this for initialization
    void Start()
    {
        grid = GameGrid.instance;
        requestManager = PathRequestManager.instance;
        gizmothing = DebugGizmo.instance;
        unit = GetComponent<Unit>();
        Debug.Assert(aStar = GetComponent<AStar>());
        inputHandler = GetComponent<InputHandler>();
        unitStateHandler = GetComponent<UnitStateHandler>();
        unitStateHandler.onUnitMoving += StartMovementPathLogic;
    }

    private void StartMovementPathLogic()
    {
        targetInfo = inputHandler.PassTargetInfo();
        StartCoroutine(GenerateMovementPath(targetInfo.startingPoint, targetInfo.targetPoint));
        MoveUnit();
    }

    // public void Move()
    // {
    //     StartMovementPathCoroutine(unit.transform.position, targetInfo.targetPoint);
        // stateHandler.ResetLists(new List<Node>());
    // }

    public IEnumerator GenerateMovementPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos);

        // Make sure we're clicking on valid targets
        if (startNode.walkable && targetNode.walkable && startNode != targetNode)
        {
            pathSuccess = aStar.PathFindingLogic(pathSuccess, startNode, targetNode, unit.currentMovementPoints);
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
            waypoints[i] = new Vector3(path[i].worldPosition.x, path[i].worldPosition.y, unit.transform.position.z);
        }
        return waypoints;
    }

    public void MoveUnit()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound, this);
    }

    // When the path is successfully found by AStar script, we start our coroutine to move our unit through the waypoints to the end node.
    // We stop previous move coroutines for safety.
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
            // unitStateHandler.SetMoving(true, 0);
        }
    }

    // The function which sets our unit's vector3's closer to each target waypoint
    IEnumerator FollowPath()
    {
        // Store our target as the first waypoint
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            // If we are at the current waypoint
            if (transform.position == currentWaypoint)
            {
                // Find next waypoint
                targetIndex++;
                // If our waypoint is the last one in the list, we've reached our target, 
                // and we proceed our statemachine and break the loop
                if (targetIndex >= path.Length)
                {
                    // Moves state logic forward, decrements movement points dynamically
                    unitStateHandler.ConfirmMovement(path.Length);
                    yield break;
                }
                // Else set current waypoint to the increased index
                currentWaypoint = path[targetIndex];
            }
            // Move toward current waypoint
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }
}
