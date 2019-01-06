﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Knight/BashKnight")]
public class BashKnight : AttackAbility {
    [SerializeField]
    private float lengthOfStun = 2.0f;
    UnitStateHandler stateHandler;
    AbilityTargeting abilityTargeting;
    AttackHandler attackHandler;
    Unit owner;
    GridEffects gridFX;
    UnitTimer timer;

    public override void OnCalled (Unit unit) {
        SetRefs (unit);
        abilityInfo.nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting (unit, this);
        gridFX.InitiateAbilityHighlights (unit, abilityInfo.nodesInAbilityRange);
    }

    public override void OnCommited (Unit unit) {
        stateHandler.SetState (unit, Unit.UnitState.acting);
        abilityTargeting.CommitToAttack (
            abilityInfo.infoTheSecond.startPos, abilityInfo.infoTheSecond.targetPos, abilityInfo.infoTheSecond.slot
        );
        unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
    }

    public override void OnAbilityConnected (Unit targetedUnit) {
        attackHandler.DealDamage (targetedUnit, owner);
        if (targetedUnit.currentUnitState != Unit.UnitState.cooldown) {
            UnitStateHandler.onUnitStunned (targetedUnit, lengthOfStun);
            stateHandler.SetState (targetedUnit, Unit.UnitState.cooldown);
        }
        OnFinished (owner);
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
        if (!gridFX) {
            gridFX = FindObjectOfType<GridEffects> ().GetComponent<GridEffects> ();
        }
        if (!timer) {
            timer = FindObjectOfType<UnitTimer> ().GetComponent<UnitTimer> ();
        }

        if (!owner) {
            owner = unit;
        }
    }

}