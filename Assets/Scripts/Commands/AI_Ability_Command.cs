using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Ability_Command : Command {
	public override void execute (Ability ability, Unit unit, Node targetNode, AI_InputHandler inputHandler) {
		inputHandler.SelectUnit (unit);
		inputHandler.PrepActionData (unit, ability, targetNode);
		inputHandler.PlanAction (unit, ability);
		GameGrid.instance.ResetNodeCosts ();
		inputHandler.InitiateAbility (unit, ability);
	}
}