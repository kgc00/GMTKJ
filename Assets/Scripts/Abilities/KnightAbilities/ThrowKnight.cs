using System;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Knight/ThrowKnight")]
public class ThrowKnight : AttackAbility {
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
        CreateProjectile (unit);
        unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
    }

    private void CreateProjectile (Unit unit) {
        GameObject go = new GameObject ("throwing thing projectile");
        // go.AddComponent<Sprite>();
        KnightThrownProjectile ktp = go.AddComponent<KnightThrownProjectile> ();
        SphereCollider so = go.AddComponent<SphereCollider> ();
        so.radius = .25f;
        go.transform.transform.position = abilityInfo.infoTheSecond.startPos;

        ktp.thingo (abilityInfo.infoTheSecond.startPos, abilityInfo.infoTheSecond.targetPos, unit, OnAbilityConnected);
        OnFinished (unit);
    }

    public override void OnAbilityConnected (Unit unit) {
        attackHandler.DealDamage (unit, owner);
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