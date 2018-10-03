using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    void Awake()
    {
        // pass in an array of all units.... or call a function on unit component enabled
        UnitTimer.onTimerStarted += StartTimer;
        UnitTimer.onTimerStopped += StopTimer;
    }
    void StartTimer(Unit _unit)
    {
        _unit.transform.Find("Cooldown Canvas/Cooldown Image").GetComponent<Image>().enabled = true;
    }

    void StopTimer(Unit _unit, Unit.UnitState _state)
    {
        // if a unit is killed while timer is counting do it will cause an error 
        // on recieving the action call, hopefully this fixes it
        if (_unit)
        {
            _unit.transform.Find("Cooldown Canvas/Cooldown Image").GetComponent<Image>().enabled = false;
        }
    }
}
