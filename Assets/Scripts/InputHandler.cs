﻿using System;
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
    AttackTargeting attackTargeting;
    public event Action<TargetingInformation> onAbilityCalled = delegate { };
    public event Action<Vector3, Vector3, Unit> onRequestingMovementLogic = delegate { };
    public event Action<Vector3, Vector3, Unit, int> onRequestingAttackLogic = delegate { };
    void Start()
    {
        sceneManager = SceneManager.instance;
        grid = GameGrid.instance;
        cam = Camera.main;
        attackTargeting = FindObjectOfType<AttackTargeting>().GetComponentInChildren<AttackTargeting>();
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
            unitStateHandler.GetAttackData(_unit.GetComponent<AbilityManager>().ReturnAbilityInfo());
            unitStateHandler.GetAbil(_unit.GetComponent<AbilityManager>().ReturnAbility());
            unitStateHandler.SetState(_unit, Unit.UnitState.planningAction);
            CallForAnimation(_unit, 0);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            unitStateHandler.GetAttackData(_unit.GetComponent<AbilityManager>().ReturnAbilityInfo());
            unitStateHandler.GetAbil(_unit.GetComponent<AbilityManager>().ReturnAbility());
            unitStateHandler.SetState(_unit, Unit.UnitState.planningAction);
            CallForAnimation(_unit, 1);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            unitStateHandler.GetAttackData(_unit.GetComponent<AbilityManager>().ReturnAbilityInfo());
            unitStateHandler.GetAbil(_unit.GetComponent<AbilityManager>().ReturnAbility());
            unitStateHandler.SetState(_unit, Unit.UnitState.planningAction);
            CallForAnimation(_unit, 2);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            unitStateHandler.GetAttackData(_unit.GetComponent<AbilityManager>().ReturnAbilityInfo());
            unitStateHandler.GetAbil(_unit.GetComponent<AbilityManager>().ReturnAbility());
            unitStateHandler.SetState(_unit, Unit.UnitState.planningAction);
            CallForAnimation(_unit, 3);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // unitStateHandler.SetState(_unit, Unit.UnitState.idle);
            // UnitSelectionHandler.SetSelection(_unit, Unit.SelectionState.notSelected);
            Debug.LogError("scripts are not set up to handle this request");
            return;
        }
    }

    private bool CallForAnimation(Unit _unit, int v)
    {
        return _unit.GetComponent<AbilityManager>().AnimateAbilitySelection(v);
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

    public void AttackInput(Vector3 startPos, Vector3 targetPos, int slot)
    {
        if (Input.GetMouseButtonDown(0))
        {
            attackTargeting.InitiateAttack(startPos, targetPos, slot);
        }
    }

    private static void SelectUnit(Unit _selectedUnit)
    {
        UnitSelectionHandler.SetSelection(_selectedUnit, Unit.SelectionState.selected);
        return;
    }
}
