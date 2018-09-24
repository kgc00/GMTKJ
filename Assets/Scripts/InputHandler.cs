using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Camera cam;
    public Transform target;
    SceneManager sceneManager;
    GameGrid grid;
    Unit selectedUnit;
    UnitStateHandler unitStateHandler;
    public event Action<TargetingInformation> onAbilityCalled = delegate { };
    public event Action<Vector3, Vector3, Unit> onRequestingMovementLogic = delegate { };
    public event Action<Vector3, Vector3, Unit, int> onRequestingAttackLogic = delegate { };
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
            if (WorldManager.ReturnSelectedUnit() != selectedUnit)
            {
                selectedUnit = WorldManager.ReturnSelectedUnit();
            }

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
            onRequestingAttackLogic(_unit.transform.position, target.position,
            _unit, _unit.GetComponent<AbilityManager>().ReturnCurrentAttack());
        }
    }

    private void SelectedLogic(Unit _unit)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_unit.GetComponent<AbilityManager>().AnimateAbilitySelection(0))
            {
                unitStateHandler.SetState(_unit, Unit.UnitState.planningMovement);
            }
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_unit.GetComponent<AbilityManager>().AnimateAbilitySelection(1))
            {
                unitStateHandler.SetState(_unit, Unit.UnitState.planningAttack);
            }
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_unit.GetComponent<AbilityManager>().AnimateAbilitySelection(2))
            {
                unitStateHandler.SetState(_unit, Unit.UnitState.planningAttack);
            }
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (_unit.GetComponent<AbilityManager>().AnimateAbilitySelection(3))
            {
                unitStateHandler.SetState(_unit, Unit.UnitState.planningAttack);
            }
            return;
        }
    }

    private bool ValidSelectedState(Unit _unit)
    {
        if (_unit.currentSelectionState == Unit.SelectionState.selected)
        {
            if (_unit.currentUnitState == Unit.UnitState.planningMovement ||
            _unit.currentUnitState == Unit.UnitState.planningAttack ||
            _unit.currentUnitState == Unit.UnitState.idle)
            {
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
                Unit _selectedUnit = hit.transform.parent.gameObject.GetComponent<Unit>();
                if (_selectedNode.occupiedByUnit == Node.OccupiedByUnit.ally &&
                _selectedUnit.currentUnitState == Unit.UnitState.idle)
                {
                    SelectUnit(_selectedUnit);
                }
            }
        }
    }

    private static void SelectUnit(Unit _selectedUnit)
    {
        UnitSelectionHandler.SetSelection(_selectedUnit, Unit.SelectionState.selected);
        return;
    }
}
