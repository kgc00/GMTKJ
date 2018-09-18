using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    void Awake()
    {
        transform.Find("Cooldown Image").GetComponent<Image>().enabled = false;
        UnitTimer.onTimerStarted += StartTimer;
        UnitTimer.onTimerStopped += StopTimer;
    }
    void StartTimer(Unit _unit)
    {
        _unit.transform.Find("Cooldown Canvas/Cooldown Image").GetComponent<Image>().enabled = true;
    }

    void StopTimer(Unit _unit, Unit.UnitState state)
    {
        _unit.transform.Find("Cooldown Canvas/Cooldown Image").GetComponent<Image>().enabled = false;
    }
}
