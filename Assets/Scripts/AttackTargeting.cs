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
    InputHandler inputHandler;
    public static event Action<Unit, List<Node>> onGenerateAttackRange = delegate { };

    void Start()
    {
        aStar = FindObjectOfType<AStar>().GetComponent<AStar>();
        grid = GameGrid.instance;
        inputHandler = FindObjectOfType<InputHandler>().GetComponent<InputHandler>();
        unitStateHandler = FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>();
        inputHandler.onRequestingAttackLogic += RequestingAttackLogic;
        UnitStateHandler.onUnitPlanningAttack += InitiateAttackTargeting;
    }

    // Need individual move logic here
    private void InitiateAttackTargeting(Unit _unit, Ability.AbilityInfo _abilityInfo)
    {
        DetermineAttackType(_unit);
        // pass output to generate possible moves
        GeneratePossibleMoves(_unit, _unit.transform.position, _abilityInfo);
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
            inputHandler.AttackInput(startPos, targetPos, slot);
        }
    }

    private void CommitAttack(Unit _targetUnit, Unit _attackingUnit)
    {
        int _incomingDamage = _attackingUnit.attackPower;
        _targetUnit.TakeDamage(_incomingDamage);
        unitStateHandler.AttackFinished(_attackingUnit);
    }

    public void InitiateAttack(Vector3 _startPos, Vector3 _targetPos, int slot)
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

    private List<Node> GeneratePossibleMoves(Unit _unit, Vector3 startPos, Ability.AbilityInfo info)
    {
        nodesWithinAttackRange = new List<Node>();
        Node targetNode = grid.NodeFromWorldPosition(startPos);
        foreach (Node node in grid.GetAttackRange(targetNode, info))
        {
            if (aStar.PathFindingLogic(false, targetNode, node, info.attackRange))
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
