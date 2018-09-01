using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum UnitState { ready, idle, moving };
    [SerializeField]
    public UnitState currentUnitState;
    public int maxMovementPointsPerTurn, currentMovementPoints, movementCost;

    void Start()
    {
        // Set enum so we can have some control over player actions
        currentUnitState = UnitState.idle;
        // Movementvariables declared here
        movementCost = 0;
        maxMovementPointsPerTurn = 6;
        currentMovementPoints = maxMovementPointsPerTurn;
    }
}
