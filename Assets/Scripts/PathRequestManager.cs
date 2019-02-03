using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Processes requests for movement and executes callbacks.
public class PathRequestManager : MonoBehaviour {

    public Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();
    public PathRequest currentPathRequest;

    public static PathRequestManager instance;

    bool isProcessingPath;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (this);
        }
    }

    // Our method which requests to start a new path find from AStar.  For optimization, we use callbacks
    // so that we can process these requests over multiple frames.
    public static void RequestPath (Vector3 pathStart, Vector3 _pathEnd,
        Action<Vector3[], bool, Unit, Action<Unit>> _callback, MovementHandler _movementHandler, Unit _unit, Action<Unit> onDestReached) {
        // When we receive the movement request, we create a new request and process it with enqueue/trynext
        PathRequest _newRequest = new PathRequest (pathStart, _pathEnd, _callback, onDestReached);
        instance.pathRequestQueue.Enqueue (_newRequest);
        instance.TryProcessNext (_movementHandler, _unit, onDestReached);
    }

    void TryProcessNext (MovementHandler movementHandler, Unit _unit, Action<Unit> onDestReached) {
        // if we aren't already processing a request and there is a request to process...
        if (processing ()) {
            NewPathLogic ();
            movementHandler.GenerateMovementPath (currentPathRequest.pathStart, currentPathRequest.pathEnd, _unit, onDestReached);
        } else {
            Debug.LogError ("cant proccess 2 requests at the same time");
        }
    }

    public void NewPathLogic () {
        currentPathRequest = pathRequestQueue.Dequeue ();
        isProcessingPath = true;
    }

    public bool processing () {
        return !isProcessingPath && pathRequestQueue.Count > 0;
    }

    // Our method to finish path logic on the request manager, and pass a callback method
    // to whatever unit was asking for the path in the first place.
    public void FinishedProcessingPath (Vector3[] path, bool success, Unit unit, Action<Unit> onDestReached, MovementHandler movementHandler) {
        // Pass the path vector3[] and status of success/failure to the interested unit
        currentPathRequest.callback (path, success, unit, onDestReached);
        // Reset our manager's statemachine so it can accept/process additional path requests.
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

        public PathRequest (Vector3 _start, Vector3 _end, Action<Vector3[], bool, Unit, Action<Unit>> _callback = null, Action<Unit> _onDestReached = null) {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            onDestReached = _onDestReached;
        }

    }
}