using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Unit))]
public class InputHandler : MonoBehaviour
{

    InputHandler inputHandler;
    Unit unit;
    public Transform target;
    public List<Node> nodesInRange;
    SceneManager sceneManager;
    GameGrid grid;
    DebugGizmo gizmothing;
    AStar aStar;
    Node selectedNode;
    UnitStateHandler unitStateHandler;
    private TargetingInformation targetInformation;
    public event Action<TargetingInformation> onAbilityCalled = delegate { };
    void Start()
    {
        sceneManager = SceneManager.instance;
        grid = GameGrid.instance;
        gizmothing = DebugGizmo.instance;
        unitStateHandler = GetComponent<UnitStateHandler>();
        Debug.Assert(aStar = GetComponent<AStar>());
        Debug.Assert(unit = GetComponent<Unit>());
        Debug.Assert(inputHandler = GetComponent<InputHandler>());
        unitStateHandler.onUnitPlanningMovement += DisplayMoves;
        unitStateHandler.onMovementFinished += ResetNodesInRange;
    }

    void Update()
    {
        SelectionLogic();
        SelectedLogic();
        MovementLogic();
        AttackLogic();
    }

    private void AttackLogic()
    {
        if (unit.currentUnitState == Unit.UnitState.planningAttack){
            print("need to implement attack logic");
        }
    }

    private void SelectedLogic()
    {
        if (unit.currentUnitState == Unit.UnitState.selected)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                unitStateHandler.SetState(Unit.UnitState.planningMovement);
                return;
            } else if (Input.GetKeyDown(KeyCode.Alpha2)){
                unitStateHandler.SetState(Unit.UnitState.planningAttack);
                return;
            }
        }
    }

    private void MovementLogic()
    {
        if (unit.currentUnitState == Unit.UnitState.planningMovement)
        {
            if (IsLegalMove())
            {
                DisplayPath(unit.transform.position, target.position);
            }
            if (IsLegalMove() && Input.GetMouseButtonDown(0) && unit.currentMovementPoints > 0)
            {
                StoreTargetInfo(unit.transform.position, target.position);
                unitStateHandler.SetState(Unit.UnitState.moving);
            }
        }
    }

    private bool IsLegalMove()
    {
        return nodesInRange.Contains(grid.NodeFromWorldPosition(target.position)) &&
                grid.NodeFromWorldPosition(target.position) != grid.NodeFromWorldPosition(unit.transform.position) &&
                !grid.nodesContainingUnits.Contains(grid.NodeFromWorldPosition(target.position));
    }

    private void AbilityOne()
    {
        StoreTargetInfo(unit.transform.position, target.position);
        onAbilityCalled(PassTargetInfo());
    }

    private void SelectionLogic()
    {
        if (unit.currentUnitState == Unit.UnitState.unselected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedNode = grid.NodeFromWorldPosition(target.position);
                if (selectedNode.occupiedByUnit == Node.OccupiedByUnit.ally &&
                UnitFromNode(selectedNode) == unit)
                {
                    unitStateHandler.SetState(Unit.UnitState.selected);
                    return;
                }
            }
        }
    }

    private Unit UnitFromNode(Node _selectedNode)
    {
        Unit affectedUnit = null;
        Collider[] hitColliders = Physics.OverlapSphere(_selectedNode.worldPosition, grid.nodeRadius, grid.allyMask);
        foreach (Collider collider in hitColliders)
        {
            affectedUnit = collider.gameObject.GetComponentInParent<Unit>();
            if (affectedUnit != null)
            {
                return affectedUnit;
            }
        }
        return affectedUnit;
    }

    private void StoreTargetInfo(Vector3 startingPos, Vector3 targetPos)
    {
        targetInformation = new TargetingInformation(startingPos, targetPos);
    }

    public TargetingInformation PassTargetInfo()
    {
        return targetInformation;
    }

    public void DisplayMoves(Unit _unit)
    {
        if (unit.currentMovementPoints > 0)
        {
            GeneratePossibleMoves(unit.transform.position, unit.currentMovementPoints);
            gizmothing.playerRequestingPath = true;
        }
    }

    private List<Node> GeneratePossibleMoves(Vector3 startPos, int range)
    {
        if (nodesInRange == null)
        {
            nodesInRange = new List<Node>();
        }
        Node targetNode = grid.NodeFromWorldPosition(startPos);
        foreach (Node node in grid.GetRange(targetNode, range))
        {
            if (aStar.PathFindingLogic(false, targetNode, node, range))
            {
                nodesInRange.Add(node);
            }
        }
        gizmothing._nodesWithinRange = nodesInRange;
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
        path.Reverse();
        gizmothing.path = path;
    }

    public void ResetNodesInRange()
    {
        nodesInRange = new List<Node>();
    }
}
