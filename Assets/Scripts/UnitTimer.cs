using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTimer : MonoBehaviour {
    private UnitStateHandler unitStateHandler;
    private UnitSelectionHandler unitSelectionHandler;
    public static event Action<Unit> onTimerStarted = delegate { };
    public static event Action<Unit, Unit.UnitState> onTimerStopped = delegate { };
    private Dictionary<Unit, CoroutineInfo> currentCoroutines;
    private struct CoroutineInfo {
        public TimerInfo timerInfo;
        public Coroutine coroutine;
        public CoroutineInfo (TimerInfo _timerInfo, Coroutine _coroutine) {
            timerInfo = _timerInfo;
            coroutine = _coroutine;
        }
    }

    private struct TimerInfo {
        public float maxTime;
        public float timeLeft;
        public Unit unit;
        public TimerInfo (float _maxTime, Unit _unit, float _timeLeft) {
            maxTime = _maxTime;
            unit = _unit;
            timeLeft = _timeLeft;
        }
    }

    // need to refactor coroutine into a dictionary of coroutines which contain coroutine and unit
    // to allow for multiple timers at once

    // Use this for initialization
    void Start () {
        currentCoroutines = new Dictionary<Unit, CoroutineInfo> ();
        unitStateHandler = FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ();
        unitSelectionHandler = FindObjectOfType<UnitSelectionHandler> ().GetComponent<UnitSelectionHandler> ();
        UnitStateHandler.onUnitStunned += AddTimeToTimerStunned;
    }

    private void AddTimeToTimerStunned (Unit unitStunned, float timeStunned) {
        // currentCoroutines.ContainsKey (unitStunned);
        // if (currentCoroutines.ContainsKey (unitStunned)) {
        //     CoroutineInfo temp = currentCoroutines[unitStunned];
        //     if (temp.timerInfo.timeLeft > timeStunned) {
        //         Debug.Log ("yes");
        //         StopCoroutine (temp.coroutine);
        //         StartTimer (unitStunned, timeStunned);
        //     } else {
        //         Debug.Log ("no");
        //     }
        // } else {
        //     StartTimer (unitStunned, timeStunned);
        // }

        StartTimer (unitStunned, timeStunned);
    }

    private void EndTimer (Unit _unit) {
        unitStateHandler.SetState (_unit, Unit.UnitState.idle);
        onTimerStopped (_unit, Unit.UnitState.idle);
        if (_unit == WorldManager.ReturnSelectedUnit ()) {
            FindObjectOfType<AbilityUI> ().GetComponent<AbilityUI> ().PopulateAbilityPanel (_unit);
        }
    }

    private void ReadyUnit (Unit _unit) {
        EndTimer (_unit);
    }

    public void AddTimeToTimerAbil (Unit _unit, float timeToAdd) {
        float _maxTime = timeToAdd;
        StartTimer (_unit, _maxTime);
    }

    private void StartTimer (Unit _unit, float _maxTime) {
        TimerInfo info = new TimerInfo (_maxTime, _unit, _maxTime);
        Coroutine thisCoroutine = StartCoroutine ("InitiateCooldown", info);

        if (currentCoroutines.ContainsKey (_unit)) {
            CoroutineInfo temp = currentCoroutines[_unit];
            if (info.maxTime > temp.timerInfo.timeLeft) {
                StopCoroutine (temp.coroutine);
            }
        }

        CoroutineInfo coroutineInfo = new CoroutineInfo (info, thisCoroutine);
        currentCoroutines.Add (_unit, coroutineInfo);
        onTimerStarted (_unit);
    }

    private IEnumerator InitiateCooldown (TimerInfo info) {
        while (info.timeLeft > 0) {
            info.timeLeft -= Time.deltaTime;
            yield return null;

            // this will make less calls and optimize logic, but not sure how 
            // to make the timer accurate when using this method.

            // yield return new WaitForSeconds (.1f);
        }
        ReadyUnit (info.unit);
        currentCoroutines.Remove (info.unit);
    }
}