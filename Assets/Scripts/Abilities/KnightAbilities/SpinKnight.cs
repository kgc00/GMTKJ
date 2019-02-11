using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Knight/SpinKnight")]
public class SpinKnight : AttackAbility {
    UnitStateHandler stateHandler;
    AbilityTargeting abilityTargeting;
    AttackHandler attackHandler;
    Unit owner;
    GridEffects gridFX;
    UnitTimer timer;
    public override void OnCalled (Unit thisUnit) {
        SetRefs (thisUnit);
        abilityInfo.nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting (thisUnit, this);
        gridFX.InitiateAbilityHighlights (thisUnit, abilityInfo.nodesInAbilityRange);

    }
    public override void OnCommited (Unit thisUnit) {
        thisUnit.SetCurrentAbility (this);
        stateHandler.SetUnitState (thisUnit, Unit.UnitState.acting);
        abilityTargeting.CommitToAoEAttack (
            abilityInfo.nodesInAbilityRange, thisUnit, abilityInfo.infoTheSecond.slot, OnAbilityConnected
        );
        thisUnit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
    }
    public override void OnAbilityConnected (Unit targetedUnit) {
        attackHandler.DealDamage (targetedUnit, owner);
    }
    public override void OnFinished (Unit thisUnit) {
        thisUnit.SetCurrentAbility (null);
        stateHandler.SetUnitState (thisUnit, Unit.UnitState.cooldown);
        timer.AddTimeToTimerAbil (thisUnit, abilityInfo.cooldownTime);
        Debug.Log ("onFinished was called");
    }
    private void SetRefs (Unit thisUnit) {
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
            owner = thisUnit;
        }
    }
}