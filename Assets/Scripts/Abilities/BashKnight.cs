using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Knight/BashKnight")]
public class BashKnight : AttackAbility
{
    [SerializeField]
    private float lengthOfStun = 2.0f;
    UnitStateHandler stateHandler;
    AbilityTargeting abilityTargeting;
    AttackHandler attackHandler;
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
        if (!owner)
        {
            owner = unit;
        }
        List<Node> _nodesInAbilityRange = abilityTargeting.InitiateAbilityTargeting(unit, this);
    }

    public override void OnCommited(Unit unit)
    {
        stateHandler.SetState(unit, Unit.UnitState.acting);
        abilityTargeting.CommitToAttack(abilityInfo.infoTheSecond.startPos, abilityInfo.infoTheSecond.targetPos, abilityInfo.infoTheSecond.slot);
    }

    public override void OnAbilityConnected(Unit targetedUnit)
    {
        attackHandler.DealDamage(targetedUnit, owner);
        if (targetedUnit.currentUnitState != Unit.UnitState.cooldown)
        {
            UnitStateHandler.onUnitStunned(targetedUnit, lengthOfStun);
            stateHandler.SetState(targetedUnit, Unit.UnitState.cooldown);
        }
        OnFinished(owner);
    }

    public override void OnFinished(Unit unit)
    {
    }
}
