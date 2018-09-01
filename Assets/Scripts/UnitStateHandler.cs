using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Unit))]
public class UnitStateHandler : MonoBehaviour
{
    Unit unit;
    SceneManager sceneManager;
    GameGrid grid;
    DebugGizmo gizmothing;
    WorldManager worldManager;
    public Action onUnitSelected = delegate { };
    public Action onUnitMoving = delegate { };

    // Use this for initialization
    void Start()
    {
        sceneManager = FindObjectOfType<SceneManager>().GetComponent<SceneManager>();
        grid = GameGrid.instance;
        gizmothing = DebugGizmo.instance;
        worldManager = WorldManager.instance;
        Debug.Assert(unit = GetComponent<Unit>());
    }

    public void SetState(Unit.UnitState state)
    {
        worldManager.SetAllUnitStates();
        unit.currentUnitState = state;
        if (state == Unit.UnitState.selected)
        {
            print("selected");
            // display player actions
            onUnitSelected();
        }
        else if (state == Unit.UnitState.moving)
        {
            SetMoving(true);
        }
    }

    private void SetMoving(bool isMoving)
    {
        gizmothing.playerRequestingPath = false;
        unit.currentUnitState = Unit.UnitState.moving;
        onUnitMoving();
    }

    public void ConfirmMovement(int movementPointsUsed)
    {
        unit.currentMovementPoints -= movementPointsUsed;
        // unit.currentUnitState = Unit.UnitState.unselected;
        foreach (Node node in grid.grid)
        {
            ResetCosts(node);
        }
        // Reset value so the next time we display the legal moves a path isn't already there
        gizmothing.path = new List<Node>();
    }

    // Called to reset state logic when user "ends their turn".
    public void NextTurn()
    {
        unit.currentMovementPoints = unit.maxMovementPointsPerTurn;
        ConfirmMovement(0);
        sceneManager.UpdateText(unit.currentMovementPoints);
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
