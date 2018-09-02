using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    public enum UnitState { ready, idle, moving, unselected, selected, cooldown, attacking };
    [SerializeField]
    public UnitState currentUnitState;
    public int maxMovementPointsPerTurn, currentMovementPoints, movementCost;
    public event Action<Unit> OnUnitDeath = delegate { };

    void Start()
    {
        currentUnitState = UnitState.unselected;
        movementCost = 0;
        maxMovementPointsPerTurn = 6;
        currentMovementPoints = maxMovementPointsPerTurn;
    }

    void UnitDeath(){
        OnUnitDeath(this);
        Destroy(gameObject, 2.0f);
    }
}
