using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateHandler : MonoBehaviour {
    Unit unit;
    UnitSelectionHandler unitSelectionHandler;
    GameGrid grid;
    WorldManager worldManager;
    public static Action<Unit, Ability> onUnitPlanningAction = delegate { };
    public static Action<Unit, Ability> onUnitActing = delegate { };
    public static Action<Unit, float> onUnitStunned;

    private Ability.AbilityInfo currentAbilityInfo = new Ability.AbilityInfo ();
    public Ability curAbil;
    public int curAbilSlot;
    void Start () {
        unitSelectionHandler = FindObjectOfType<UnitSelectionHandler> ().GetComponent<UnitSelectionHandler> ();
        grid = GameGrid.instance;
        worldManager = WorldManager.instance;
        unit = GetComponent<Unit> ();
    }

    public void SetState (Unit _unit, Unit.UnitState _state) {
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

    internal void SetAbilSlot (Unit unit, int abilitySlot) {
        curAbilSlot = abilitySlot;
    }

    private void SetActing (Unit unit, Unit.UnitState state) {
        UnitSelectionHandler.SetSelection (unit,
            Unit.SelectionState.notSelected, curAbil);
        onUnitActing (unit, curAbil);
    }

    private void SetPlanningAction (Unit unit, Unit.UnitState state) {
        grid.ResetNodeCosts ();
        onUnitPlanningAction (unit, curAbil);
    }

    internal void SetAttackData (Ability.AbilityInfo _abilityInfo) {
        currentAbilityInfo = _abilityInfo;
    }

    internal void SetAbil (Ability _abil) {
        curAbil = _abil;
    }

    private void SetIdle (Unit _unit, Unit.UnitState _state) { }

    private void SetOnCooldown (Unit _unit, Unit.UnitState _state) {

    }
}