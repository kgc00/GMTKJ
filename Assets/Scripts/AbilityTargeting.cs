using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTargeting : MonoBehaviour {
    [SerializeField]
    List<Node> nodesWithinRange = new List<Node> ();
    GameGrid grid;
    GridEffects gridfx;
    AStar aStar;
    UnitStateHandler unitStateHandler;
    InputHandler inputHandler;
    public static event Action<Unit, List<Node>> onGenerateAbilityRange = delegate { };
    public static event Action<Node, Unit, Ability, Unit> onCommitToMeleeAttack = delegate { };
    public static event Action<List<Unit>, Unit, AttackAbility, Action<Unit>> onCommitToMeleeAOEAttack = delegate { };
    [SerializeField]
    private Transform target;
    List<Node> targetNode = new List<Node> (1);
    private AbilityTargetingData relevantAbilityInfo = new AbilityTargetingData (
        new Vector3 (0, 0, 0), new Vector3 (0, 0, 0), 0);

    void Start () {
        aStar = FindObjectOfType<AStar> ().GetComponent<AStar> ();
        grid = GameGrid.instance;
        gridfx = FindObjectOfType<GridEffects> ().GetComponent<GridEffects> ();
        target = FindObjectOfType<TargetPosition> ().transform;
        inputHandler = FindObjectOfType<InputHandler> ().GetComponent<InputHandler> ();
        unitStateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
    }

    public List<Node> InitiateAbilityTargeting (Unit _unit, Ability abil) {
        return GenerateTargeting (_unit, _unit.transform.position, abil);
    }

    private List<Node> GenerateTargeting (Unit _unit, Vector3 startPos, Ability abil) {
        nodesWithinRange = new List<Node> ();
        Ability.AbilityInfo info = abil.abilityInfo;
        Node targetNode = grid.NodeFromWorldPosition (startPos);
        foreach (Node node in grid.GetAttackRange (targetNode, info)) {
            if (aStar.PathFindingLogic (false, targetNode, node, info.attackRange)) {
                nodesWithinRange.Add (node);
            }
        }
        onGenerateAbilityRange (_unit, nodesWithinRange);
        return nodesWithinRange;
    }

    public void HandleAbilityInput () {
        Vector3 targetPos = target.position;
        if (IsLegalMove (targetPos)) {
            targetNode = CacheSelectedTile (targetPos);
            gridfx.RenderSelectorHighlights (targetNode);
        }
    }

    public void CommitToAttack (Vector3 _startPos, Vector3 _targetPos, int slot) {
        Node _selectedNode = grid.NodeFromWorldPosition (_targetPos);
        Unit _target = UnitFromNode.SingleUnitFromNode (_selectedNode);
        Unit _attackingUnit = UnitFromNode.SingleUnitFromNode (grid.NodeFromWorldPosition (_startPos));
        Ability _ability = _attackingUnit.GetComponent<AbilityManager> ().ReturnAbility ();
        onCommitToMeleeAttack (_selectedNode, _attackingUnit, _ability, _target);
    }

    internal void CommitToAoEAttack (List<Node> nodesInAbilityRange, Unit attackingUnit, int slot, Action<Unit> callback = null) {
        List<Unit> unitsAffected = new List<Unit> ();
        foreach (Node node in nodesInAbilityRange) {
            if (UnitFromNode.SingleUnitFromNode (node)) {
                unitsAffected.Add (UnitFromNode.SingleUnitFromNode (node));
            }
        }
        Ability ability = attackingUnit.GetComponent<AbilityManager> ().ReturnAbility ();
        AttackAbility attackAbility = null;
        if (ability is AttackAbility) {
            attackAbility = (AttackAbility) ability;
            ability = attackAbility;
            onCommitToMeleeAOEAttack (unitsAffected, attackingUnit, attackAbility, callback);
        } else {
            Debug.LogError ("No attack ability found");
        }
    }
    internal void CommitToRangedAttack (Vector3 startPos, Vector3 targetPos, int slot, Action<Unit> callback = null) {

    }

    private bool IsLegalMove (Vector3 targetPos) {
        return nodesWithinRange.Contains (grid.NodeFromWorldPosition (targetPos));
    }

    private List<Node> CacheSelectedTile (Vector3 targetPos) {
        if (!targetNode.Contains (grid.NodeFromWorldPosition (targetPos))) {
            targetNode.Clear ();
            targetNode.Add (grid.NodeFromWorldPosition (targetPos));
        }
        return targetNode;
    }

    public AbilityTargetingData CacheRelevantInfo (Vector3 startPos, Vector3 targetPos, int slot) {
        return new AbilityTargetingData (startPos, targetPos, slot);
    }
}