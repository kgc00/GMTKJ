﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Archer/PrecisionShot")]
public class PrecisionShot : AttackAbility {
	UnitStateHandler stateHandler;
	AbilityTargeting abilityTargeting;
	GameGrid grid;
	GridEffects gridFX;
	AttackHandler attackHandler;
	UnitTimer timer;
	Unit owner;

	public override void OnCalled (Unit unit) {
		SetRefs (unit);
		abilityInfo.nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting (unit, this);
		gridFX.InitiateAbilityHighlights (unit, abilityInfo.nodesInAbilityRange);
	}

	public override void OnCommited (Unit unit) {
		stateHandler.SetState (owner, Unit.UnitState.acting);
		CreateProjectile (unit);
		unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
		OnFinished (owner);
	}
	private void CreateProjectile (Unit unit) {
		GameObject go = new GameObject ("precision arrow");
		// go.AddComponent<Sprite>();
		PiercingArrow arrowProjectile = go.AddComponent<PiercingArrow> ();
		SphereCollider so = go.AddComponent<SphereCollider> ();
		so.isTrigger = true;
		so.radius = .25f;
		go.transform.transform.position = abilityInfo.infoTheSecond.startPos;

		arrowProjectile.FireProjectile (abilityInfo.infoTheSecond.startPos, abilityInfo.infoTheSecond.targetPos, unit, Impact);
	}
	public void Impact (Vector3 impactPoint) {
		Node impactNode = grid.NodeFromWorldPosition (impactPoint);
		Node targetNode = grid.NodeFromWorldPosition (
			abilityInfo.infoTheSecond.targetPos
		);
		Unit impactedUnit = grid.UnitFromNode (impactNode);
		if (impactNode == targetNode) {
			OnAbilityConnected (impactedUnit, (int) (abilityInfo.attackPower * 1));
		} else {
			OnAbilityConnected (impactedUnit, (int) (abilityInfo.attackPower * .5));
		}
		// spawns whatever visual special effects
	}
	public override void OnAbilityConnected (Unit targetedUnit) { }

	public void OnAbilityConnected (Unit targetedUnit, int dmg) {
		attackHandler.DealAbilityDamage (targetedUnit, owner, dmg);
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