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
        UnitStateHandler.onUnitMoving += SetUnselected;
    }

	public static void SetSelection(Unit _unit, Unit.SelectionState _selectionState){
        _unit.currentSelectionState = _selectionState;
		if(_selectionState == Unit.SelectionState.selected)
        {
            SetSelected(_unit, _selectionState);
        }
        else if (_selectionState == Unit.SelectionState.notSelected)
        {
            SetUnselected(_unit);
        }
    }

    private static void SetUnselected(Unit _unit)
    {
        onUnitUnselected(_unit);
    }

    private static void SetSelected(Unit _unit, Unit.SelectionState _selectionState)
    {
        onUnitSelected(_unit);
    }
}
