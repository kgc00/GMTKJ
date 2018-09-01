using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum UnitState { ready, idle, moving, unselected, selected };
    [SerializeField]
    public UnitState currentUnitState;
    public int maxMovementPointsPerTurn, currentMovementPoints, movementCost;

    void Start()
    {
        currentUnitState = UnitState.unselected;
        movementCost = 0;
        maxMovementPointsPerTurn = 6;
        currentMovementPoints = maxMovementPointsPerTurn;
    }
}
