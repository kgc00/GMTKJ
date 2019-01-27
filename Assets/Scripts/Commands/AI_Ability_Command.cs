using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Ability_Command : Command {
	public override void execute (Ability ability, Unit unit, AI_InputHandler inputHandler) {
		inputHandler.SelectUnit (unit);
		inputHandler.PrepActionData (unit, ability);
		inputHandler.InitiateAbility (unit, ability);
	}
}