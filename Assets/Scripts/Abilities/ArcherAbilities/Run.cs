using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Archer/Run")]
public class Run : MovementAbility {

	UnitStateHandler stateHandler;
	AbilityTargeting abilityTargeting;
	GameGrid grid;
	GridEffects gridFX;
	UnitMovement unitMovement;
	MovementHandler movementHandler;
	UnitTimer timer;
	Unit owner;

	public override void OnCalled (Unit unit) {
		SetRefs (unit);
		abilityInfo.nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting (unit, this);
		gridFX.InitiateAbilityHighlights (unit, abilityInfo.nodesInAbilityRange);
	}

	public override void OnCommited (Unit unit) {
		stateHandler.SetState (owner, Unit.UnitState.acting);
		unitMovement.CommitMovement (abilityInfo.infoTheSecond.startPos,
			abilityInfo.infoTheSecond.targetPos,
			owner,
			OnDestinationReached
		);
		unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
	}

	public override void OnDestinationReached (Unit unit) {
		OnFinished (unit);
	}

	public override void OnFinished (Unit unit) {
		stateHandler.SetState (unit, Unit.UnitState.cooldown);
		timer.AddTimeToTimerAbil (unit, abilityInfo.cooldownTime);
		Debug.Log ("onFinished was called");
	}

	private void SetRefs (Unit unit) {
		if (!stateHandler) {
			stateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
		}
		if (!abilityTargeting) {
			abilityTargeting = FindObjectOfType<AbilityTargeting> ().GetComponent<AbilityTargeting> ();
		}
		if (!unitMovement) {
			unitMovement = FindObjectOfType<UnitMovement> ().GetComponent<UnitMovement> ();
		}
		if (!movementHandler) {
			movementHandler = FindObjectOfType<MovementHandler> ().GetComponent<MovementHandler> ();
		}
		if (!timer) {
			timer = FindObjectOfType<UnitTimer> ().GetComponent<UnitTimer> ();
		}
		if (!gridFX) {
			gridFX = FindObjectOfType<GridEffects> ().GetComponent<GridEffects> ();
		}
		grid = GameGrid.instance;
		owner = unit;
	}
}