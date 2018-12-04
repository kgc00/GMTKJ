using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Processes requests for movement and executes callbacks.
public class PathRequestManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    public static PathRequestManager instance;

    bool isProcessingPath;

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
    }

    // Our method which requests to start a new path find from AStar.  For optimization, we use callbacks
    // so that we can process these requests over multiple frames.
    public static void RequestPath(Vector3 pathStart, Vector3 _pathEnd,
    Action<Vector3[], bool, Unit> _callback, MovementHandler _movementHandler, Unit _unit)
    {
        // When we receive the movement request, we create a new request and process it with enqueue/trynext
        PathRequest _newRequest = new PathRequest(pathStart, _pathEnd, _callback);
        instance.pathRequestQueue.Enqueue(_newRequest);
        instance.TryProcessNext(_movementHandler, _unit);
    }

    void TryProcessNext(MovementHandler movementHandler, Unit _unit)
    {
        // if we aren't already processing a request and there is a request to process...
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            // parse the current request.
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            // Start pathfinding logic!
            movementHandler.GenerateMovementPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, _unit);
        }
    }

    // Our method to finish path logic on the request manager, and pass a callback method
    // to whatever unit was asking for the path in the first place.
    public void FinishedProcessingPath(Vector3[] path, bool success, Unit unit, MovementHandler movementHandler)
    {
        // Pass the path vector3[] and status of success/failure to the interested unit
        currentPathRequest.callback(path, success, unit);
        // Reset our manager's statemachine so it can accept/process additional path requests.
        isProcessingPath = false;
        TryProcessNext(movementHandler, unit);
    }

    // Data structure which contains all the information necessary to communicate a complex command
    // between AStar, RequestManager, and Unit.
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool, Unit> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool, Unit> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }

    }
}