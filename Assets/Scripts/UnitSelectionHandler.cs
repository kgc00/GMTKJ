using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionHandler : MonoBehaviour {

    public static event Action<Unit> onUnitSelectedByPlayer = delegate { };
    public static event Action<Unit> onUnitSelectedByAI = delegate { };
    public static event Action<Unit> onUnitUnselectedByPlayer = delegate { };
    public static event Action<Unit> onUnitUnselectedByAI = delegate { };
    void Start () {
        UnitStateHandler.onUnitActing += UnselectedParser;
    }

    public static void SetSelectionForPlayer (Unit _unit, Unit.SelectionState _selectionState, Ability curAbil) {
        _unit.currentSelectionState = _selectionState;
        switch (_selectionState) {
            case Unit.SelectionState.selected:
                SetSelectedByPlayer (_unit, _selectionState);
                break;
            case Unit.SelectionState.notSelected:
                SetUnselectedByPlayer (_unit, curAbil);
                break;
            default:
                break;
        }
    }

    public static void SetSelectionForAI (Unit _unit, Unit.SelectionState _selectionState, Ability curAbil) {
        _unit.currentSelectionState = _selectionState;
        switch (_selectionState) {
            case Unit.SelectionState.selected:
                SetSelectedByAI (_unit, _selectionState);
                break;
            case Unit.SelectionState.notSelected:
                SetUnselectedByAI (_unit, curAbil);
                break;
            default:
                break;
        }
    }

    private static void UnselectedParser (Unit _unit, Ability abil) {
        switch (_unit.faction) {
            case Unit.Faction.Player:
                SetUnselectedByPlayer (_unit, abil);
                break;
            case Unit.Faction.Enemy:
                SetUnselectedByAI (_unit, abil);
                break;
            default:
                break;
        }

    }

    private static void SetUnselectedByPlayer (Unit _unit, Ability abil) {
        onUnitUnselectedByPlayer (_unit);
    }

    private static void SetSelectedByPlayer (Unit _unit, Unit.SelectionState _selectionState) {
        onUnitSelectedByPlayer (_unit);
    }

    private static void SetUnselectedByAI (Unit _unit, Ability abil) {
        onUnitUnselectedByAI (_unit);
    }

    private static void SetSelectedByAI (Unit _unit, Unit.SelectionState _selectionState) {
        onUnitSelectedByAI (_unit);
    }

}