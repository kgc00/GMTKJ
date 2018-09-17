using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementHandler : MonoBehaviour
{
    public Unit unit;
    public Transform target;
    GameGrid grid;
    PathRequestManager requestManager;
    AStar aStar;
    InputHandler inputHandler;
    TargetingInformation targetInfo;
    private Vector3[] path;
    UnitStateHandler unitStateHandler;
    UnitMovement unitMovement;
    private int targetIndex;
    private float speed = 5f;

    void Start()
    {
        grid = GameGrid.instance;
        requestManager = PathRequestManager.instance;
        unit = GetComponent<Unit>();
        target = FindObjectOfType<TargetPosition>().transform;
        Debug.Assert(aStar = GetComponent<AStar>());
        unitMovement = GetComponent<UnitMovement>();
        inputHandler = GetComponent<InputHandler>();
        unitStateHandler = GetComponent<UnitStateHandler>();
        // unitStateHandler.onUnitMoving += StartMovementPathLogic;
    }

    private void StartMovementPathLogic()
    {
        targetInfo = unitMovement.PassTargetInfo();
        Unit _unit = grid.UnitFromNode(grid.NodeFromWorldPosition(targetInfo.startingPoint));
        StartCoroutine(GenerateMovementPath(targetInfo.startingPoint, targetInfo.targetPoint, _unit));
        MoveUnit(_unit);
    }

    public IEnumerator GenerateMovementPath(Vector3 _startPos, Vector3 _targetPos, Unit _unit)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPosition(_startPos);
        Node targetNode = grid.NodeFromWorldPosition(_targetPos);

        if (startNode.walkable && targetNode.walkable && startNode != targetNode)
        {
            pathSuccess = aStar.PathFindingLogic(pathSuccess, startNode, targetNode, unit.currentMovementPoints);
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        else
        {
            Debug.LogError("Path requested was not valid.");
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess, _unit, this);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = ConvertPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] ConvertPath(List<Node> path)
    {
        Vector3[] waypoints = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            waypoints[i] = new Vector3(path[i].worldPosition.x, path[i].worldPosition.y, unit.transform.position.z);
        }
        return waypoints;
    }

    public void MoveUnit(Unit _unit)
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound, this, _unit);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful, Unit _unit)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath", _unit);
        }
    }

    IEnumerator FollowPath(Unit _unit)
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    unitStateHandler.ConfirmMovement(_unit);
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }
}
