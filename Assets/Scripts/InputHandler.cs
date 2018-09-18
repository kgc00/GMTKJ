using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Camera cam;
    public Transform target;
    public List<Node> nodesInRange;
    SceneManager sceneManager;
    GameGrid grid;
    Unit selectedUnit;
    UnitStateHandler unitStateHandler;
    private TargetingInformation targetInformation;
    public event Action<TargetingInformation> onAbilityCalled = delegate { };
    public event Action<Vector3, Vector3, Unit> onRequestingMovementLogic = delegate { };
    public event Action<Vector3, Vector3, Unit> onRequestingAttackLogic = delegate { };
    void Start()
    {
        sceneManager = SceneManager.instance;
        grid = GameGrid.instance;
        cam = Camera.main;
        unitStateHandler = FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>();
        if (target == null)
        {
            target = FindObjectOfType<TargetPosition>().transform;
        }
    }

    void Update()
    {
        if (WorldManager.instance.ReturnUnitSelected())
        {
            selectedUnit = WorldManager.ReturnSelectedUnit();
            if (ValidSelectedState(selectedUnit))
            {
                SelectedLogic(selectedUnit);
            }
            MovementLogic(selectedUnit);
            AttackLogic(selectedUnit);
        }
        else
        {
            SelectionLogic();
        }
    }

    private void AttackLogic(Unit _unit)
    {
        if (_unit.currentUnitState == Unit.UnitState.planningAttack)
        {
            onRequestingAttackLogic(_unit.transform.position, target.position, _unit);
        }
    }

    private void SelectedLogic(Unit _unit)
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            unitStateHandler.SetState(_unit, Unit.UnitState.planningMovement);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            unitStateHandler.SetState(_unit, Unit.UnitState.planningAttack);
            return;
        }

    }

    private bool ValidSelectedState(Unit _unit)
    {
        if (_unit.currentSelectionState == Unit.SelectionState.selected)
        {
            if (_unit.currentUnitState == Unit.UnitState.planningMovement ||
            _unit.currentUnitState == Unit.UnitState.planningAttack ||
            _unit.currentUnitState == Unit.UnitState.idle){
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    private void MovementLogic(Unit _unit)
    {
        if (_unit.currentUnitState == Unit.UnitState.planningMovement)
        {
            onRequestingMovementLogic(_unit.transform.position, target.position, _unit);
        }
    }

    private void SelectionLogic()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50, 1 << 11))
            {
                Node _selectedNode = grid.NodeFromWorldPosition(hit.transform.position);
                if (_selectedNode.occupiedByUnit == Node.OccupiedByUnit.ally)
                {
                    SelectUnit(hit);
                }
            }
        }
    }

    private static void SelectUnit(RaycastHit hit)
    {
        Unit _unitToSelect = hit.transform.parent.gameObject.GetComponent<Unit>();
        UnitSelectionHandler.SetSelection(_unitToSelect, Unit.SelectionState.selected);
        return;
    }
}
