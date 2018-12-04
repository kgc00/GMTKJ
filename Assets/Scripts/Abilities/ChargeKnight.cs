using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Ability/Knight/ChargeKnight")]
public class ChargeKnight : AttackAbility
{
    UnitStateHandler stateHandler;
    AbilityTargeting abilityTargeting;
    GameGrid grid;
    UnitMovement unitMovement;
    AttackHandler attackHandler;
    MovementHandler movementHandler;
    List<Node> nodesTraveled;
    Node nodeBeforeCollision;
    UnitTimer timer;
    Unit owner;

    public override void OnCalled(Unit unit)
    {
        if (!stateHandler)
        {
            stateHandler = FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>();
        }
        if (!abilityTargeting)
        {
            abilityTargeting = FindObjectOfType<AbilityTargeting>().GetComponent<AbilityTargeting>();
        }
        if (!attackHandler)
        {
            attackHandler = FindObjectOfType<AttackHandler>().GetComponent<AttackHandler>();
        }
        if (!unitMovement)
        {
            unitMovement = FindObjectOfType<UnitMovement>().GetComponent<UnitMovement>();
        }
        if (!movementHandler)
        {
            movementHandler = FindObjectOfType<MovementHandler>().GetComponent<MovementHandler>();
        }
        if (!timer)
        {
            timer = FindObjectOfType<UnitTimer>().GetComponent<UnitTimer>();
        }
        grid = GameGrid.instance;
        owner = unit;
        List<Node> _nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting(unit, this);
    }

    public override void OnCommited(Unit unit)
    {
        stateHandler.SetState(unit, Unit.UnitState.acting);
        unitMovement.CommitMovement(abilityInfo.infoTheSecond.startPos,
         abilityInfo.infoTheSecond.targetPos,
         unit);
        unit.EnableDetWithAlerts(OnAbilityConnected, SetNodesTraveled);
    }

    public override void OnAbilityConnected(Unit unit)
    {
        attackHandler.DealDamage(unit, owner);
        SetNewUnitPosition(owner);
    }

    private void SetNewUnitPosition(Unit thisUnit)
    {
        movementHandler.OnStopPath(nodeBeforeCollision.transform.position, thisUnit, OnFinished);
    }

    public void SetNodesTraveled(List<Node> nodes)
    {
        if (nodes[0])
        {
            nodesTraveled = nodes;
            if (nodesTraveled.Count > 1)
            {
                nodeBeforeCollision = nodesTraveled[nodesTraveled.Count - 2];
            }
            else
            {
                nodeBeforeCollision = nodesTraveled[nodesTraveled.Count - 1];
            }
        }
    }

    public override void OnFinished(Unit unit)
    {
        unit.DisableDet();
        stateHandler.SetState(unit, Unit.UnitState.cooldown);
        timer.AddTimeToTimerAbil(unit, abilityInfo.cooldownTime);
    }
}
