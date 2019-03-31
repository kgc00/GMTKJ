using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Enemy/Bat/Bite")]
public class Bite : AttackAbility {
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
        unit.SetCurrentAbility (this);
        stateHandler.SetUnitState (unit, Unit.UnitState.acting);
        // somehow if we don't set the slot here it stays as 0 no matter the actual position
        abilityInfo.infoTheSecond.slot = unit.GetComponent<AbilityManager> ().GetSlotFromAbility (this);
        stateHandler.SetAttackDataAI (this.abilityInfo);
        // Debug.Log (abilityInfo.infoTheSecond.slot);
        abilityTargeting.CommitToAttackAI (
            abilityInfo.infoTheSecond.startPos, abilityInfo.infoTheSecond.targetPos, abilityInfo.infoTheSecond.slot
        );

        // AI only, no need for animation.  Keeping it here just in case
        // unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
    }

    public override void OnAbilityConnected (Unit targetedUnit) {
        attackHandler.DealDamage (targetedUnit, owner);
        OnFinished (owner);
    }

    public override void OnFinished (Unit unit) {
        owner.SetCurrentAbility (null);
        stateHandler.SetUnitState (owner, Unit.UnitState.cooldown);
        timer.AddTimeToTimerAbil (owner, abilityInfo.cooldownTime);
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