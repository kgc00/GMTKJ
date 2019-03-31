using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Processes requests for movement and executes callbacks.
public class PathRequestManager : MonoBehaviour {
    public Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();
    public PathRequest currentPathRequest;

    public static PathRequestManager instance;
    [SerializeField]
    public Dictionary<Unit, Coroutine> currentMovementCoroutines;
    bool isProcessingPath = false;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (this);
        }
        currentMovementCoroutines = new Dictionary<Unit, Coroutine> ();
    }

    // Our method which requests to start a new path find from AStar.  For optimization, we use callbacks
    // so that we can process these requests over multiple frames.
    public static void RequestPath (
        Vector3 pathStart,
        Vector3 _pathEnd,
        Action<Vector3[], bool, Unit, Action<Unit>> _callback,
        MovementHandler _movementHandler,
        Unit _unit,
        Action<Unit> onDestReached,
        Action<Unit> commandFailed = null,
        Action<Ability, Unit, Node, List<Node>> aiRequestCallback = null,
        Ability ability = null,
        List<Node> possibleNodes = null) {
        PathRequest _newRequest = new PathRequest (pathStart,
            _pathEnd,
            _callback,
            onDestReached);
        if (!instance.pathRequestQueue.Contains (_newRequest)) {
            instance.pathRequestQueue.Enqueue (_newRequest);
            if (commandFailed != null &&
                aiRequestCallback != null &&
                possibleNodes != null) {
                instance.TryProcessNext (_movementHandler,
                    _unit,
                    onDestReached,
                    commandFailed,
                    aiRequestCallback,
                    ability,
                    possibleNodes);
            } else {
                instance.TryProcessNext (_movementHandler,
                    _unit,
                    onDestReached);
            }
        }
    }

    void TryProcessNext (MovementHandler movementHandler,
        Unit _unit,
        Action<Unit> onDestReached,
        Action<Unit> commandFailed = null,
        Action<Ability, Unit, Node, List<Node>> aiRequestCallback = null,
        Ability ability = null,
        List<Node> possibleNodes = null) {
        if (canProcessNewRequest ()) {
            NewPathLogic ();
            if (_unit.faction == Unit.Faction.Player) {
                StartCoroutine (
                    movementHandler.GenerateMovementPath (currentPathRequest.pathStart,
                        currentPathRequest.pathEnd,
                        _unit,
                        onDestReached));

            } else if (_unit.faction == Unit.Faction.Enemy) {
                if (aiRequestCallback != null &&
                    possibleNodes != null
                ) {
                    var targetNode = GameGrid.instance.NodeFromWorldPosition (currentPathRequest.pathEnd);
                    // Debug.Log ("starting AI logic");
                    StartCoroutine (movementHandler.GeneratePathForAI (
                        currentPathRequest.pathStart,
                        currentPathRequest.pathEnd,
                        ability,
                        _unit,
                        targetNode,
                        possibleNodes,
                        aiRequestCallback));
                } else {
                    // Debug.Log (currentMovementCoroutines.Count);
                    // Debug.Log (pathRequestQueue.Count);
                    if (!currentMovementCoroutines.ContainsKey (_unit)) {
                        Debug.Log ("starting AI logic p2");
                        Coroutine routine = StartCoroutine (
                            movementHandler.GenerateMovementPath (
                                currentPathRequest.pathStart,
                                currentPathRequest.pathEnd,
                                _unit,
                                onDestReached,
                                currentMovementCoroutines));
                        currentMovementCoroutines.Add (_unit, routine);
                        // Debug.Log (currentMovementCoroutines[_unit]);
                    } else {
                        Debug.Log ("didnt call generate movementpath");
                    }
                }
            }
        } else {
            // Debug.Log ("cant process");
            // Debug.Log ("cant proccess 2 requests at the same time: " + _unit + " " +
            // pathRequestQueue.Count + " " + isProcessingPath);
            // if (_unit.faction == Unit.Faction.Enemy && commandFailed != null) {
            //     commandFailed (_unit);
            // }
        }
    }

    public void NewPathLogic () {
        currentPathRequest = pathRequestQueue.Dequeue ();
        isProcessingPath = true;
    }

    public bool canProcessNewRequest () {
        return !isProcessingPath && pathRequestQueue.Count > 0;
    }

    public void FinishedProcessingPath (Vector3[] path,
        bool success,
        Unit unit,
        Action<Unit> onDestReached,
        MovementHandler movementHandler) {
        currentPathRequest.callback (path, success, unit, onDestReached);
        isProcessingPath = false;
        TryProcessNext (movementHandler, unit, currentPathRequest.onDestReached);
    }

    // Data structure which contains all the information necessary to communicate a complex command
    // between AStar, RequestManager, and Unit.
    public struct PathRequest {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Unit> onDestReached;
        public Action<Vector3[], bool, Unit, Action<Unit>> callback;

        public PathRequest (Vector3 _start, Vector3 _end,
            Action<Vector3[], bool, Unit, Action<Unit>> _callback = null, Action<Unit> _onDestReached = null,
            Action<Unit> commandFailed = null) {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            onDestReached = _onDestReached;
        }
    }
}