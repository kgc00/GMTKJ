using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Ability_Command : AI_Command {
	public override void execute (Ability ability, Unit unit, Node targetNode, AI_InputHandler inputHandler, float delay) {
		inputHandler.SelectUnit (unit);
		inputHandler.PrepActionData (unit, ability, targetNode);
		inputHandler.PlanAction (unit, ability, targetNode);
		GameGrid.instance.ResetNodeCosts ();
		inputHandler.HandleExecution (unit, ability, delay);
	}
}