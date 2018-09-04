using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    public enum UnitState
    {
        planningMovement, moving, unselected,
        selected, cooldown, attacking, planningAttack
    };
    [SerializeField]
    public UnitState currentUnitState;
    public int maxMovementPointsPerTurn, currentMovementPoints, movementCost, attackRange, maxHealth, currentHealth, attackPower;
    public event Action<Unit> OnUnitDeath = delegate { };

    public event Action<int, int, int> OnDamageTaken = delegate { };

    void Start()
    {
        currentUnitState = UnitState.unselected;
        movementCost = 0;
        maxMovementPointsPerTurn = 6;
        maxHealth = 3;
        currentHealth = maxHealth;
        attackPower = 1;
        attackRange = 1;
        currentMovementPoints = maxMovementPointsPerTurn;
    }

    void UnitDeath()
    {
        OnUnitDeath(this);
        Destroy(gameObject, 2.0f);
    }

    public void DamageTaken(int incomingDamage)
    {
        OnDamageTaken(currentHealth, maxHealth, incomingDamage);
        currentHealth -= incomingDamage;
        CheckForUnitDeath();
    }

    private void CheckForUnitDeath()
    {
        if (currentHealth <= 0)
        {
            UnitDeath();
        }
    }
}
