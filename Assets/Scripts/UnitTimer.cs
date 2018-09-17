using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitTimer : MonoBehaviour
{

    private float cooldownTimeRemaining;
    private bool coolingDown = false;
    public static event Action<Unit> onTimerStarted = delegate { };
    public static event Action<Unit, Unit.UnitState> onTimerRemoved = delegate { };

    // Use this for initialization
    void Start()
    {
        // unitStateHandler.onMovementFinished += AddTimeToTimerMovement;
        // unitStateHandler.onAttackFinished += AddTimeToTimerAttack;
    }

    private void EndTimer()
    {
        coolingDown = false;
        // unitStateHandler.SetState(Unit.UnitState.unselected);
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
        // onTimerStarted();
    }
}
