using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_InputHandler : MonoBehaviour {
	UnitStateHandler unitStateHandler;
	AbilityTargeting abilityTargeting;
	GridEffects gridfx;
	private Dictionary<Unit, Coroutine> currentCoroutines;

	void Start () {
		abilityTargeting = FindObjectOfType<AbilityTargeting> ().GetComponentInChildren<AbilityTargeting> ();
		unitStateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
		gridfx = FindObjectOfType<GridEffects> ().GetComponent<GridEffects> ();
		currentCoroutines = new Dictionary<Unit, Coroutine> ();
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

	public void PrepActionData (Unit _unit, Ability _ability, Node _targetNode) {
		unitStateHandler.SetAttackDataAI (_unit.GetComponent<AbilityManager> ().ReturnAbilityInfo ());
		unitStateHandler.SetAbilAI (_ability);
		unitStateHandler.SetUnitState (_unit, Unit.UnitState.planningAction);
		unitStateHandler.SetAbilSlotAI (_unit, _unit.GetComponent<AbilityManager> ().ReturnCurrentAttack ());
		SetInfoTheSecond (_unit, _ability, _targetNode);
	}

	public void PlanAction (Unit _unit, Ability _ability, Node targetNode) {
		_ability.OnCalled (_unit);
		List<Node> nodeList = new List<Node> ();
		nodeList.Add (targetNode);
		gridfx.RenderSelectorHighlights (nodeList, _unit);
	}

	private void SetInfoTheSecond (Unit unit, Ability ability, Node targetNode) {
		AbilityTargetingData dataToUse = new AbilityTargetingData (
			unit.transform.position,
			targetNode.transform.position,
			unitStateHandler.curAIAbilSlot);
		ability.abilityInfo.infoTheSecond = dataToUse;
		unitStateHandler.curAIAbil.abilityInfo.infoTheSecond = dataToUse;
	}

	internal List<Node> ReturnPossibleNodes (Ability abil, Unit unitToControl, Node targetNode) {
		List<Node> possibleNodes =
			abilityTargeting.RequestPathForAI (
				unitToControl,
				targetNode,
				abil);
		return possibleNodes;
	}

	public void InitiateAbility (Unit unit, Ability ability) {
		if (unit.currentUnitState == Unit.UnitState.planningAction) {
			// abilityTargeting.ValidateAbilityAI (ability);
			CallAbility (unit);
		} else {
			Debug.LogError ("Wrong state");
		}
	}

	public void CallAbility (Unit selectedUnit) {
		unitStateHandler.curAIAbil.OnCommited (selectedUnit);
	}

	internal void HandleExecution (Unit unit, Ability ability, float delay) {
		if (currentCoroutines.ContainsKey (unit)) {
			StopCoroutine (currentCoroutines[unit]);
			currentCoroutines.Remove (unit);
		}
		Coroutine thisCoroutine = StartCoroutine (DelayExecution (() => InitiateAbility (unit, ability), delay));
		currentCoroutines.Add (unit, thisCoroutine);
	}

	private IEnumerator DelayExecution (
		Action callback,
		float timeToWait) {
		yield return new WaitForSeconds (timeToWait);
		callback ();
		yield break;
	}
}