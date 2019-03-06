using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Mage/Teleport")]
public class Teleport : MovementAbility {
	[SerializeField]
	GameObject teleportation;
	[SerializeField]
	GameObject teleportation_reverse;
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
		var nodePosition = grid.NodeFromWorldPosition (abilityInfo.infoTheSecond.targetPos).transform.position;
		var teleportLocation = new Vector3 (
			nodePosition.x,
			nodePosition.y,
			unit.transform.position.z);

		bool nodeHasUnit = grid.UnitFromNode (grid.NodeFromWorldPosition (abilityInfo.infoTheSecond.targetPos));
		bool nodeIsNotWalkable = !grid.NodeFromWorldPosition (abilityInfo.infoTheSecond.startPos).walkable;
		if (nodeHasUnit || nodeIsNotWalkable) {
			return;
		} else {
			unit.SetCurrentAbility (this);
			stateHandler.SetUnitState (owner, Unit.UnitState.acting);
			unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
			unit.transform.position = teleportLocation;
			GameObject go = Instantiate (teleportation);
			go.transform.transform.position = grid.NodeFromWorldPosition (abilityInfo.infoTheSecond.startPos).transform.position;
			Destroy (go, .5f);
			GameObject go2 = Instantiate (teleportation_reverse);
			go2.transform.transform.position = nodePosition;
			Destroy (go2, .5f);
			OnFinished (unit);
		}

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