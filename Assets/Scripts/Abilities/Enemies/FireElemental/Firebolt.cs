using System;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Enemy/FireElemental/Firebolt")]
public class Firebolt : AttackAbility {
    [SerializeField]
    GameObject fireball_GO;
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

        // AI only, no need for animation.  Keeping it here just in case
        // unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
    }

    private void CreateProjectile (Unit unit) {
        GameObject go = Instantiate (fireball_GO);
        FireballProjectile fireballProjectile = go.AddComponent<FireballProjectile> ();
        SphereCollider so = go.AddComponent<SphereCollider> ();
        so.isTrigger = true;
        so.radius = .25f;
        go.transform.position = abilityInfo.infoTheSecond.startPos;
        Vector3 diff =
            grid.NodeFromWorldPosition (abilityInfo.infoTheSecond.targetPos).transform.position -
            abilityInfo.infoTheSecond.startPos;
        diff.Normalize ();
        float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler (0f, 0f, rot_z);
        fireballProjectile.FireProjectile (abilityInfo.infoTheSecond.startPos,
            grid.NodeFromWorldPosition (abilityInfo.infoTheSecond.targetPos).transform.position, unit, GetImpactedUnitFromNode);
        OnFinished (owner);
    }
    public void GetImpactedUnitFromNode (Node node) {
        if (grid.UnitFromNode (node)) {
            OnAbilityConnected (grid.UnitFromNode (node));
        }
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