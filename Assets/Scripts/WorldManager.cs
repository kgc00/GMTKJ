using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    List<Unit> allUnits;
    private Unit selectedUnit;
    public static WorldManager instance;
    [SerializeField]
    private bool anyUnitSelected = false;
    private bool test = false;
    public static event Func<bool> onRequestGridState;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        UnitSelectionHandler.onUnitSelected += UnitSelected;
        UnitSelectionHandler.onUnitSelected += DeselectOtherUnits;
        UnitStateHandler.onUnitPastPlanning += SetNoUnitsSelected;
        allUnits = new List<Unit>(FindObjectsOfType<Unit>());
    }

    private void UnitSelected(Unit _selectedUnit)
    {
        _selectedUnit.currentSelectionState = Unit.SelectionState.selected;
        anyUnitSelected = true;
        selectedUnit = _selectedUnit;
    }

    private void SetNoUnitsSelected(Unit unit)
    {
        anyUnitSelected = false;
    }

    public static Unit ReturnSelectedUnit()
    {
        return WorldManager.instance.selectedUnit;
    }

    public bool ReturnUnitSelected()
    {
        return anyUnitSelected;
    }

    public bool ReturnShouldDisplayGrid()
    {
        return onRequestGridState();
    }

    public void DeselectOtherUnits(Unit _selectedUnit)
    {
        foreach (Unit unit in allUnits)
        {
            if (unit != _selectedUnit && unit.currentSelectionState == Unit.SelectionState.selected)
            {
                unit.currentSelectionState = Unit.SelectionState.notSelected;
            }
        }
    }
}
