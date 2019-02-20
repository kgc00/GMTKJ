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
    private Dictionary<Unit, Vector3[]> pathDictionary;
    UnitStateHandler unitStateHandler;
    UnitMovement unitMovement;
    private int targetIndex;
    private float speed = 5f;
    private Dictionary<Unit, Coroutine> currentCoroutines;

    void Start () {
        grid = GameGrid.instance;
        requestManager = PathRequestManager.instance;
        target = FindObjectOfType<TargetPosition> ().transform;
        aStar = FindObjectOfType<AStar> ().GetComponent<AStar> ();
        unitMovement = FindObjectOfType<UnitMovement> ().GetComponent<UnitMovement> ();
        inputHandler = FindObjectOfType<InputHandler> ().GetComponent<InputHandler> ();
        unitStateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
        currentCoroutines = new Dictionary<Unit, Coroutine> ();
        pathDictionary = new Dictionary<Unit, Vector3[]> ();
    }

    public void StartMovementPathLogic (Unit _unit,
        Action<Unit> onDestReached
    ) {
        if (currentCoroutines.ContainsKey (_unit)) {
            StopCoroutine (currentCoroutines[_unit]);
            currentCoroutines.Remove (_unit);
        }
        targetInfo = unitMovement.PassTargetInfo ();
        Coroutine currentRoutine = StartCoroutine (
            GenerateMovementPath (targetInfo.startingPoint,
                targetInfo.targetPoint, _unit, onDestReached)
        );
        currentCoroutines.Add (_unit, currentRoutine);
        MoveUnit (_unit, onDestReached);
    }

    public IEnumerator GenerateMovementPath (Vector3 _startPos, Vector3 _targetPos, Unit _unit, Action<Unit> onDestReached) {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPosition (_startPos);
        Node targetNode = grid.NodeFromWorldPosition (_targetPos);

        if (startNode.walkable && targetNode.walkable && startNode != targetNode) {
            // if there is a null error, set current ability for unit in the oncalled method of the ability
            pathSuccess = aStar.PathFindingLogic (
                pathSuccess,
                startNode,
                targetNode,
                _unit.currentAbility.abilityInfo.attackRange
            );
        }
        yield return null;
        if (pathSuccess) {
            waypoints = RetracePath (startNode, targetNode, _unit);
        } else {
            Debug.Log ("Unable to reach target within alotted range.");
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
    Vector3[] RetracePathAI (Node startNode, Node endNode, Unit _unit) {
        List<Node> path = new List<Node> ();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add (currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = ConvertPathAI (path, _unit);
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
    Vector3[] ConvertPathAI (List<Node> path, Unit _unit) {
        Vector3[] waypoints = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++) {
            waypoints[i] = new Vector3 (path[i].worldPosition.x, path[i].worldPosition.y, _unit.transform.position.z);
        }
        return waypoints;
    }
    public void MoveUnit (Unit _unit, Action<Unit> onDestReached) {
        PathRequestManager.RequestPath (transform.position, target.position, OnPathFoundAbil, this, _unit, onDestReached);
    }

    public void OnPathFoundAbil (Vector3[] newPath, bool pathSuccessful, Unit _unit, Action<Unit> onDestReached) {
        if (pathSuccessful) {
            if (pathDictionary.ContainsKey (_unit)) {
                pathDictionary.Remove (_unit);
            }
            pathDictionary.Add (_unit, newPath);

            targetIndex = 0;
            if (currentCoroutines.ContainsKey (_unit)) {
                StopCoroutine ("FollowPathAbil");
                currentCoroutines.Remove (_unit);
            }
            Coroutine currentRoutine = StartCoroutine (FollowPathAbil (_unit, onDestReached));
            currentCoroutines.Add (_unit, currentRoutine);
        }
    }

    public void OnStopPath (Vector3 dest, Unit _unit, Action<Unit> onDestReached = null) {
        if (currentCoroutines.ContainsKey (_unit)) {
            StopCoroutine ("FollowPathAbil");
            currentCoroutines.Remove (_unit);
        }
        if (pathDictionary.ContainsKey (_unit)) {
            pathDictionary[_unit] = new Vector3[] { dest };
        } else {
            Debug.LogError ("idk");
        }
        targetIndex = 0;
        Debug.Log ("onStopPath called by: " + _unit);
        Coroutine currentRoutine = StartCoroutine (FollowPathAbil (_unit, onDestReached));
        currentCoroutines.Add (_unit, currentRoutine);
    }

    IEnumerator FollowPathAbil (Unit _unit, Action<Unit> onDestReached = null) {
        yield return new WaitForSeconds (.15f);
        Vector3 currentWaypoint = new Vector3 (-999, -999, -999);
        if (pathDictionary.ContainsKey (_unit)) {
            currentWaypoint = pathDictionary[_unit][0];
        } else {
            Debug.LogError ("idk");
            yield break;;
        }

        while (true) {
            if (_unit.transform.position == currentWaypoint) {
                targetIndex++;
                if (targetIndex >= pathDictionary[_unit].Length) {
                    if (onDestReached != null) {
                        onDestReached (_unit);
                        yield break;
                    } else {
                        yield break;
                    }
                }
                currentWaypoint = pathDictionary[_unit][targetIndex];
            }
            _unit.transform.position = Vector3.MoveTowards (_unit.transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator GeneratePathForAI (Vector3 _startPos, Vector3 _targetPos, Ability abil,
        Unit unitToControl, Node targetNode, List<Node> possibleNodes,
        Action<Ability, Unit, Node, List<Node>> onFinishedRequest) {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPosition (_startPos);
        Node ultimateTarget = grid.NodeFromWorldPosition (_targetPos);

        if (startNode.walkable && ultimateTarget.walkable && startNode != ultimateTarget) {
            // if there is a null error, set current ability for unit in the oncalled method of the ability
            pathSuccess = aStar.PathFindingLogic (
                pathSuccess,
                startNode,
                ultimateTarget,
                99
                //_unit.currentAbility.abilityInfo.attackRange
            );
        }
        yield return null;
        if (pathSuccess) {
            waypoints = RetracePathAI (startNode, targetNode, unitToControl);
        } else {
            Debug.Log ("Unable to reach target within alotted range.");
        }
        requestManager.FinishedProcessingPath (waypoints, pathSuccess, unitToControl, SomeFakeFunction, this);
        onFinishedRequest (abil,
            unitToControl,
            targetNode,
            possibleNodes);
    }
    public void SomeFakeFunction (Unit unit) {

    }
}