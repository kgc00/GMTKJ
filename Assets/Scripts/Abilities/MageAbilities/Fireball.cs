using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Mage/Fireball")]
public class Fireball : AttackAbility {
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
        stateHandler.SetStatePlayerUnit (owner, Unit.UnitState.acting);
        CreateProjectile (unit);
        unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
    }

    private void CreateProjectile (Unit unit) {
        GameObject go = new GameObject ("fireball projectile");
        // go.AddComponent<Sprite>();
        FireballProjectile fireballProjectile = go.AddComponent<FireballProjectile> ();
        SphereCollider so = go.AddComponent<SphereCollider> ();
        so.isTrigger = true;
        so.radius = .25f;
        go.transform.transform.position = abilityInfo.infoTheSecond.startPos;

        fireballProjectile.FireProjectile (abilityInfo.infoTheSecond.startPos, abilityInfo.infoTheSecond.targetPos, unit, Explode);
    }

    public void Explode (Node node) {
        Vector3 impactPoint = node.transform.position;
        List<Node> nodesImpacted = grid.GetNeighbors (node);
        nodesImpacted.Add (node);
        foreach (Node targetNode in nodesImpacted) {
            if (grid.UnitFromNode (targetNode)) {
                OnAbilityConnected (grid.UnitFromNode (targetNode));
            }
        }
        // spawns whatever visual special effects
        OnFinished (owner);
    }

    public override void OnAbilityConnected (Unit targetedUnit) {
        attackHandler.DealDamage (targetedUnit, owner);
    }

    public override void OnFinished (Unit unit) {
        stateHandler.SetStatePlayerUnit (unit, Unit.UnitState.cooldown);
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