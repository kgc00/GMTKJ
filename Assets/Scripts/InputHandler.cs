using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public event Action<Vector3, Vector3> onRequestingMovementLogic = delegate { };
    public event Action<Vector3, Vector3> onRequestingAttackLogic = delegate { };
    void Start()
    {
        sceneManager = SceneManager.instance;
        grid = GameGrid.instance;
        gizmothing = DebugGizmo.instance;
        unitStateHandler = GetComponent<UnitStateHandler>();
        Debug.Assert(aStar = GetComponent<AStar>());
        Debug.Assert(unit = GetComponent<Unit>());
        Debug.Assert(inputHandler = GetComponent<InputHandler>());
        target = FindObjectOfType<TargetPosition>().transform;
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
        if (unit.currentUnitState == Unit.UnitState.planningAttack)
        {
            onRequestingAttackLogic(unit.transform.position, target.position);
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
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                unitStateHandler.SetState(Unit.UnitState.planningAttack);
                return;
            }
        }
    }

    private void MovementLogic()
    {
        if (unit.currentUnitState == Unit.UnitState.planningMovement)
        {
            onRequestingMovementLogic(unit.transform.position, target.position);
        }
    }

    private void SelectionLogic()
    {
        if (!WorldManager.instance.AnyUnitSelected())
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
}
