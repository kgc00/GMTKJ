using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Image hourGlass;
    UnitTimer timer;

    void Awake()
    {
		hourGlass = transform.Find("Cooldown Image").GetComponent<Image>();
        timer = GetComponentInParent<UnitTimer>();
        timer.onTimerStarted += StartTimer;
        timer.onTimerRemoved += StopTimer;
        StopTimer(Unit.UnitState.unselected);
    }
    void StartTimer()
    {
        hourGlass.enabled = true;
    }

    void StopTimer(Unit.UnitState state)
    {
        hourGlass.enabled = false;
    }
}
