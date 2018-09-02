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
            unit.GetComponent<UnitStateHandler>().onUnitSelected += DeselectOtherUnits;
            unit.OnUnitDeath += UnitDestroyed;
        }
    }

    public void DeselectOtherUnits(Unit selectedUnit)
    {
        foreach (Unit unit in allUnits)
        {
            if (unit != selectedUnit && unit.currentUnitState == Unit.UnitState.selected)
            {
                unit.currentUnitState = Unit.UnitState.unselected;
            }
        }
    }

    public void UnitDestroyed(Unit destroyedUnit){
        destroyedUnit.GetComponent<UnitStateHandler>().onUnitSelected -= DeselectOtherUnits;
        destroyedUnit.OnUnitDeath -= UnitDestroyed;
        print("wooo");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
