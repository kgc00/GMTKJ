using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour {
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
    Coroutine currentRoutine;

    void Start () {
        grid = GameGrid.instance;
        requestManager = PathRequestManager.instance;
        target = FindObjectOfType<TargetPosition> ().transform;
        aStar = FindObjectOfType<AStar> ().GetComponent<AStar> ();
        unitMovement = FindObjectOfType<UnitMovement> ().GetComponent<UnitMovement> ();
        inputHandler = FindObjectOfType<InputHandler> ().GetComponent<InputHandler> ();
        unitStateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
    }

    public void StartMovementPathLogic (Unit _unit,
        Action<Unit> onDestReached
    ) {
        targetInfo = unitMovement.PassTargetInfo ();
        StartCoroutine (
            GenerateMovementPath (targetInfo.startingPoint,
                targetInfo.targetPoint, _unit, onDestReached)
        );
        MoveUnit (_unit, onDestReached);
    }

    public IEnumerator GenerateMovementPath (Vector3 _startPos, Vector3 _targetPos, Unit _unit, Action<Unit> onDestReached) {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPosition (_startPos);
        Node targetNode = grid.NodeFromWorldPosition (_targetPos);

        if (startNode.walkable && targetNode.walkable && startNode != targetNode) {
            pathSuccess = aStar.PathFindingLogic (pathSuccess, startNode, targetNode, _unit.currentMovementPoints);
        }
        yield return null;
        if (pathSuccess) {
            waypoints = RetracePath (startNode, targetNode, _unit);
        } else {
            Debug.LogError ("Path requested was not valid.");
        }
        requestManager.FinishedProcessingPath (waypoints, pathSuccess, _unit, onDestReached, this);
    }

    Vector3[] RetracePath (Node startNode, Node endNode, Unit _unit) {
        List<Node> path = new List<Node> ();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add (currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = ConvertPath (path, _unit);
        Array.Reverse (waypoints);
        return waypoints;
    }

    Vector3[] ConvertPath (List<Node> path, Unit _unit) {
        Vector3[] waypoints = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++) {
            waypoints[i] = new Vector3 (path[i].worldPosition.x, path[i].worldPosition.y, _unit.transform.position.z);
        }
        return waypoints;
    }

    public void MoveUnit (Unit _unit, Action<Unit> onDestReached) {
        // work ondestreached to the request
        PathRequestManager.RequestPath (transform.position, target.position, OnPathFoundAbil, this, _unit, onDestReached);
    }

    public void OnPathFoundAbil (Vector3[] newPath, bool pathSuccessful, Unit _unit, Action<Unit> onDestReached) {
        if (pathSuccessful) {
            path = newPath;
            targetIndex = 0;
            if (currentRoutine != null) {
                StopCoroutine ("FollowPath");
            }
            currentRoutine = StartCoroutine (FollowPathAbil (_unit, onDestReached));
        }
    }

    public void OnStopPath (Vector3 dest, Unit _unit, Action<Unit> onDestReached = null) {
        if (currentRoutine != null) {
            StopCoroutine ("FollowPathAbil");
        }
        path = new Vector3[] { dest };
        targetIndex = 0;
        Debug.Log ("onStopPath called by: " + _unit);
        if (onDestReached != null) {
            currentRoutine = StartCoroutine (FollowPathAbil (_unit, onDestReached));
        } else {
            currentRoutine = StartCoroutine (FollowPathAbil (_unit, onDestReached));
        }
    }

    IEnumerator FollowPathAbil (Unit _unit, Action<Unit> onDestReached = null) {
        yield return new WaitForSeconds (.15f);
        Vector3 currentWaypoint = path[0];
        while (true) {
            if (_unit.transform.position == currentWaypoint) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    if (onDestReached != null) {
                        onDestReached (_unit);
                        yield break;
                    } else {
                        yield break;
                    }
                }
                currentWaypoint = path[targetIndex];
            }
            _unit.transform.position = Vector3.MoveTowards (_unit.transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }
}