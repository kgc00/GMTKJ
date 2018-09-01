using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class InputHandler : MonoBehaviour
{

    MovementHandler movementHandler;
	Unit actor;
	public Transform target;
	public List<Node> nodesInRange;
	SceneManager sceneManager;
	GameGrid grid;
	DebugGizmo gizmothing;
	AStar aStar;

    // Use this for initialization
    void Start()
    {
		sceneManager = SceneManager.instance;
		grid = GameGrid.instance;
		gizmothing = DebugGizmo.instance;
		Debug.Assert(aStar = GetComponentInParent<AStar>());
		Debug.Assert(actor = GetComponentInParent<Unit>());
        Debug.Assert(movementHandler = GetComponentInParent<MovementHandler>());
    }

    // Update is called once per frame
    void Update()
    {
        // If we can move, and the location we are trying to move to is valid...
        if (actor.currentUnitState == Unit.UnitState.ready && nodesInRange.Contains(grid.NodeFromWorldPosition(target.position)))
        {
            // This function won't actually move, only construct the path and shows the user feedback.
            DisplayPath(actor.transform.position, target.position);

            if (Input.GetMouseButtonDown(0) && actor.currentMovementPoints > 0)
            {
                // Initiate move logic.
                movementHandler.Move();
            }
        }
    }

    // This function preps for movement, dealing with state machine logic
    public void DisplayMoves()
    {
        // Safety check for unit's state
        if (actor.currentUnitState == Unit.UnitState.idle)
        {
            // If we can move, we calculate possibilities for movement
            if (actor.currentMovementPoints > 0)
            {
                GeneratePossibleMoves(actor.transform.position, actor.currentMovementPoints);
                // Update our enum so we can move
                actor.currentUnitState = Unit.UnitState.ready;
                // We need to set this to true to draw the path for player to see.
                gizmothing.playerRequestingPath = true;
            }
            else
            {
                // Let the user know if they have no movement points left
                // sceneManager.UpdateTextEndTurn();
            }
        }
    }

    // This function will try to find the range the unit can move this turn
    private List<Node> GeneratePossibleMoves(Vector3 startPos, int range)
    {
        // Initialize our list if it isn't already set
        if (nodesInRange == null)
        {
            nodesInRange = new List<Node>();
        }

        // Determine our start node and add it to our range
        Node targetNode = grid.NodeFromWorldPosition(startPos);
        // For each node in a grid determined by remaining movement points...
        foreach (Node node in grid.GetRange(targetNode, range))
        {
            // Check and see if the path from that node to the actor is under acceptable limits 
            if (aStar.PathFindingLogic(false, targetNode, node, actor.currentMovementPoints))
            {
                // If within range add to a list we'll use later
                nodesInRange.Add(node);
            }
        }
        // Forward the data to the grid so it can draw the correct nodes
        gizmothing._nodesWithinRange = nodesInRange;
        return nodesInRange;
    }

    // Purely a visual feedback method.  Called on update if unitActor's state is ready to show feedback for user.
    void DisplayPath(Vector3 startPos, Vector3 targetPos)
    {
        // Same old pathfinding logic.

        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos);
        bool pathSuccess = false;

        // Make sure we're clicking on valid targets
        if (startNode.walkable && targetNode.walkable && startNode != targetNode)
        {
            pathSuccess = aStar.PathFindingLogic(pathSuccess, startNode, targetNode, actor.currentMovementPoints);
        }
        if (pathSuccess)
        {
            GetPathToDisplay(startNode, targetNode);
        }
    }

    // Purely a visual feedback method.  Called on update if unitActor's state is ready.
    void GetPathToDisplay(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        gizmothing.path = path;
    }
}
