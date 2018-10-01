using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitStateHandler : MonoBehaviour
{
    Unit unit;
    SceneManager sceneManager;
    UnitSelectionHandler unitSelectionHandler;
    GameGrid grid;
    WorldManager worldManager;
    public Dictionary<Unit.UnitState, Action<Unit>> unitStateDictionary;
    public static Action<Unit> onUnitUnselected = delegate { };
    public static Action<Unit> onUnitPastPlanning = delegate { };
    public static Action<Unit> onUnitMoving = delegate { };
    public static Action<Unit> onMovementFinished = delegate { };
    public static Action<Unit> onUnitPlanningMovement = delegate { };
    public static Action<Unit, Ability.AbilityInfo, Ability> onUnitPlanningAttack = delegate { };
    public static Action<Unit, Ability> onUnitPlanningAction = delegate { };
    public static Action<Unit, Ability> onUnitActing = delegate { };
    public static Action<Unit> onUnitAttacking = delegate { };
    public static Action<Unit> onAttackFinished = delegate { };
    public static Action<Unit> onUnitCoolingDown = delegate { };
    public static Action<Unit> onUnitIdle = delegate { };
    public static Action<Unit, float> onUnitStunned;

    private Ability.AbilityInfo currentAbilityInfo = new Ability.AbilityInfo();
    Ability curAbil;

    void Start()
    {
        #region dictionary
        // unitStateDictionary = new Dictionary<Unit.UnitState, Action<Unit>>(){
        //     {Unit.UnitState.planningMovement, onUnitPlanningMovement},
        //     {Unit.UnitState.planningAttack, onUnitPlanningAttack},
        //     {Unit.UnitState.moving, onUnitMoving},
        //     {Unit.UnitState.attacking, onUnitAttacking},
        //     {Unit.UnitState.cooldown, onUnitCoolingDown},
        //     {Unit.UnitState.idle, onUnitIdle},
        // };
        #endregion
        sceneManager = FindObjectOfType<SceneManager>().GetComponent<SceneManager>();
        unitSelectionHandler = FindObjectOfType<UnitSelectionHandler>().GetComponent<UnitSelectionHandler>();
        grid = GameGrid.instance;
        worldManager = WorldManager.instance;
        curAbil = new BashKnight();
        unit = GetComponent<Unit>();
    }

    public void SetState(Unit _unit, Unit.UnitState _state)
    {
        _unit.currentUnitState = _state;
        switch (_unit.currentUnitState)
        {
            case Unit.UnitState.idle:
                SetIdle(_unit, _state);
                break;
            case Unit.UnitState.planningMovement:
                SetPlanningMovement(_unit, _state);
                break;
            case Unit.UnitState.planningAttack:
                SetPlanningAttack(_unit, _state);
                break;
            case Unit.UnitState.moving:
                SetMoving(_unit);
                UnitSelectionHandler.SetSelection(_unit, Unit.SelectionState.notSelected);
                break;
            case Unit.UnitState.attacking:
                SetAttacking(_unit, _state);
                UnitSelectionHandler.SetSelection(_unit, Unit.SelectionState.notSelected);
                break;
            case Unit.UnitState.cooldown:
                SetOnCooldown(_unit, _state);
                break;
            case Unit.UnitState.planningAction:
                SetPlanningAction(_unit, _state);
                break;
            case Unit.UnitState.acting:
                SetActing(_unit, _state);
                break;
            default:
                Debug.LogError("Unrecognized unit state");
                break;
        }
    }

    private void SetActing(Unit unit, Unit.UnitState state)
    {
        onUnitActing(unit, curAbil);
    }

    private void SetPlanningAction(Unit unit, Unit.UnitState state)
    {
        onUnitPlanningAction(unit, curAbil);
    }

    internal void GetAttackData(Ability.AbilityInfo _abilityInfo)
    {
        currentAbilityInfo = _abilityInfo;
    }

    internal void GetAbil(Ability _abil)
    {
        curAbil = _abil;
    }

    private void SetIdle(Unit _unit, Unit.UnitState _state)
    {
        onUnitIdle(_unit);
    }

    private void SetPlanningMovement(Unit _unit, Unit.UnitState _state)
    {
        grid.UpdateNodeStatuses(_unit);
        // why every node in grid?  Should be only nodes in range
        foreach (Node node in grid.grid)
        {
            ResetCosts(node);
        }
        onUnitPlanningMovement(_unit);
    }

    private void SetPlanningAttack(Unit _unit, Unit.UnitState _state)
    {
        grid.UpdateNodeStatuses(_unit);
        // why every node in grid?  Should be only nodes in range
        foreach (Node node in grid.grid)
        {
            ResetCosts(node);
        }
        onUnitPlanningAttack(_unit, currentAbilityInfo, curAbil);
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
        onMovementFinished(_unit);
    }

    private static void ResetCosts(Node node)
    {
        node.gCost = 0;
        node.hCost = 0;
    }
}
