using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{

    void Start()
    {
        AbilityTargeting.onCommitToAttack += HandleCommitToAttack;
    }

    // need to figure out how to handle extremely different cases
    private void HandleCommitToAttack(Node _targetNode, Unit _attackingUnit, Ability _abilityUsed, Unit _targetUnit)
    {
        if (_targetUnit)
        {
            DealDamage(_targetUnit, _attackingUnit);
            if (_abilityUsed is AttackAbility)
            {
                AttackAbility _attackAbility = (AttackAbility)_abilityUsed;
                _attackAbility.OnAbilityConnected(_targetUnit);
            }
        }
        FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>().AttackFinished(_attackingUnit);
    }

    public void DealDamage(Unit _targetUnit, Unit _attackingUnit)
    {
        int _incomingDamage = _attackingUnit.attackPower;
        _targetUnit.TakeDamage(_incomingDamage);
    }
}
