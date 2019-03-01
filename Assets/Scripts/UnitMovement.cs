using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour {
    UnitStateHandler unitStateHandler;
    GameGrid grid;
    AStar aStar;
    InputHandler inputHandler;
    TargetingInformation targetInformation;
    bool gridShoulDisplay = false;
    MovementHandler movementHandler;

    private void Awake () {
        unitStateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
        inputHandler = FindObjectOfType<InputHandler> ().GetComponent<InputHandler> ();
        aStar = FindObjectOfType<AStar> ().GetComponent<AStar> ();
        movementHandler = FindObjectOfType<MovementHandler> ().GetComponent<MovementHandler> ();
        grid = GameGrid.instance;
    }

    private void Start () {
        WorldManager.onRequestGridState += ReturnDisplayGrid;
    }

    private void OnDestroy () {
        WorldManager.onRequestGridState += ReturnDisplayGrid;
    }

    public void CommitMovement (Vector3 startPos,
        Vector3 targetPos, Unit _unit, Action<Unit> onDestReached) {
        StoreTargetInfo (startPos, targetPos);
        gridShoulDisplay = false;
        movementHandler.StartMovementPathLogic (_unit, onDestReached);
    }

    private void StoreTargetInfo (Vector3 startingPos, Vector3 targetPos) {
        targetInformation = new TargetingInformation (startingPos, targetPos);
    }

    public TargetingInformation PassTargetInfo () {
        return targetInformation;
    }

    private bool ReturnDisplayGrid () {
        return gridShoulDisplay;
    }
}