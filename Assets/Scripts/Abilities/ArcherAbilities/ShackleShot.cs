using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Archer/ShackleShot")]
public class ShackleShot : AttackAbility {

	UnitStateHandler stateHandler;
	AbilityTargeting abilityTargeting;
	GameGrid grid;
	GridEffects gridFX;
	AttackHandler attackHandler;
	UnitTimer timer;
	Unit owner;
	[SerializeField]
	int targetRadius = -1;
	[SerializeField]
	float stunLengthFull = -1;
	[SerializeField]
	float stunLengthShort = -1;
	List<Unit> unitsImpacted = new List<Unit> ();

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
		GameObject go = new GameObject ("shackle shot");
		// go.AddComponent<Sprite>();
		ShackleProjectile shackleProjectile = go.AddComponent<ShackleProjectile> ();
		SphereCollider so = go.AddComponent<SphereCollider> ();
		so.isTrigger = true;
		so.radius = .25f;
		go.transform.transform.position = abilityInfo.infoTheSecond.startPos;

		shackleProjectile.FireProjectile (abilityInfo.infoTheSecond.startPos, abilityInfo.infoTheSecond.targetPos, unit, Impact);
	}
	public void Impact (Vector3 impactPoint) {
		List<Node> nodesImpacted = grid.GetAOEExtendingRange (
			startingNode: grid.NodeFromWorldPosition (
				abilityInfo.infoTheSecond.startPos),
			range : targetRadius,
			targetNode : grid.NodeFromWorldPosition (
				impactPoint));

		if (unitsImpacted.Count > 0) {
			unitsImpacted.Clear ();
		}

		foreach (Node targetNode in nodesImpacted) {
			if (grid.UnitFromNode (targetNode)) {
				unitsImpacted.Add (grid.UnitFromNode (targetNode));
			}
		}
		OnAbilityConnected (unitsImpacted);
		// spawns whatever visual special effects
	}
	public void OnAbilityConnected (List<Unit> unitsImpacted) {
		int k = 0;
		for (int i = 0; i < unitsImpacted.Count; i++) {
			if (unitsImpacted.Count > 1) {
				if (k < 2) {
					UnitStateHandler.onUnitStunned (unitsImpacted[i], stunLengthFull);
					k++;
				}
			} else {
				UnitStateHandler.onUnitStunned (unitsImpacted[i], stunLengthShort);
			}
			stateHandler.SetUnitState (unitsImpacted[i], Unit.UnitState.cooldown);
		}
	}
	public override void OnFinished (Unit unit) {
		unit.SetCurrentAbility (null);
		stateHandler.SetUnitState (unit, Unit.UnitState.cooldown);
		timer.AddTimeToTimerAbil (unit, abilityInfo.cooldownTime);
		Debug.Log ("onFinished was called");
	}

	public override void OnAbilityConnected (Unit unit) { }

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