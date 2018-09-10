using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargeting : MonoBehaviour
{

    List<Node> nodesWithinAttackRange = new List<Node>();
    List<Node> nodeAttackTargeting = new List<Node>();
    GameGrid grid;
    Unit unit;
    AStar aStar;
    UnitStateHandler unitStateHandler;
    public static event Action<List<Node>> onGenerateAttackRange = delegate { };


    // Use this for initialization
    void Start()
    {
        grid = GameGrid.instance;
        unit = GetComponent<Unit>();
        aStar = GetComponent<AStar>();
        unitStateHandler = GetComponent<UnitStateHandler>();
        GetComponent<InputHandler>().onRequestingAttackLogic += RequestingAttackLogic;
        unitStateHandler.onPlanningAttack += InitiateAttackTargetting;
    }

    private void CommitAttack(Unit targetUnit, int incomingDamage)
    {
        targetUnit.DamageTaken(incomingDamage);
        unitStateHandler.AttackFinished();
    }

    private void InitiateAttackTargetting()
    {
        GeneratePossibleMoves(unit.transform.position, unit.attackRange);
    }

    void RequestingAttackLogic(Vector3 startPos, Vector3 targetPos)
    {
        if (IsLegalMove(targetPos))
        {
            DisplayTargetting(targetPos);
            if (Input.GetMouseButtonDown(0))
            {
                InitiateAttack(targetPos);
            }
        }
    }

    private void InitiateAttack(Vector3 targetPos)
    {
        Node selectedNode = grid.NodeFromWorldPosition(targetPos);
        Unit target = UnitFromNode.SingleUnitFromNode(selectedNode);
        if (target != null)
        {
            unitStateHandler.SetState(Unit.UnitState.attacking);
            CommitAttack(target, unit.attackPower);
        }
        else
        {
            unitStateHandler.AttackFinished();
        }
    }

    private bool IsLegalMove(Vector3 targetPos)
    {
        return nodesWithinAttackRange.Contains(grid.NodeFromWorldPosition(targetPos)) &&
                grid.NodeFromWorldPosition(targetPos) != grid.NodeFromWorldPosition(unit.transform.position);
    }

    private List<Node> GeneratePossibleMoves(Vector3 startPos, int range)
    {
        nodesWithinAttackRange = new List<Node>();
        Node targetNode = grid.NodeFromWorldPosition(startPos);
        foreach (Node node in grid.GetAttackRange(targetNode, range))
        {
            if (aStar.PathFindingLogic(false, targetNode, node, range))
            {
                nodesWithinAttackRange.Add(node);
            }
        }
        onGenerateAttackRange(nodesWithinAttackRange);
        return nodesWithinAttackRange;
    }

    private void DisplayTargetting(Vector3 targetPos)
    {
        nodeAttackTargeting = new List<Node>();
        nodeAttackTargeting.Add(grid.NodeFromWorldPosition(targetPos));
    }
}
