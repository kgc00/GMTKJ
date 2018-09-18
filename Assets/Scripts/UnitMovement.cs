using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitMovement : MonoBehaviour
{
    UnitStateHandler unitStateHandler;
    List<Node> nodesInRange;
    GameGrid grid;
    AStar aStar;
    Unit unit;
    InputHandler inputHandler;
    TargetingInformation targetInformation;
    bool gridShoulDisplay = false;
    public static event Action<List<Node>> onGenerateMovementRange = delegate { };
    public static event Action<List<Node>> onGeneratePath = delegate { };

    private void Start()
    {
        unitStateHandler = FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>();
        inputHandler = FindObjectOfType<InputHandler>().GetComponent<InputHandler>();
        aStar = FindObjectOfType<AStar>().GetComponent<AStar>();
        grid = GameGrid.instance;
        inputHandler.onRequestingMovementLogic += MovementLogic;
        UnitStateHandler.onUnitPlanningMovement += DisplayMoves;
        UnitStateHandler.onMovementFinished += ResetNodesInRange;
        UnitStateHandler.onUnitMoving += ResetNodesInRange;
        WorldManager.onRequestGridState += ReturnDisplayGrid;
    }

    public void DisplayMoves(Unit _unit)
    {
        if (_unit.currentMovementPoints > 0)
        {
            GeneratePossibleMoves(_unit.transform.position, _unit.currentMovementPoints);
            gridShoulDisplay = true;
        }
    }

    public void MovementLogic(Vector3 startPos, Vector3 targetPos, Unit _unit)
    {
        if (IsLegalMove(startPos, targetPos))
        {
            DisplayPath(startPos, targetPos, _unit);
        }
        if (IsLegalMove(startPos, targetPos) && Input.GetMouseButtonDown(0) && _unit.currentMovementPoints > 0)
        {
            StoreTargetInfo(startPos, targetPos);
            unitStateHandler.SetState(_unit, Unit.UnitState.moving);
            gridShoulDisplay = false;
        }
    }


    private bool IsLegalMove(Vector3 _startPos, Vector3 _targetPos)
    {
        return nodesInRange.Contains(grid.NodeFromWorldPosition(_targetPos)) &&
                grid.NodeFromWorldPosition(_targetPos) != grid.NodeFromWorldPosition(_startPos) &&
                !grid.nodesContainingUnits.Contains(grid.NodeFromWorldPosition(_targetPos));
    }

    private List<Node> GeneratePossibleMoves(Vector3 startPos, int range)
    {
        if (nodesInRange == null)
        {
            nodesInRange = new List<Node>();
        }
        Node targetNode = grid.NodeFromWorldPosition(startPos);
        // need to alter for attacks
        foreach (Node node in grid.GetRange(targetNode, range))
        {
            if (aStar.PathFindingLogic(false, targetNode, node, range))
            {
                nodesInRange.Add(node);
            }
        }
        onGenerateMovementRange(nodesInRange);
        return nodesInRange;
    }

    void DisplayPath(Vector3 startPos, Vector3 targetPos, Unit _unit)
    {
        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos);
        bool pathSuccess = false;
        if (startNode.walkable && targetNode.walkable && startNode != targetNode)
        {
            pathSuccess = aStar.PathFindingLogic(pathSuccess, startNode, targetNode, _unit.currentMovementPoints);
        }

        if (pathSuccess)
        {
            GetPathToDisplay(startNode, targetNode);
        }
    }

    void GetPathToDisplay(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        onGeneratePath(path);
        path.Reverse();
    }

    private void StoreTargetInfo(Vector3 startingPos, Vector3 targetPos)
    {
        targetInformation = new TargetingInformation(startingPos, targetPos);
    }

    public TargetingInformation PassTargetInfo()
    {
        return targetInformation;
    }

    public void ResetNodesInRange(Unit _unit)
    {
        nodesInRange = new List<Node>();
    }

    private bool ReturnDisplayGrid()
    {
        return gridShoulDisplay;
    }
}
