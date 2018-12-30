using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitSelectionHandler : MonoBehaviour
{

    public static event Action<Unit> onUnitSelected = delegate { };
    public static event Action<Unit> onUnitUnselected = delegate { };
    public static Dictionary<Unit.SelectionState, Action<Unit>> unitSelectionDictionary;
    void Start()
    {
        unitSelectionDictionary = new Dictionary<Unit.SelectionState, Action<Unit>>(){
            {Unit.SelectionState.selected, onUnitSelected},
            {Unit.SelectionState.notSelected, onUnitUnselected},
        };
        UnitStateHandler.onUnitActing += SetUnselected;
    }

    public static void SetSelection(Unit _unit, Unit.SelectionState _selectionState, Ability curAbil)
    {
        _unit.currentSelectionState = _selectionState;
        switch (_selectionState)
        {
            case Unit.SelectionState.selected:
                SetSelected(_unit, _selectionState);
                break;
            case Unit.SelectionState.notSelected:
                SetUnselected(_unit, curAbil);
                break;
            default:
                break;
        }
    }

    private static void SetUnselected(Unit _unit, Ability abil)
    {
        onUnitUnselected(_unit);
    }

    private static void SetSelected(Unit _unit, Unit.SelectionState _selectionState)
    {
        onUnitSelected(_unit);
    }
}
