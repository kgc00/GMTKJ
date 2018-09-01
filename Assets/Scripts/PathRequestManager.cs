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
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback, MovementHandler movementHandler)
    {
		// When we receive the movement request, we create a new request and process it with enqueue/trynext
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext(movementHandler);
    }

    void TryProcessNext(MovementHandler movementHandler)
    {
		// if we aren't already processing a request and there is a request to process...
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
			// parse the current request.
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
			// Start pathfinding logic!
            movementHandler.GenerateMovementPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }
	
	// Our method to finish path logic on the request manager, and pass a callback method
	// to whatever unit was asking for the path in the first place.
    public void FinishedProcessingPath(Vector3[] path, bool success, MovementHandler movementHandler)
    {
		// Pass the path vector3[] and status of success/failure to the interested unit
        currentPathRequest.callback(path, success);
		// Reset our manager's statemachine so it can accept/process additional path requests.
        isProcessingPath = false;
        TryProcessNext(movementHandler);
    }

	// Data structure which contains all the information necessary to communicate a complex command
	// between AStar, RequestManager, and Unit.
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }

    }
}