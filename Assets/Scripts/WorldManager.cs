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
    public static event Func<bool> onRequestGridState;
    private BashKnight dummyAbility;

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
        UnitSelectionHandler.onUnitUnselected += SetNoUnitsSelected;
        UnitStateHandler.onUnitActing += SetNoUnitsSelected;
        Unit.OnUnitDeath += RemoveUnitFromList;
        allUnits = new List<Unit>(FindObjectsOfType<Unit>());
    }

    public void AddUnitToList(Unit unit)
    {
        if (!allUnits.Contains(unit)) { allUnits.Add(unit); }
    }
    public void RemoveUnitFromList(Unit unit)
    {
        if (allUnits.Contains(unit)) { allUnits.Remove(unit); }
    }
    private void UnitSelected(Unit _selectedUnit)
    {
        _selectedUnit.currentSelectionState = Unit.SelectionState.selected;
        anyUnitSelected = true;
        selectedUnit = _selectedUnit;
    }

    public void SetNoUnitsSelected(Unit unit)
    {
        anyUnitSelected = false;
    }

    private void SetNoUnitsSelected(Unit unit, Ability abil)
    {
        anyUnitSelected = false;
    }

    public List<Unit> GetAllUnits()
    {
        return allUnits;
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
