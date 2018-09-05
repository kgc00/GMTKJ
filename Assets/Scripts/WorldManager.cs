using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    Unit[] allUnits;
    public static WorldManager instance;
    [SerializeField]
    private bool anyUnitSelected = false;
    
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
        UnitStateHandler.onUnitSelected += UnitSelected;
        UnitStateHandler.onUnitPastPlanning += SetNoUnitsSelected;
    }

    public void UnitSelected(Unit selectedUnit)
    {
        anyUnitSelected = true;
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

    private void SetNoUnitsSelected(Unit unit){
        anyUnitSelected = false;
    }

    public bool ReturnUnitSelected(){
        return anyUnitSelected;
    }
}
