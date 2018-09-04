using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    Unit[] allUnits;
    public static WorldManager instance;
    // Use this for initialization
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
        FindAllUnits();
    }

    private void FindAllUnits()
    {
        allUnits = FindObjectsOfType<Unit>();
        foreach (Unit unit in allUnits)
        {
            unit.GetComponent<UnitStateHandler>().onUnitSelected += UnitSelected;
            unit.OnUnitDeath += UnitDestroyed;
        }
    }

    public void UnitSelected(Unit selectedUnit)
    {
        DeselectOtherUnits(selectedUnit);
    }

    public void DeselectOtherUnits(Unit selectedUnit)
    {
        foreach (Unit unit in allUnits)
        {
            if (unit != selectedUnit && unit.currentUnitState == Unit.UnitState.selected)
            {
                unit.GetComponent<UnitStateHandler>().SetState(Unit.UnitState.unselected);
            }
        }
    }

    public void UnitDestroyed(Unit destroyedUnit)
    {
        destroyedUnit.GetComponent<UnitStateHandler>().onUnitSelected -= DeselectOtherUnits;
        destroyedUnit.OnUnitDeath -= UnitDestroyed;
    }

    public bool NoUnitsSelected()
    {
        foreach (Unit unit in allUnits)
        {
            if (unit.currentUnitState == Unit.UnitState.selected ||
            unit.currentUnitState == Unit.UnitState.planningAttack ||
            unit.currentUnitState == Unit.UnitState.planningMovement)
            {
                return false;
            }
        }
        return true;
    }

    // void Update(){
    //     if (Input.GetKeyDown(KeyCode.Space)){
    //         print(AnyUnitSelected());
    //     }
    // }
}
