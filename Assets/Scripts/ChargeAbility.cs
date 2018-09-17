using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityManager))]
public class ChargeAbility : MonoBehaviour {

	InputHandler inputHandler;

	void Awake(){
		inputHandler = GetComponent<InputHandler>();
		inputHandler.onAbilityCalled += CheckAbility;
	}

	private void CheckAbility(TargetingInformation targetInfo){
	}
}
