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
        UpdateAllUnits();
    }

    private void UpdateAllUnits()
    {
        allUnits = FindObjectsOfType<Unit>();
    }

	public void SetAllUnitStates(){
		foreach (Unit unit in allUnits)
		{
			unit.currentUnitState = Unit.UnitState.unselected;
		}
	}

    // Update is called once per frame
    void Update()
    {

    }
}
