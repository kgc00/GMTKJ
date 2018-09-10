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
    public static event Action<List<Node>> onGenerateMovementRange = delegate { };
    public static event Action<List<Node>> onGeneratePath = delegate { };

    private void Start()
    {
        unitStateHandler = GetComponent<UnitStateHandler>();
        inputHandler = GetComponent<InputHandler>();
        aStar = GetComponent<AStar>();
        grid = GameGrid.instance;
        inputHandler.onRequestingMovementLogic += MovementLogic;
        unitStateHandler.onUnitPlanningMovement += DisplayMoves;
        unitStateHandler.onMovementFinished += ResetNodesInRange;
        unitStateHandler.onUnitMoving += ResetNodesInRange;
        unit = GetComponent<Unit>();
    }
    public void DisplayMoves(Unit _unit)
    {
        if (_unit.currentMovementPoints > 0)
        {
            GeneratePossibleMoves(_unit.transform.position, _unit.currentMovementPoints);
        }
    }

    public void MovementLogic(Vector3 startPos, Vector3 targetPos)
    {
        if (IsLegalMove(targetPos))
        {
            DisplayPath(startPos, targetPos);
        }
        if (IsLegalMove(targetPos) && Input.GetMouseButtonDown(0) && unit.currentMovementPoints > 0)
        {
            StoreTargetInfo(startPos, targetPos);
            unitStateHandler.SetState(Unit.UnitState.moving);
        }
    }


    private bool IsLegalMove(Vector3 targetPos)
    {
        return nodesInRange.Contains(grid.NodeFromWorldPosition(targetPos)) &&
                grid.NodeFromWorldPosition(targetPos) != grid.NodeFromWorldPosition(unit.transform.position) &&
                !grid.nodesContainingUnits.Contains(grid.NodeFromWorldPosition(targetPos));
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

    void DisplayPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos);
        bool pathSuccess = false;

        if (startNode.walkable && targetNode.walkable && startNode != targetNode)
        {
            pathSuccess = aStar.PathFindingLogic(pathSuccess, startNode, targetNode, unit.currentMovementPoints);
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

    public void ResetNodesInRange()
    {
        nodesInRange = new List<Node>();
    }
}
