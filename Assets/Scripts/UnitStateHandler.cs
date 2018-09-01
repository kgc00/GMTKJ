using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitStateHandler : MonoBehaviour
{
    Unit actor;
    SceneManager sceneManager;
	GameGrid grid;
	DebugGizmo gizmothing;
    // Use this for initialization
    void Start()
    {
        sceneManager = FindObjectOfType<SceneManager>().GetComponent<SceneManager>();
		grid = GameGrid.instance;
		gizmothing = DebugGizmo.instance;
        Debug.Assert(actor = GetComponentInParent<Unit>());			
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set our bools to true, return a new node path to draw, remove movement points, and update the text on the side of the screen
    public void SetMoving(bool isMoving, int movementPointsUsed)
    {
        actor.currentMovementPoints -= movementPointsUsed;
        sceneManager.UpdateText(actor.currentMovementPoints);
        // Because we call this function from different locations, change the enum state based on requirements 
        // of the statemachine.
        if (!isMoving)
        {
            // Switch our statemachine
            actor.currentUnitState = Unit.UnitState.idle;
            foreach (Node node in grid.grid)
            {
                ResetCosts(node);
            }
            // Reset value so the next time we display the legal moves a path isn't already there
            gizmothing.path = new List<Node>();
        }
        else
        {
            // Turn off path display
            gizmothing.playerRequestingPath = false;
            // Switch out state machine
            actor.currentUnitState = Unit.UnitState.moving;
        }
    }

    // Called to reset state logic when user "ends their turn".
    public void NextTurn()
    {
        actor.currentMovementPoints = actor.maxMovementPointsPerTurn;
        SetMoving(false, 0);
        sceneManager.UpdateText(actor.currentMovementPoints);
    }

    // Necessary because we calculate path costs multiple times in our game.
    private static void ResetCosts(Node node)
    {
        node.gCost = 0;
        node.hCost = 0;
    }

    // Reset our lists so we get correct, clean data for gizmo draws and function calls next turn.
    public void ResetLists(List<Node> nodesInRange)
    {
        nodesInRange = new List<Node>();
        gizmothing._nodesWithinRange = nodesInRange;
        gizmothing.path = new List<Node>();
    }
}
