using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Knight/ChargeKnight")]
public class ChargeKnight : MovementAbility {
    UnitStateHandler stateHandler;
    AbilityTargeting abilityTargeting;
    GameGrid grid;
    GridEffects gridFX;
    UnitMovement unitMovement;
    AttackHandler attackHandler;
    MovementHandler movementHandler;
    List<Node> nodesTraveled;
    Node nodeBeforeCollision;
    UnitTimer timer;
    Unit owner;

    public override void OnCalled (Unit unit) {
        SetRefs (unit);
        unit.SetCurrentAbility (this);
        abilityInfo.nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting (unit, this);
        gridFX.InitiateAbilityHighlights (unit, abilityInfo.nodesInAbilityRange);
    }

    public override void OnCommited (Unit unit) {
        unit.SetCurrentAbility (this);
        stateHandler.SetUnitState (owner, Unit.UnitState.acting);
        unitMovement.CommitMovement (abilityInfo.infoTheSecond.startPos,
            abilityInfo.infoTheSecond.targetPos,
            owner,
            OnFinished
        );
        owner.EnableDetWithAlerts (OnAbilityConnected, SetNodesTraveled);
        unit.GetComponent<AbilityManager> ().AnimateAbilityUse (abilityInfo.infoTheSecond.slot);
    }

    public void OnAbilityConnected (Unit unit) {
        attackHandler.DealDamage (unit, owner);
        SetNewUnitPosition (owner);
        movementHandler.OnStopPath (
            grid.NodeFromWorldPosition (unit.transform.position).transform.position,
            unit);
        // WorldManager.AlignUnitsToGrid();
    }

    private void SetNewUnitPosition (Unit thisUnit) {
        movementHandler.OnStopPath (nodeBeforeCollision.transform.position, thisUnit);
        OnDestinationReached (thisUnit);
    }

    public override void OnDestinationReached (Unit unit) {
        if (unit.currentUnitState != Unit.UnitState.cooldown) {
            OnFinished (unit);
        }
    }
    public void SetNodesTraveled (List<Node> nodes) {
        // Debug.Log ("charge passed in: " + nodes.Count);
        if (nodes[0]) {
            nodesTraveled = nodes;
            if (nodesTraveled.Count >= 2) {
                // Debug.Log ("charge passed in: " + nodesTraveled.Count);
                nodeBeforeCollision = nodesTraveled[nodesTraveled.Count - 2];
            } else {
                // Debug.Log ("nodes count: " + nodesTraveled.Count + ".  0: " + nodesTraveled[0]);
                nodeBeforeCollision = nodesTraveled[0];
            }
        }
    }

    public override void OnFinished (Unit unit) {
        unit.SetCurrentAbility (null);
        unit.DisableDet (this);
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