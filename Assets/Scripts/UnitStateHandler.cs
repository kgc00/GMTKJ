using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitStateHandler : MonoBehaviour
{
    Unit unit;
    SceneManager sceneManager;
    GameGrid grid;
    WorldManager worldManager;
    public Dictionary<Unit.UnitState, Action<Unit>> unitStateDictionary;
    public static Action<Unit> onUnitUnselected = delegate { };
    public static Action<Unit> onUnitPastPlanning = delegate { };
    public static Action<Unit> onUnitMoving = delegate { };
    public static Action<Unit> onMovementFinished = delegate { };
    public static Action<Unit> onUnitPlanningMovement = delegate { };
    public static Action<Unit> onUnitPlanningAttack = delegate { };
    public static Action<Unit> onUnitAttacking = delegate { };
    public static Action<Unit> onAttackFinished = delegate { };
    public static Action<Unit> onUnitCoolingDown = delegate { };
    public static Action<Unit> onUnitIdle = delegate { };

    void Start()
    {
        unitStateDictionary = new Dictionary<Unit.UnitState, Action<Unit>>(){
            {Unit.UnitState.planningMovement, onUnitPlanningMovement},
            {Unit.UnitState.planningAttack, onUnitPlanningAttack},
            {Unit.UnitState.moving, onUnitMoving},
            {Unit.UnitState.attacking, onUnitAttacking},
            {Unit.UnitState.cooldown, onUnitCoolingDown},
            {Unit.UnitState.idle, onUnitIdle},
        };

        sceneManager = FindObjectOfType<SceneManager>().GetComponent<SceneManager>();
        grid = GameGrid.instance;
        worldManager = WorldManager.instance;
        unit = GetComponent<Unit>();
    }

    public void SetState(Unit _unit, Unit.UnitState _state)
    {
        _unit.currentUnitState = _state;
        if (_state == Unit.UnitState.idle)
        {
            SetIdle(_unit, _state);
        }
        else if (_state == Unit.UnitState.planningMovement)
        {
            SetPlanningMovement(_unit, _state);
        }
        else if (_state == Unit.UnitState.planningAttack)
        {
            SetPlanningAttack(_unit, _state);
        }
        else if (_state == Unit.UnitState.moving)
        {
            SetMoving(_unit);
        }
        else if (_state == Unit.UnitState.attacking)
        {
            SetAttacking(_unit, _state);
        }
        else if (_state == Unit.UnitState.cooldown)
        {
            SetOnCooldown(_unit, _state);
        }
    }

    private void SetIdle(Unit _unit, Unit.UnitState _state)
    {
        onUnitIdle(_unit);
    }

    private void SetPlanningMovement(Unit _unit, Unit.UnitState _state)
    {
        onUnitPlanningMovement(_unit);
    }

    private void SetPlanningAttack(Unit _unit, Unit.UnitState _state)
    {
        onUnitPlanningAttack(_unit);
    }
    
    private static bool InPrepState(Unit.UnitState state)
    {
        return state == Unit.UnitState.planningAttack ||
                state == Unit.UnitState.planningMovement;
    }

    private void SetAttacking(Unit _unit, Unit.UnitState _state)
    {
        onUnitAttacking(_unit);
    }

    private void SetOnCooldown(Unit _unit, Unit.UnitState _state)
    {
        onUnitCoolingDown(_unit);
    }

    private void SetMoving(Unit _unit)
    {
        onUnitMoving(_unit);
    }

    public void AttackFinished(Unit _attackingUnit)
    {
        onAttackFinished(_attackingUnit);
        SetState(_attackingUnit, Unit.UnitState.cooldown);
    }

    public void DestinationReached(Unit _unit)
    {
        SetOnCooldown(_unit, Unit.UnitState.cooldown);
        grid.UpdateNodeStatuses();
        // why every node in grid?  Should be only nodes in range
        foreach (Node node in grid.grid)
        {
            ResetCosts(node);
        }
        // 0 subscribers
        // onMovementFinished(_unit);
    }
    public void ConfirmMovement(Unit _unit)
    {
        DestinationReached(_unit);
    }

    public void NextTurn(Unit _unit)
    {
        unit.currentMovementPoints = unit.maxMovementPointsPerTurn;
        ConfirmMovement(_unit);
    }

    private static void ResetCosts(Node node)
    {
        node.gCost = 0;
        node.hCost = 0;
    }
}
