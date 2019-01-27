using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_InputHandler : MonoBehaviour {
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
	}

	public bool ValidUnitState (Unit _unit) {
		if (_unit.currentUnitState == Unit.UnitState.idle) {
			return true;
		} else {
			return false;
		}
	}

	public void SelectUnit (Unit _selectedUnit) {
		UnitSelectionHandler.SetSelectionForAI (_selectedUnit, Unit.SelectionState.selected, null);
		return;
	}

	public void PrepActionData (Unit _unit, Ability _ability) {
		unitStateHandler.SetAttackDataAI (_unit.GetComponent<AbilityManager> ().ReturnAbilityInfo ());
		unitStateHandler.SetAbilAI (_ability);
		unitStateHandler.SetStateAIUnit (_unit, Unit.UnitState.planningAction);
		unitStateHandler.SetAbilSlotAI (_unit, _unit.GetComponent<AbilityManager> ().ReturnCurrentAttack ());
	}

	public void InitiateAbility (Unit unit, Ability ability) {
		if (unit.currentUnitState == Unit.UnitState.planningAction) {
			abilityTargeting.ValidateAbilityAI (ability);
			CallAbility (
				selectedUnit: unit,
				startPos: unit.transform.position,
				targetPos: ability.abilityInfo.infoTheSecond.targetPos,
				slot: unitStateHandler.curAIAbilSlot
			);
		} else {
			Debug.LogError ("Wrong state");
		}
	}

	public void CallAbility (Unit selectedUnit, Vector3 startPos, Vector3 targetPos, int slot) {
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
	}
}