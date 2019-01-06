using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/Knight/ChargeKnight")]
public class ChargeKnight : AttackAbility {
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
        abilityInfo.nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting (unit, this);
        gridFX.InitiateAbilityHighlights (unit, abilityInfo.nodesInAbilityRange);
    }

    public override void OnCommited (Unit unit) {
        stateHandler.SetState (owner, Unit.UnitState.acting);
        unitMovement.CommitMovement (abilityInfo.infoTheSecond.startPos,
            abilityInfo.infoTheSecond.targetPos,
            owner,
            OnFinished
        );
        owner.EnableDetWithAlerts (OnAbilityConnected, SetNodesTraveled);
    }

    public override void OnAbilityConnected (Unit unit) {
        attackHandler.DealDamage (unit, owner);
        SetNewUnitPosition (owner);
        // Debug.Log ("damage has been dealt to: " + unit);
    }

    private void SetNewUnitPosition (Unit thisUnit) {
        movementHandler.OnStopPath (nodeBeforeCollision.transform.position, thisUnit);
    }

    public void SetNodesTraveled (List<Node> nodes) {
        Debug.Log ("passed in: " + nodes.Count);
        if (nodes[0]) {
            nodesTraveled = nodes;
            if (nodesTraveled.Count >= 2) {
                Debug.Log (nodesTraveled.Count);
                nodeBeforeCollision = nodesTraveled[nodesTraveled.Count - 2];
            } else {
                Debug.Log ("nodes count: " + nodesTraveled.Count + ".  0: " + nodesTraveled[0]);
                nodeBeforeCollision = nodesTraveled[0];
            }
        }
    }

    public override void OnFinished (Unit unit) {
        unit.DisableDet (this);
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