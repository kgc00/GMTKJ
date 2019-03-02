using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Archer/PiercingShot")]
public class PiercingShot : AttackAbility {
	UnitStateHandler stateHandler;
	AbilityTargeting abilityTargeting;
	GameGrid grid;
	GridEffects gridFX;
	AttackHandler attackHandler;
	UnitTimer timer;
	Unit owner;
	[SerializeField]
	int targetRadius;

	public override void OnCalled (Unit unit) {
		SetRefs (unit);
		abilityInfo.nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting (unit, this);
		gridFX.InitiateAbilityHighlights (unit, abilityInfo.nodesInAbilityRange);
	}

	public override void OnCommited (Unit unit) {
		unit.SetCurrentAbility (this);
		stateHandler.SetUnitState (owner, Unit.UnitState.acting);
		CreateProjectile (unit);
		unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
		OnFinished (owner);
	}
	private void CreateProjectile (Unit unit) {
		GameObject go = new GameObject ("piercing arrow");
		// go.AddComponent<Sprite>();
		PiercingArrow arrowProjectile = go.AddComponent<PiercingArrow> ();
		SphereCollider so = go.AddComponent<SphereCollider> ();
		so.isTrigger = true;
		so.radius = .25f;
		go.transform.transform.position = abilityInfo.infoTheSecond.startPos;

		arrowProjectile.FireProjectile (abilityInfo.infoTheSecond.startPos, abilityInfo.infoTheSecond.targetPos, unit, Impact);
	}
	public void Impact (Vector3 impactPoint) {
		List<Node> nodesImpacted = grid.GetAOEExtendingRange (
			startingNode: grid.NodeFromWorldPosition (
				abilityInfo.infoTheSecond.startPos),
			range : targetRadius,
			targetNode : grid.NodeFromWorldPosition (
				impactPoint));
		foreach (Node targetNode in nodesImpacted) {
			targetNode.GetComponent<SpriteRenderer> ().enabled = false;
			if (grid.UnitFromNode (targetNode)) {
				OnAbilityConnected (grid.UnitFromNode (targetNode));
			}
		}
		// spawns whatever visual special effects
	}
	public override void OnAbilityConnected (Unit targetedUnit) {
		attackHandler.DealDamage (targetedUnit, owner);
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
		if (!attackHandler) {
			attackHandler = FindObjectOfType<AttackHandler> ().GetComponent<AttackHandler> ();
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