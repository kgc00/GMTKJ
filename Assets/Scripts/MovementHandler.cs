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
    Dictionary<Unit, TargetingInformation> targetInfoDictionary = new Dictionary<Unit, TargetingInformation> ();
    private Dictionary<Unit, Vector3[]> pathDictionary;
    UnitStateHandler unitStateHandler;
    UnitMovement unitMovement;
    private Dictionary<Unit, int> targetIndexDictionary;
    private float speed = 5f;
    private Dictionary<Unit, Coroutine> currentGeneratePathCoroutines;
    private Dictionary<Unit, Coroutine> currentFollowPathCoroutines;

    void Start () {
        grid = GameGrid.instance;
        requestManager = PathRequestManager.instance;
        target = FindObjectOfType<TargetPosition> ().transform;
        aStar = FindObjectOfType<AStar> ().GetComponent<AStar> ();
        unitMovement = FindObjectOfType<UnitMovement> ().GetComponent<UnitMovement> ();
        inputHandler = FindObjectOfType<InputHandler> ().GetComponent<InputHandler> ();
        unitStateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
        currentGeneratePathCoroutines = new Dictionary<Unit, Coroutine> ();
        currentFollowPathCoroutines = new Dictionary<Unit, Coroutine> ();
        pathDictionary = new Dictionary<Unit, Vector3[]> ();
        targetIndexDictionary = new Dictionary<Unit, int> ();
    }

    public void StartMovementPathLogic (Unit _unit,
        Action<Unit> onDestReached,
        Vector3 destination
    ) {
        if (currentGeneratePathCoroutines.ContainsKey (_unit)) {
            StopCoroutine (currentGeneratePathCoroutines[_unit]);
            currentGeneratePathCoroutines.Remove (_unit);
        }
        if (targetInfoDictionary.ContainsKey (_unit)) {
            targetInfoDictionary.Remove (_unit);
        }
        TargetingInformation targetInfo = unitMovement.PassTargetInfo ();
        targetInfoDictionary.Add (_unit, targetInfo);

        Coroutine currentRoutine = StartCoroutine (
            GenerateMovementPath (targetInfoDictionary[_unit].startingPoint,
                targetInfoDictionary[_unit].targetPoint, _unit, onDestReached)
        );
        currentGeneratePathCoroutines.Add (_unit, currentRoutine);
        MoveUnit (_unit, onDestReached, destination);
    }

    public IEnumerator GenerateMovementPath (
        Vector3 _startPos,
        Vector3 _targetPos,
        Unit _unit,
        Action<Unit> onDestReached,
        Dictionary<Unit, Coroutine> pathRequestCoroutines = null) {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPosition (_startPos);
        Node targetNode = grid.NodeFromWorldPosition (_targetPos);

        if (startNode.walkable && targetNode.walkable && startNode != targetNode) {
            // if there is a null error, set current ability for unit in the oncalled method of the ability
            // Debug.Log (_unit.currentAbility);
            pathSuccess = aStar.PathFindingLogic (
                pathSuccess,
                startNode,
                targetNode,
                _unit.currentAbility.abilityInfo.attackRange
            );
        }
        yield return null;
        if (_unit.faction == Unit.Faction.Enemy) {
            if (pathSuccess) {
                waypoints = RetracePathAI (startNode, targetNode, _unit);
                Debug.Log (waypoints.Length);
            }
        } else {
            if (pathSuccess) {
                waypoints = RetracePath (startNode, targetNode, _unit);
                Debug.Log (waypoints.Length);
            }
        }
        // Debug.Log ("finished");
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
        Debug.Log (waypoints.Length);
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
        Debug.Log (waypoints.Length);
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
    public void MoveUnit (Unit _unit, Action<Unit> onDestReached, Vector3 destination) {
        PathRequestManager.RequestPath (transform.position,
            destination,
            OnPathFoundAbil,
            this,
            _unit,
            onDestReached);
    }

    public void OnPathFoundAbil (Vector3[] newPath,
        bool pathSuccessful,
        Unit _unit,
        Action<Unit> onDestReached) {

        if (pathSuccessful) {
            if (pathDictionary.ContainsKey (_unit)) {
                pathDictionary.Remove (_unit);
            }
            pathDictionary.Add (_unit, newPath);

            if (targetIndexDictionary.ContainsKey (_unit)) {
                targetIndexDictionary.Remove (_unit);
            }
            targetIndexDictionary.Add (_unit, 0);

            if (currentFollowPathCoroutines.ContainsKey (_unit)) {
                StopCoroutine ("FollowPathAbil");
                currentFollowPathCoroutines.Remove (_unit);
            }
            Coroutine currentRoutine = StartCoroutine (FollowPathAbil (_unit, onDestReached));
            currentFollowPathCoroutines.Add (_unit, currentRoutine);
        }
    }

    public void OnStopPath (Vector3 dest, Unit _unit, Action<Unit> onDestReached = null) {
        if (currentFollowPathCoroutines.ContainsKey (_unit)) {
            StopCoroutine ("FollowPathAbil");
            currentFollowPathCoroutines.Remove (_unit);
        }

        if (pathDictionary.ContainsKey (_unit)) {
            pathDictionary[_unit] = new Vector3[] { dest };
        } else {
            Debug.LogError ("idk");
        }

        if (targetIndexDictionary.ContainsKey (_unit)) {
            targetIndexDictionary.Remove (_unit);
        }
        targetIndexDictionary.Add (_unit, 0);

        Debug.Log ("onStopPath called by: " + _unit);
        Coroutine currentRoutine = StartCoroutine (FollowPathAbil (_unit, onDestReached));
        currentFollowPathCoroutines.Add (_unit, currentRoutine);
    }

    IEnumerator FollowPathAbil (Unit _unit,
        Action<Unit> onDestReached = null) {
        yield return new WaitForSeconds (.15f);
        Vector3 currentWaypoint = new Vector3 (-999, -999, -999);
        if (pathDictionary.ContainsKey (_unit)) {
            currentWaypoint = pathDictionary[_unit][0];
        } else {
            Debug.LogError ("idk");
            yield break;
        }

        Dictionary<Unit, Coroutine> pathRequestCoroutines = PathRequestManager.instance.currentMovementCoroutines;

        while (true) {
            if (_unit.transform.position == currentWaypoint) {
                targetIndexDictionary[_unit]++;
                if (targetIndexDictionary[_unit] >= pathDictionary[_unit].Length) {
                    if (onDestReached != null) {
                        onDestReached (_unit);
                        if (pathRequestCoroutines != null) {
                            if (pathRequestCoroutines.ContainsKey (_unit)) {
                                pathRequestCoroutines.Remove (_unit);
                            }
                        }
                        Debug.Log ("destination reached: " + _unit);
                        yield break;
                    } else {
                        yield break;
                    }
                }
                currentWaypoint = pathDictionary[_unit][targetIndexDictionary[_unit]];
            }
            _unit.transform.position = Vector3.MoveTowards (_unit.transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator GeneratePathForAI (
        Vector3 _startPos,
        Vector3 _targetPos,
        Ability abil,
        Unit unitToControl,
        Node targetNode,
        List<Node> possibleNodes,
        Action<Ability, Unit, Node, List<Node>> onFinishedRequest) {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPosition (_startPos);
        Node ultimateTarget = grid.NodeFromWorldPosition (_targetPos);

        Dictionary<Unit, Coroutine> pathRequestCoroutines = null;
        if (PathRequestManager.instance.currentMovementCoroutines != null) {
            pathRequestCoroutines = PathRequestManager.instance.currentMovementCoroutines;
        }

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