using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{

    void Start()
    {
        // UnitStateHandler.onUnitSelectingAttack += IncomingAttackRequest;
    }

    private void IncomingAttackRequest(Unit _unit)
    {
        // grab attacks from ability manager
        List<Ability> attackOptions = _unit.GetComponent<AbilityManager>().ReturnEquippedAbilities();
        // Enable ability specific targetting/input....

        // pass input into targetting script
    }
}
