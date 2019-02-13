using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour {

    void Start () {
        AbilityTargeting.onCommitToMeleeAttack += HandleCommitToAttack;
        AbilityTargeting.onCommitToMeleeAOEAttack += HandleMeleeAOEAttack;
    }

    // need to figure out how to handle extremely different cases
    private void HandleCommitToAttack (Node _targetNode, Unit _attackingUnit, Ability _abilityUsed, Unit _targetUnit) {
        if (_targetUnit) {
            DealDamage (_targetUnit, _attackingUnit);
            if (_abilityUsed is AttackAbility) {
                AttackAbility _attackAbility = (AttackAbility) _abilityUsed;
                _attackAbility.OnAbilityConnected (_targetUnit);
            }
        } else {
            if (_abilityUsed is AttackAbility) {
                AttackAbility _attackAbility = (AttackAbility) _abilityUsed;
                _attackAbility.OnFinished (_attackingUnit);
            }
        }
    }

    public void DealDamage (Unit _targetUnit, Unit _attackingUnit) {
        int _incomingDamage = _attackingUnit.attackPower;
        _targetUnit.TakeDamage (_incomingDamage);
    }

    public void DealAbilityDamage (Unit _targetUnit, Unit _attackingUnit, int abilityDamage = -1) {
        if (_targetUnit != null) {
            _targetUnit.TakeDamage (abilityDamage);
        }
    }

    private void HandleMeleeAOEAttack (List<Unit> unitsAffected, Unit attackingUnit, AttackAbility ability, Action<Unit> callback = null) {
        foreach (Unit unit in unitsAffected) {
            if (callback != null) {
                callback (unit);
            }
        }
        ability.OnFinished (attackingUnit);
    }
}