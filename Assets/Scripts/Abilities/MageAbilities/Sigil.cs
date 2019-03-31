using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Mage/Sigil")]
public class Sigil : AttackAbility {

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
		unit.SetCurrentAbility (this);
		stateHandler.SetUnitState (owner, Unit.UnitState.acting);
		CreateSigil (unit);
		unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
		OnFinished (owner);
	}

	private void CreateSigil (Unit unit) {
		GameObject go = new GameObject ("Sigil");
		// go.AddComponent<Sprite>();
		SigilObject sigil = go.AddComponent<SigilObject> ();
		SphereCollider so = go.AddComponent<SphereCollider> ();
		so.radius = .25f;
		go.transform.transform.position = abilityInfo.infoTheSecond.targetPos;

		sigil.SpawnSigil (unit, OnAbilityConnected);
	}

	public override void OnAbilityConnected (Unit targetedUnit) {
		Debug.Log ("dealing damage");
		attackHandler.DealDamage (targetedUnit, owner);
	}

	public override void OnFinished (Unit unit) {
		unit.SetCurrentAbility (null);
		stateHandler.SetUnitState (unit, Unit.UnitState.cooldown);
		timer.AddTimeToTimerAbil (unit, abilityInfo.cooldownTime);
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