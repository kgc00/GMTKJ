using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Unit))]
public class UnitTimer : MonoBehaviour
{

    UnitStateHandler unitStateHandler;
    float cooldownTimeRemaining;
    bool coolingDown = false;
    public event Action onTimerStarted = delegate { };
    public event Action<float> onTimeChanged = delegate { };
    public event Action<Unit.UnitState> onTimerRemoved = delegate { };

    // Use this for initialization
    void Start()
    {
        unitStateHandler = GetComponent<UnitStateHandler>();
        unitStateHandler.onMovementFinished += AddTimeToTimerMovement;
        unitStateHandler.onAttackFinished += AddTimeToTimerAttack;
    }

    private void EndTimer()
    {
        coolingDown = false;
        onTimerRemoved(Unit.UnitState.unselected);
    }

    // Update is called once per frame
    void Update()
    {
        if (coolingDown)
        {
            ModifyTime();
            if (cooldownTimeRemaining <= 0)
            {
                ReadyUnit();
            }
        }
    }

    private void ModifyTime()
    {
        cooldownTimeRemaining -= Time.deltaTime;
        // onTimeChanged(cooldownTimeRemaining);
    }

    private void ReadyUnit()
    {
        EndTimer();
    }

    private void AddTimeToTimerMovement()
    {
        cooldownTimeRemaining += 2.5f;
        CheckStartTimer();
    }

    private void AddTimeToTimerAttack()
    {
        cooldownTimeRemaining += 3.5f;
        CheckStartTimer();
    }

    private void CheckStartTimer()
    {
        if (cooldownTimeRemaining > 0)
        {
            StartTimer();
        }
    }

    private void StartTimer()
    {
        coolingDown = true;
        onTimerStarted();
    }
}
