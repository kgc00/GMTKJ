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
    private Coroutine currentTimer;
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
        UnitStateHandler.onUnitStunned += AddTimeToTimerStunned;
    }

    private void AddTimeToTimerStunned(Unit unitStunned, float timeStunned)
    {
        if (currentTimer == null)
        {
            StartTimer(unitStunned, timeStunned);
        }
        else
        {
        }
    }

    private void EndTimer(Unit _unit)
    {
        unitStateHandler.SetState(_unit, Unit.UnitState.idle);
        onTimerStopped(_unit, Unit.UnitState.idle);
        currentTimer = null;
        if (_unit == WorldManager.ReturnSelectedUnit())
        {
            FindObjectOfType<AbilityUI>().GetComponent<AbilityUI>().PopulateAbilityPanel(_unit);
        }
    }

    private void ReadyUnit(Unit _unit)
    {
        EndTimer(_unit);
    }

    public void AddTimeToTimerAbil(Unit _unit, float timeToAdd)
    {
        if (currentTimer == null)
        {
            float _timeToAddToTimer = timeToAdd;
            StartTimer(_unit, _timeToAddToTimer);
        }
        else
        {
            Debug.LogError("somehow we got double timers");
        }
    }

    private void StartTimer(Unit _unit, float _timeToAddToTimer)
    {
        TimerInfo info = new TimerInfo(_timeToAddToTimer, _unit);
        onTimerStarted(_unit);
        currentTimer = StartCoroutine("InitiateCooldown", info);
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
