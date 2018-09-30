using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{

    void Start()
    {
        // UnitStateHandler.onUnitSelectingAttack += IncomingAttackRequest;
        AttackTargeting.onCommitToAttack += HandleCommitToAttack;
    }

    private void HandleCommitToAttack(Node _targetNode, Unit _attackingUnit, Ability _abilityUsed)
    {
        // switch (switch_on)
        // {

        //     default:
        // }
    }

    private void IncomingAttackRequest(Unit _unit)
    {
        // grab attacks from ability manager
        List<Ability> attackOptions = _unit.GetComponent<AbilityManager>().ReturnEquippedAbilities();
        // Enable ability specific targetting/input....

        // pass input into targetting script
    }

    private void DealDamage(Unit _targetUnit, Unit _attackingUnit)
    {
        int _incomingDamage = _attackingUnit.attackPower;
        _targetUnit.TakeDamage(_incomingDamage);
        // unitStateHandler.AttackFinished(_attackingUnit);
    }

    internal static void HandleDealDamage(Unit targetUnit, Unit attackingUnit)
    {
        throw new NotImplementedException();
    }
}
