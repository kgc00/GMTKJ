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
	public Action UnitReady = delegate { };

    // Use this for initialization
    void Start()
    {
        unitStateHandler = GetComponent<UnitStateHandler>();
        unitStateHandler.onMovementFinished += AddTimeToTimer;
    }

    // Update is called once per frame
    void Update()
    {
		if (coolingDown){
			cooldownTimeRemaining -= Time.deltaTime;
			if (cooldownTimeRemaining <= 0)
            {
                ReadyUnit();
            }
        }
    }

    private void ReadyUnit()
    {
        coolingDown = false;
		UnitReady();
    }

    private void AddTimeToTimer()
    {
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
    }
}
