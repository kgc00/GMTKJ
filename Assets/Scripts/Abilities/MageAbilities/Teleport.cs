using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Mage/Teleport")]
public class Teleport : MovementAbility {

	UnitStateHandler stateHandler;
	AbilityTargeting abilityTargeting;
	GameGrid grid;
	GridEffects gridFX;
	UnitTimer timer;
	Unit owner;

	public override void OnCalled (Unit unit) {
		SetRefs (unit);
		abilityInfo.nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting (unit, this);
		gridFX.InitiateAbilityHighlights (unit, abilityInfo.nodesInAbilityRange);
	}

	public override void OnCommited (Unit unit) {
		unit.SetCurrentAbility (this);
		stateHandler.SetUnitState (owner, Unit.UnitState.acting);
		unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
		var nodePosition = grid.NodeFromWorldPosition (abilityInfo.infoTheSecond.targetPos).transform.position;
		var teleportLocation = new Vector3 (
			nodePosition.x,
			nodePosition.y,
			unit.transform.position.z);
		unit.transform.position = teleportLocation;
		OnFinished (unit);
		// vfx
	}
	public override void OnDestinationReached (Unit unit) {

	}
	public override void OnFinished (Unit unit) {
		unit.SetCurrentAbility (null);
		stateHandler.SetUnitState (unit, Unit.UnitState.cooldown);
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