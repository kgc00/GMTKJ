using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateHandler : MonoBehaviour {
    GameGrid grid;
    WorldManager worldManager;
    public static Action<Unit, Ability> onUnitPlanningAction = delegate { };
    public static Action<Unit, Ability> onUnitActing = delegate { };
    public static Action<Unit, float> onUnitStunned;

    private Ability.AbilityInfo currentPlayerAbilityInfo = new Ability.AbilityInfo ();
    public Ability curPlayerAbil;
    public int curPlayerAbilSlot;
    private Ability.AbilityInfo currentAIAbilityInfo = new Ability.AbilityInfo ();
    public Ability curAIAbil;
    public int curAIAbilSlot;
    void Start () {
        grid = GameGrid.instance;
        worldManager = WorldManager.instance;
    }

    public void SetStatePlayerUnit (Unit _unit, Unit.UnitState _state) {
        _unit.currentUnitState = _state;
        switch (_unit.currentUnitState) {
            case Unit.UnitState.idle:
                SetIdle (_unit, _state);
                break;
            case Unit.UnitState.planningAction:
                SetPlanningAction (_unit, _state);
                break;
            case Unit.UnitState.acting:
                SetActing (_unit, _state);
                break;
            case Unit.UnitState.cooldown:
                SetOnCooldown (_unit, _state);
                break;
            default:
                Debug.LogError ("Unrecognized unit state");
                break;
        }
    }

    internal void SetAbilSlotPlayer (Unit unit, int abilitySlot) {
        curPlayerAbilSlot = abilitySlot;
    }

    internal void SetAbilPlayer (Ability _abil) {
        curPlayerAbil = _abil;
    }

    internal void SetAttackDataPlayer (Ability.AbilityInfo _abilityInfo) {
        currentPlayerAbilityInfo = _abilityInfo;
    }

    public void SetStateAIUnit (Unit _unit, Unit.UnitState _state) {
        _unit.currentUnitState = _state;
        switch (_unit.currentUnitState) {
            case Unit.UnitState.idle:
                SetIdle (_unit, _state);
                break;
            case Unit.UnitState.planningAction:
                SetPlanningAction (_unit, _state);
                break;
            case Unit.UnitState.acting:
                SetActing (_unit, _state);
                break;
            case Unit.UnitState.cooldown:
                SetOnCooldown (_unit, _state);
                break;
            default:
                Debug.LogError ("Unrecognized unit state");
                break;
        }
    }

    internal void SetAttackDataAI (Ability.AbilityInfo _abilityInfo) {
        currentAIAbilityInfo = _abilityInfo;
    }

    internal void SetAbilAI (Ability _abil) {
        curAIAbil = _abil;
    }

    internal void SetAbilSlotAI (Unit unit, int abilitySlot) {
        curAIAbilSlot = abilitySlot;
    }

    private void SetPlanningAction (Unit unit, Unit.UnitState state) {
        grid.ResetNodeCosts ();
        onUnitPlanningAction (unit, curPlayerAbil);
    }

    private void SetActing (Unit unit, Unit.UnitState state) {
        UnitSelectionHandler.SetSelectionForPlayer (unit,
            Unit.SelectionState.notSelected, curPlayerAbil);
        onUnitActing (unit, curPlayerAbil);
    }

    private void SetIdle (Unit _unit, Unit.UnitState _state) { }

    private void SetOnCooldown (Unit _unit, Unit.UnitState _state) {

    }
}