using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Image hourGlass;

    void Awake()
    {
		hourGlass = transform.Find("Cooldown Image").GetComponent<Image>();
        UnitTimer.onTimerStarted += StartTimer;
        UnitTimer.onTimerRemoved += StopTimer;
        // StopTimer(Unit.UnitState.unselected);
    }
    void StartTimer(Unit _unit)
    {
        hourGlass.enabled = true;
    }

    void StopTimer(Unit _unit, Unit.UnitState state)
    {
        hourGlass.enabled = false;
    }
}
