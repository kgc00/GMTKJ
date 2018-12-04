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
    AbilityTargeting abilityTargeting;
    public event Action<TargetingInformation> onAbilityCalled = delegate { };
    public event Action<Vector3, Vector3, Unit> onRequestingMovementLogic = delegate { };
    public event Action<Vector3, Vector3, Unit, int> onRequestingAttackLogic = delegate { };
    void Start()
    {
        sceneManager = SceneManager.instance;
        grid = GameGrid.instance;
        cam = Camera.main;
        abilityTargeting = FindObjectOfType<AbilityTargeting>().GetComponentInChildren<AbilityTargeting>();
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
            if (selectedUnit.currentUnitState == Unit.UnitState.planningAction)
            {
                abilityTargeting.HandleAbilityInput();
                AbilityInput(selectedUnit: selectedUnit, startPos: selectedUnit.transform.position,
                targetPos: target.position, slot: unitStateHandler.curAbilSlot);
                return;
            }

            if (ValidSelectedState(selectedUnit))
            {
                SelectedLogic(selectedUnit);
            }
            MovementLogic(selectedUnit);
        }
        else
        {
            SelectionLogic();
        }
    }

    private void SelectedLogic(Unit _unit)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PrepActionData(_unit, 0);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PrepActionData(_unit, 1);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PrepActionData(_unit, 2);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PrepActionData(_unit, 3);
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

    private void PrepActionData(Unit _unit, int _abilitySlot)
    {
        CallForAnimation(_unit, _abilitySlot);
        unitStateHandler.SetAttackData(_unit.GetComponent<AbilityManager>().ReturnAbilityInfo());
        unitStateHandler.SetAbil(_unit.GetComponent<AbilityManager>().ReturnAbility());
        unitStateHandler.SetState(_unit, Unit.UnitState.planningAction);
        unitStateHandler.SetAbilSlot(_unit, _abilitySlot);
    }

    private bool ValidSelectedState(Unit _unit)
    {
        if (_unit.currentSelectionState == Unit.SelectionState.selected)
        {
            if (_unit.currentUnitState == Unit.UnitState.planningMovement ||
            _unit.currentUnitState == Unit.UnitState.planningAttack ||
            _unit.currentUnitState == Unit.UnitState.planningAction ||
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

    public void AbilityInput(Unit selectedUnit, Vector3 startPos, Vector3 targetPos, int slot)
    {
        if (Input.GetMouseButtonDown(0))
        {
            unitStateHandler.curAbil.abilityInfo.infoTheSecond = abilityTargeting.CacheRelevantInfo(startPos, targetPos, slot);
            unitStateHandler.curAbil.OnCommited(selectedUnit);
        }
    }

    private static void SelectUnit(Unit _selectedUnit)
    {
        UnitSelectionHandler.SetSelection(_selectedUnit, Unit.SelectionState.selected);
        return;
    }
}
