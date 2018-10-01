using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{

    void Start()
    {
        AttackTargeting.onCommitToAttack += HandleCommitToAttack;
    }

    private void HandleCommitToAttack(Node _targetNode, Unit _attackingUnit, Ability _abilityUsed, Unit _targetUnit)
    {
        if (_targetUnit)
        {
            DealDamage(_targetUnit, _attackingUnit);
            _abilityUsed.OnAbilityConnected(_targetUnit);
        }
        FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>().AttackFinished(_attackingUnit);
    }

    private void DealDamage(Unit _targetUnit, Unit _attackingUnit)
    {
        int _incomingDamage = _attackingUnit.attackPower;
        _targetUnit.TakeDamage(_incomingDamage);
    }
}
