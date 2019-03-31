using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Enemy/FireElemental/Spread")]
public class Spread : MovementAbility {

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
        owner.SetCurrentAbility (this);
        stateHandler.SetUnitState (owner, Unit.UnitState.acting);
        abilityInfo.infoTheSecond.slot = owner.GetComponent<AbilityManager> ().GetSlotFromAbility (this);
        stateHandler.SetAttackDataAI (this.abilityInfo);
        unitMovement.CommitMovement (
            abilityInfo.infoTheSecond.startPos,
            abilityInfo.infoTheSecond.targetPos,
            owner,
            OnDestinationReached
        );

        // AI only, no need for animation.  Keeping it here just in case
        // unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
    }

    public override void OnDestinationReached (Unit unit) {
        OnFinished (unit);
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