using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour {
    private Dictionary<Unit, Image> timerImages;
    void Awake () {
        timerImages = new Dictionary<Unit, Image> ();
        UnitTimer.onTimerStarted += StartTimer;
        UnitTimer.onTimerStopped += StopTimer;
        UnitTimer.onFillChange += SetFill;
    }

    private void OnDestroy () {
        UnitTimer.onTimerStarted -= StartTimer;
        UnitTimer.onTimerStopped -= StopTimer;
        UnitTimer.onFillChange -= SetFill;
    }

    void StartTimer (Unit _unit) {
        if (!timerImages.ContainsKey (_unit)) {
            timerImages.Add (_unit, _unit.transform.Find ("Cooldown Canvas/Cooldown Image").GetComponent<Image> ());
        }
        timerImages[_unit].enabled = true;
    }

    void SetFill (Unit _unit, float fillAmount) {
        if (!timerImages.ContainsKey (_unit)) {
            timerImages.Add (_unit, _unit.transform.Find ("Cooldown Canvas/Cooldown Image").GetComponent<Image> ());
        }
        timerImages[_unit].fillAmount = fillAmount;
    }

    void StopTimer (Unit _unit, Unit.UnitState _state) {
        // check for instances where the unit dies during the timer duration
        if (_unit) {
            if (!timerImages.ContainsKey (_unit)) {
                timerImages.Add (_unit, _unit.transform.Find ("Cooldown Canvas/Cooldown Image").GetComponent<Image> ());
            }
            timerImages[_unit].enabled = false;
        }
    }
}