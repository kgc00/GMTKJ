using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitTimer : MonoBehaviour
{
    private UnitStateHandler unitStateHandler;
    private UnitSelectionHandler unitSelectionHandler;
    public static event Action<Unit> onTimerStarted = delegate { };
    public static event Action<Unit, Unit.UnitState> onTimerStopped = delegate { };
    private struct TimerInfo
    {
        public float timeToAddToTimer;
        public Unit unit;
        public TimerInfo(float _timeToAddToTimer, Unit _unit)
        {
            timeToAddToTimer = _timeToAddToTimer;
            unit = _unit;
        }
    }

    // Use this for initialization
    void Start()
    {
        unitStateHandler = FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>();
        unitSelectionHandler = FindObjectOfType<UnitSelectionHandler>().GetComponent<UnitSelectionHandler>();
        UnitStateHandler.onMovementFinished += AddTimeToTimerMovement;
        UnitStateHandler.onAttackFinished += AddTimeToTimerAttack;
    }

    private void EndTimer(Unit _unit)
    {
        unitStateHandler.SetState(_unit, Unit.UnitState.idle);
        onTimerStopped(_unit, Unit.UnitState.idle);
        if (_unit == WorldManager.ReturnSelectedUnit())
        {
            FindObjectOfType<AbilityUI>().GetComponent<AbilityUI>().PopulateAbilityPanel(_unit);
        }
    }

    private void ReadyUnit(Unit _unit)
    {
        EndTimer(_unit);
    }

    private void AddTimeToTimerMovement(Unit _unit)
    {
        float _timeToAddToTimer = 2.5f;
        StartTimer(_unit, _timeToAddToTimer);
    }

    private void AddTimeToTimerAttack(Unit _unit)
    {
        float _timeToAddToTimer = 3.5f;
        StartTimer(_unit, _timeToAddToTimer);
    }

    private void StartTimer(Unit _unit, float _timeToAddToTimer)
    {
        TimerInfo info = new TimerInfo(_timeToAddToTimer, _unit);
        onTimerStarted(_unit);
        StartCoroutine("InitiateCooldown", info);
    }

    private IEnumerator InitiateCooldown(TimerInfo info)
    {
        float timeToWait = info.timeToAddToTimer + Time.time;
        while (Time.time < timeToWait)
        {
            yield return new WaitForSeconds(.1f);
        }
        ReadyUnit(info.unit);
    }
}
