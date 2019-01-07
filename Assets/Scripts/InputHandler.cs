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
    void Start () {
        grid = GameGrid.instance;
        abilityTargeting = FindObjectOfType<AbilityTargeting> ().GetComponentInChildren<AbilityTargeting> ();
        unitStateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
        if (target == null) {
            target = FindObjectOfType<TargetPosition> ().transform;
        }
    }

    void Update () {
        if (WorldManager.instance.ReturnUnitSelected ()) {
            if (WorldManager.ReturnSelectedUnit () != selectedUnit) {
                selectedUnit = WorldManager.ReturnSelectedUnit ();
            }
            if (selectedUnit.currentUnitState == Unit.UnitState.planningAction) {
                abilityTargeting.HandleAbilityInput ();
                HandleCallAbility (selectedUnit: selectedUnit, startPos: selectedUnit.transform.position,
                    targetPos: grid.NodeFromWorldPosition (target.position).transform.position, slot: unitStateHandler.curAbilSlot);
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
            unitStateHandler.SetState (_unit, Unit.UnitState.idle);
            UnitSelectionHandler.SetSelection (_unit, Unit.SelectionState.notSelected, null);
            return;
        }
    }

    private bool CallForAnimation (Unit _unit, int v) {
        return _unit.GetComponent<AbilityManager> ().AnimateAbilitySelection (v);
    }

    private void PrepActionData (Unit _unit, int _abilitySlot) {
        CallForAnimation (_unit, _abilitySlot);
        unitStateHandler.SetAttackData (_unit.GetComponent<AbilityManager> ().ReturnAbilityInfo ());
        unitStateHandler.SetAbil (_unit.GetComponent<AbilityManager> ().ReturnAbility ());
        unitStateHandler.SetState (_unit, Unit.UnitState.planningAction);
        unitStateHandler.SetAbilSlot (_unit, _abilitySlot);
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
            unitStateHandler.curAbil.abilityInfo.infoTheSecond = abilityTargeting.CacheRelevantInfo (startPos, targetPos, slot);
            // if selected node is a valid node to travel
            if (unitStateHandler.curAbil.abilityInfo.nodesInAbilityRange != null &&
                unitStateHandler.curAbil.abilityInfo.nodesInAbilityRange.Contains (grid.NodeFromWorldPosition (targetPos))) {
                unitStateHandler.curAbil.OnCommited (selectedUnit);
            } else {
                Debug.Log ("not a valid selection");
            }
        }
    }

    private static void SelectUnit (Unit _selectedUnit) {
        UnitSelectionHandler.SetSelection (_selectedUnit, Unit.SelectionState.selected, null);
        return;
    }
}