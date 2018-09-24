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
        unitStateHandler = FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>();
        FindObjectOfType<InputHandler>().GetComponent<InputHandler>().onRequestingAttackLogic += RequestingAttackLogic;
        UnitStateHandler.onUnitPlanningAttack += InitiateAttackTargeting;
    }

    // Need individual move logic here
    private void InitiateAttackTargeting(Unit _unit)
    {
        DetermineAttackType(_unit);
        // pass output to generate possible moves
        GeneratePossibleMovesSquare(_unit, _unit.transform.position, _unit.attackRange);
    }

    private void DetermineAttackType(Unit _unit)
    {
        Ability.AbilityInfo info = _unit.GetComponent<AbilityManager>().ReturnAbilityInfo();
    }

    void RequestingAttackLogic(Vector3 startPos, Vector3 targetPos, Unit unit, int slot)
    {
        if (IsLegalMove(startPos, targetPos, unit))
        {
            DisplayTargeting(targetPos);
            if (Input.GetMouseButtonDown(0))
            {
                InitiateAttack(startPos, targetPos, slot);
            }
        }
    }

    private void CommitAttack(Unit _targetUnit, Unit _attackingUnit)
    {
        int _incomingDamage = _attackingUnit.attackPower;
        _targetUnit.TakeDamage(_incomingDamage);
        unitStateHandler.AttackFinished(_attackingUnit);
    }

    private void InitiateAttack(Vector3 _startPos, Vector3 _targetPos, int slot)
    {
        Node _selectedNode = grid.NodeFromWorldPosition(_targetPos);
        Unit _target = UnitFromNode.SingleUnitFromNode(_selectedNode);
        Unit _attackingUnit = UnitFromNode.SingleUnitFromNode(grid.NodeFromWorldPosition(_startPos));
        _attackingUnit.GetComponent<AbilityManager>().AnimateAbilityUse(slot);
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

    private List<Node> GeneratePossibleMovesSquare(Unit _unit, Vector3 startPos, int range)
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

    private void DisplayTargeting(Vector3 targetPos)
    {
        nodeAttackTargeting = new List<Node>();
        nodeAttackTargeting.Add(grid.NodeFromWorldPosition(targetPos));
    }
}
