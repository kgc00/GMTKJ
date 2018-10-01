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
    public static event Action<Node, Unit, Ability, Unit> onCommitToAttack = delegate { };

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
    private void InitiateAttackTargeting(Unit _unit, Ability.AbilityInfo _abilityInfo, Ability abil)
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

    private void CommitToAttack(Node _targetNode, Unit _attackingUnit, Ability _ability, Unit _target)
    {
        onCommitToAttack(_targetNode, _attackingUnit, _ability, _target);
    }

    public void InitiateAttack(Vector3 _startPos, Vector3 _targetPos, int slot)
    {
        Node _selectedNode = grid.NodeFromWorldPosition(_targetPos);
        Unit _target = UnitFromNode.SingleUnitFromNode(_selectedNode);
        Unit _attackingUnit = UnitFromNode.SingleUnitFromNode(grid.NodeFromWorldPosition(_startPos));
        _attackingUnit.GetComponent<AbilityManager>().AnimateAbilityUse(slot);
        Ability ability = _attackingUnit.GetComponent<AbilityManager>().ReturnAbility();
        unitStateHandler.SetState(_attackingUnit, Unit.UnitState.attacking);
        CommitToAttack(_selectedNode, _attackingUnit, ability, _target);
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
