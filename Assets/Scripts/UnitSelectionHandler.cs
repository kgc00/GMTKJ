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
    }

	public static void SetSelection(Unit _unit, Unit.SelectionState _selectionState){
		if(_selectionState == Unit.SelectionState.selected)
        {
            Selected(_unit, _selectionState);
        }
        else if (_selectionState == Unit.SelectionState.notSelected){
			onUnitUnselected(_unit);
		}
	}

    private static void Selected(Unit _unit, Unit.SelectionState _selectionState)
    {
        onUnitSelected(_unit);
    }
}
