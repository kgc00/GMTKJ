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
    UnitTimer timer;
    WorldManager worldManager;
    public static Action<Unit> onUnitSelected = delegate { };
    public static Action<Unit> onUnitUnselected = delegate { };
    public static Action<Unit> onUnitPastPlanning = delegate { };
    public Action onUnitMoving = delegate { };
    public Action onMovementFinished = delegate { };
    public Action<Unit> onUnitPlanningMovement = delegate { };
    public Action onPlanningAttack = delegate { };
    public Action onAttacking = delegate { };
    public Action onAttackFinished = delegate { };

    void Start()
    {
        sceneManager = FindObjectOfType<SceneManager>().GetComponent<SceneManager>();
        grid = GameGrid.instance;
        gizmothing = DebugGizmo.instance;
        worldManager = WorldManager.instance;
        Debug.Assert(unit = GetComponent<Unit>());
        timer = GetComponent<UnitTimer>();
        timer.onTimerRemoved += SetState;
    }

    public void SetState(Unit.UnitState state)
    {
        unit.currentUnitState = state;

        if (!InPrepState(state)) {
            onUnitPastPlanning(unit);
        }

        if (state == Unit.UnitState.selected)
        {
            SetSelected();
        }
        else if (state == Unit.UnitState.moving)
        {
            SetMoving(true);
        }
        else if (state == Unit.UnitState.cooldown)
        {
            SetOnCooldown();
        }
        else if (state == Unit.UnitState.attacking)
        {
            SetAttacking();
        }
        else if (state == Unit.UnitState.unselected)
        {
            SetUnselected();
        }
        else if (state == Unit.UnitState.planningMovement)
        {
            SetPlanningMovement();
        }
        else if (state == Unit.UnitState.planningAttack)
        {
            SetPlanningAttack();
        }
    }

    private static bool InPrepState(Unit.UnitState state)
    {
        return state == Unit.UnitState.selected ||
                state == Unit.UnitState.planningAttack ||
                state == Unit.UnitState.planningMovement;
    }

    private void SetPlanningAttack()
    {
        ResetLists();
        onPlanningAttack();
    }

    private void SetPlanningMovement()
    {
        ResetLists();
        onUnitPlanningMovement(unit);
    }

    private void SetSelected()
    {
        onUnitSelected(unit);
    }

    private void SetAttacking()
    {
        onAttacking();
    }

    private void SetOnCooldown()
    {
    }

    private void SetUnselected()
    {
        foreach (Node node in grid.grid)
        {
            ResetCosts(node);
        }
        onUnitUnselected(unit);
    }

    private void SetMoving(bool isMoving)
    {
        gizmothing.playerRequestingPath = false;
        onUnitMoving();
        ResetLists();
    }

    public void AttackFinished()
    {
        onAttackFinished();
        ResetLists();
        SetState(Unit.UnitState.cooldown);
    }

    public void DestinationReached()
    {
        SetOnCooldown();
        grid.UpdateNodeStatuses();
        foreach (Node node in grid.grid)
        {
            ResetCosts(node);
        }
        onMovementFinished();
        ResetLists();
    }
    public void ConfirmMovement(int movementPointsUsed)
    {
        DestinationReached();
    }

    public void NextTurn()
    {
        unit.currentMovementPoints = unit.maxMovementPointsPerTurn;
        ConfirmMovement(0);
    }

    private static void ResetCosts(Node node)
    {
        node.gCost = 0;
        node.hCost = 0;
    }

    public void ResetLists()
    {
        gizmothing._nodesWithinRange = new List<Node>();
        gizmothing.path = new List<Node>();
        gizmothing.attackTarget = new List<Node>();
    }
}
