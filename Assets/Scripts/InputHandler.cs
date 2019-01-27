using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    public Transform target;
    GameGrid grid;
    Unit selectedUnit;
    UnitStateHandler unitStateHandler;
    AbilityTargeting abilityTargeting;
    GridEffects gridFX;
    void Start () {
        grid = GameGrid.instance;
        gridFX = FindObjectOfType<GridEffects> ().GetComponentInChildren<GridEffects> ();
        abilityTargeting = FindObjectOfType<AbilityTargeting> ().GetComponentInChildren<AbilityTargeting> ();
        unitStateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
        if (target == null) {
            target = FindObjectOfType<TargetPosition> ().transform;
        }
    }

    void Update () {
        if (WorldManager.instance.ReturnUnitSelected ()) {
            if (WorldManager.ReturnSelectedPlayerUnit () != selectedUnit) {
                selectedUnit = WorldManager.ReturnSelectedPlayerUnit ();
            }
            if (selectedUnit.currentUnitState == Unit.UnitState.planningAction) {
                abilityTargeting.HandleAbilityInput ();
                HandleCallAbility (
                    selectedUnit: selectedUnit,
                    startPos: selectedUnit.transform.position,
                    targetPos: target.position,
                    slot: unitStateHandler.curPlayerAbilSlot
                );
                return;
            }

            if (ValidSelectedState (selectedUnit)) {
                SelectedLogic (selectedUnit);
            }
        } else {
            SelectionLogic ();
        }
    }

    private void SelectedLogic (Unit _unit) {
        if (Input.GetKeyDown (KeyCode.Alpha1)) {
            PrepActionData (_unit, 0);
            return;
        } else if (Input.GetKeyDown (KeyCode.Alpha2)) {
            PrepActionData (_unit, 1);
            return;
        } else if (Input.GetKeyDown (KeyCode.Alpha3)) {
            PrepActionData (_unit, 2);
            return;
        } else if (Input.GetKeyDown (KeyCode.Alpha4)) {
            PrepActionData (_unit, 3);
            return;
        } else if (Input.GetKeyDown (KeyCode.Escape)) {
            unitStateHandler.SetStatePlayerUnit (_unit, Unit.UnitState.idle);
            UnitSelectionHandler.SetSelectionForPlayer (_unit, Unit.SelectionState.notSelected, null);
            return;
        }
    }

    private bool CallForAnimation (Unit _unit, int v) {
        return _unit.GetComponent<AbilityManager> ().AnimateAbilitySelection (v);
    }

    private void PrepActionData (Unit _unit, int _abilitySlot) {
        CallForAnimation (_unit, _abilitySlot);
        unitStateHandler.SetAttackDataPlayer (_unit.GetComponent<AbilityManager> ().ReturnAbilityInfo ());
        unitStateHandler.SetAbilPlayer (_unit.GetComponent<AbilityManager> ().ReturnAbility ());
        unitStateHandler.SetStatePlayerUnit (_unit, Unit.UnitState.planningAction);
        unitStateHandler.SetAbilSlotPlayer (_unit, _abilitySlot);
    }

    private bool ValidSelectedState (Unit _unit) {
        if (_unit.currentSelectionState == Unit.SelectionState.selected) {
            if (
                _unit.currentUnitState == Unit.UnitState.planningAction ||
                _unit.currentUnitState == Unit.UnitState.idle) {
                return true;
            }
            return false;
        } else {
            return false;
        }
    }

    private void SelectionLogic () {
        if (Input.GetMouseButtonDown (0)) {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, 50, 1 << 11)) {
                Unit _selectedUnit = hit.transform.parent.gameObject.GetComponent<Unit> ();
                // need to add spport for enemy/ally distinction
                if (_selectedUnit.currentUnitState == Unit.UnitState.idle) {
                    SelectUnit (_selectedUnit);
                }
            }
        }
    }

    public void HandleCallAbility (Unit selectedUnit, Vector3 startPos, Vector3 targetPos, int slot) {
        if (Input.GetMouseButtonDown (0)) {
            unitStateHandler.curPlayerAbil.abilityInfo.infoTheSecond = abilityTargeting.CacheRelevantInfo (startPos, targetPos, slot);
            // if selected node is a valid node to travel
            if (unitStateHandler.curPlayerAbil.abilityInfo.nodesInAbilityRange != null &&
                unitStateHandler.curPlayerAbil.abilityInfo.nodesInAbilityRange.Contains (
                    grid.NodeFromWorldPosition (targetPos)
                )) {
                unitStateHandler.curPlayerAbil.OnCommited (selectedUnit);
            } else {
                Debug.Log (grid.NodeFromWorldPosition (targetPos) + " is not a valid selection");
            }
        } else if (Input.GetKeyDown (KeyCode.Escape)) {
            // set state to idle, keep unit selected, reset nodes
            unitStateHandler.SetStatePlayerUnit (selectedUnit, Unit.UnitState.idle);
            grid.ResetNodeCosts ();
            gridFX.ClearHighlights ();
        }
    }

    private static void SelectUnit (Unit _selectedUnit) {
        UnitSelectionHandler.SetSelectionForPlayer (_selectedUnit, Unit.SelectionState.selected, null);
        return;
    }
}