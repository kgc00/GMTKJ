using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    List<Unit> allUnits;
    List<Unit> allPlayerUnits;
    List<Unit> allAIUnits;
    private Unit selectedUnit;
    AI_Manager AI_Manager;
    public static WorldManager instance;
    [SerializeField]
    private bool anyUnitSelectedByPlayer = false;
    public static event Func<bool> onRequestGridState;

    void Start () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (this);
        }
        UnitSelectionHandler.onUnitSelectedByPlayer += UnitSelectedByPlayer;
        UnitSelectionHandler.onUnitSelectedByPlayer += DeselectOtherPlayerUnits;
        UnitSelectionHandler.onUnitUnselectedByPlayer += SetNoPlayerUnitsSelected;
        UnitStateHandler.onUnitActing += SetNoUnitsSelected;
        Unit.OnUnitDeath += RemoveUnitFromList;
        AI_Manager = FindObjectOfType<AI_Manager> ().GetComponent<AI_Manager> ();
        allUnits = new List<Unit> (FindObjectsOfType<Unit> ());
        allPlayerUnits = new List<Unit> ();
        allAIUnits = new List<Unit> ();
        foreach (Unit unit in allUnits) {
            if (unit.faction == Unit.Faction.Player) {
                allPlayerUnits.Add (unit);
            } else {
                allAIUnits.Add (unit);
            }
        }
        if (allAIUnits != null && AI_Manager != null) {
            AI_Manager.Store_AI_Units (allAIUnits);
        }
        // Debug.Log (allPlayerUnits.Count + " " + allAIUnits.Count);
        AlignUnitsToGrid (allUnits);
    }

    private void AlignUnitsToGrid (List<Unit> allUnits) {
        GameGrid grid = GameGrid.instance;
        foreach (Unit unit in allUnits) {
            Node unitNode = grid.NodeFromWorldPosition (unit.transform.position);
            unit.transform.position = unitNode.transform.position;
        }
    }

    public void AddUnitToList (Unit unit) {
        if (!allUnits.Contains (unit)) { allUnits.Add (unit); }
    }
    public void RemoveUnitFromList (Unit unit) {
        if (allUnits.Contains (unit)) { allUnits.Remove (unit); }
        if (allPlayerUnits.Contains (unit)) { allPlayerUnits.Remove (unit); }
        if (allAIUnits.Contains (unit)) { allAIUnits.Remove (unit); }
    }
    private void UnitSelectedByPlayer (Unit _selectedUnit) {
        _selectedUnit.currentSelectionState = Unit.SelectionState.selected;
        anyUnitSelectedByPlayer = true;
        selectedUnit = _selectedUnit;
    }

    public void SetNoPlayerUnitsSelected (Unit unit) {
        anyUnitSelectedByPlayer = false;
    }

    private void SetNoUnitsSelected (Unit unit, Ability abil) {
        switch (unit.faction) {
            case Unit.Faction.Player:
                anyUnitSelectedByPlayer = false;
                break;
            case Unit.Faction.Enemy:
                break;
            default:
                break;
        }
    }

    public List<Unit> GetAllUnits () {
        return allUnits;
    }

    public List<Unit> GetAllPlayerUnits () {
        return allPlayerUnits;
    }

    public static Unit ReturnSelectedPlayerUnit () {
        return WorldManager.instance.selectedUnit;
    }

    public bool ReturnUnitSelected () {
        return anyUnitSelectedByPlayer;
    }

    public bool ReturnShouldDisplayGrid () {
        return onRequestGridState ();
    }

    public void DeselectOtherPlayerUnits (Unit _selectedUnit) {
        foreach (Unit unit in allPlayerUnits) {
            if (unit != _selectedUnit && unit.currentSelectionState == Unit.SelectionState.selected) {
                unit.currentSelectionState = Unit.SelectionState.notSelected;
            }
        }
    }
    public void DeselectOtherAIUnits (Unit _selectedUnit) {
        foreach (Unit unit in allAIUnits) {
            if (unit != _selectedUnit && unit.currentSelectionState == Unit.SelectionState.selected) {
                unit.currentSelectionState = Unit.SelectionState.notSelected;
            }
        }
    }
}