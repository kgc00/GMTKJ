using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour, IDamageable
{
    public enum SelectionState
    {
        selected, notSelected
    };
    public enum UnitState
    {
        planningMovement, planningAttack,
        moving, attacking,
        cooldown, idle
    };
    [SerializeField]
    public UnitState currentUnitState;
    public SelectionState currentSelectionState;
    public int maxMovementPointsPerTurn, currentMovementPoints, movementCost, attackRange, maxHealth, currentHealth, attackPower;
    public static event Action<Unit> OnUnitDeath = delegate { };

    public static event Action<Unit, int, int, int> OnDamageTaken = delegate { };

    void Start()
    {
        currentUnitState = UnitState.idle;
        currentSelectionState = SelectionState.notSelected;
        movementCost = 0;
        maxMovementPointsPerTurn = 6;
        maxHealth = 3;
        currentHealth = maxHealth;
        attackPower = 1;
        attackRange = 1;
        currentMovementPoints = maxMovementPointsPerTurn;
    }

    protected void UnitDeath()
    {
        OnUnitDeath(this);
        Destroy(gameObject, 2.0f);
    }

    public void TakeDamage(int incomingDamage)
    {
        OnDamageTaken(this, currentHealth, maxHealth, incomingDamage);
        currentHealth -= incomingDamage;
        CheckForUnitDeath();
    }

    protected void CheckForUnitDeath()
    {
        if (currentHealth <= 0)
        {
            UnitDeath();
        }
    }
}
