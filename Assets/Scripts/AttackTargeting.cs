using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargeting : MonoBehaviour
{

    List<Node> nodesWithinAttackRange = new List<Node>();
    List<Node> nodeAttackTargeting = new List<Node>();
    GameGrid grid;
    AStar aStar;
    UnitStateHandler unitStateHandler;
    public static event Action<Unit, List<Node>> onGenerateAttackRange = delegate { };

    void Start()
    {
        aStar = FindObjectOfType<AStar>().GetComponent<AStar>();
        grid = GameGrid.instance;
        FindObjectOfType<InputHandler>().GetComponent<InputHandler>().onRequestingAttackLogic += RequestingAttackLogic;
        UnitStateHandler.onUnitPlanningAttack += InitiateAttackTargetting;
    }

    private void CommitAttack(Unit _targetUnit, Unit _attackingUnit)
    {
        int _incomingDamage = _attackingUnit.attackPower;
        _targetUnit.TakeDamage(_incomingDamage);
        unitStateHandler.AttackFinished(_attackingUnit);
    }

    private void InitiateAttackTargetting(Unit _unit)
    {
        GeneratePossibleMoves(_unit, _unit.transform.position, _unit.attackRange);
    }

    void RequestingAttackLogic(Vector3 startPos, Vector3 targetPos, Unit unit)
    {
        if (IsLegalMove(startPos, targetPos, unit))
        {
            DisplayTargetting(targetPos);
            if (Input.GetMouseButtonDown(0))
            {
                InitiateAttack(startPos, targetPos);
            }
        }
    }

    private void InitiateAttack(Vector3 _startPos, Vector3 _targetPos)
    {
        Node _selectedNode = grid.NodeFromWorldPosition(_targetPos);
        Unit _target = UnitFromNode.SingleUnitFromNode(_selectedNode);
        Unit _attackingUnit = UnitFromNode.SingleUnitFromNode(grid.NodeFromWorldPosition(_startPos));
        if (_target != null)
        {
            unitStateHandler.SetState(_attackingUnit, Unit.UnitState.attacking);
            CommitAttack(_target, _attackingUnit);
        }
        else
        {
            unitStateHandler.SetState(_attackingUnit, Unit.UnitState.attacking);
            unitStateHandler.AttackFinished(_attackingUnit);
        }
    }

    private bool IsLegalMove(Vector3 startPos, Vector3 targetPos, Unit unit)
    {
        return nodesWithinAttackRange.Contains(grid.NodeFromWorldPosition(targetPos)) &&
                grid.NodeFromWorldPosition(targetPos) != grid.NodeFromWorldPosition(unit.transform.position);
    }

    private List<Node> GeneratePossibleMoves(Unit _unit, Vector3 startPos, int range)
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
        onGenerateAttackRange(_unit, nodesWithinAttackRange);
        return nodesWithinAttackRange;
    }

    private void DisplayTargetting(Vector3 targetPos)
    {
        nodeAttackTargeting = new List<Node>();
        nodeAttackTargeting.Add(grid.NodeFromWorldPosition(targetPos));
    }
}
