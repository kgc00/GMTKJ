using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {
	List<Unit> ai_units = new List<Unit> ();
	private AI_InputHandler inputHandler;
	Command currentCommand = null;
	float timeBetweenCommands = 2f;
	WorldManager worldManager;
	AStar pathfinding;
	GameGrid grid;

	private void Start () {
		if (ai_units.Count > 0) {
			StartManagerCommands ();
		}
		grid = GameGrid.instance;
		worldManager = WorldManager.instance;
		pathfinding = FindObjectOfType<AStar> ().GetComponent<AStar> ();
		inputHandler = FindObjectOfType<AI_InputHandler> ().GetComponent<AI_InputHandler> ();
	}
	internal void Store_AI_Units (List<Unit> inc_ai_units) {
		ai_units = inc_ai_units;
	}
	internal void StartManagerCommands () {
		if (currentCommand == null) {
			Unit unitToControl = GetRandomUnit ();
			if (unitToControl != null) {
				Unit targetUnit = SelectClosestTarget (unitToControl);
				Ability abil = DetermineAbility (unitToControl, targetUnit);
				currentCommand = new AI_Ability_Command ();
				currentCommand.execute (abil, unitToControl, inputHandler);
				StartCoroutine ("WaitForNextCommand", timeBetweenCommands);
			} else {
				Debug.LogError ("Can't find a unit");
				StartCoroutine ("WaitForNextCommand", timeBetweenCommands);
			}
		}
	}

	IEnumerator WaitForNextCommand (float timeToWait) {
		yield return new WaitForSeconds (timeToWait);
		currentCommand = null;
		StartManagerCommands ();
		yield break;
	}

	private Ability DetermineAbility (Unit unitToControl, Unit targetUnit) {
		AbilityManager ability_manager = unitToControl.GetComponent<AbilityManager> ();
		List<Ability> abilityList = ability_manager.ReturnEquippedAbilities ();
		int distanceFromTarget = pathfinding.GetDistance (
			grid.NodeFromWorldPosition (unitToControl.transform.position),
			grid.NodeFromWorldPosition (targetUnit.transform.position));
		bool inAttackRange = false;
		Ability abilityToUse = null;
		foreach (Ability ability in abilityList) {
			if (ability.abilityInfo.attackRange <= distanceFromTarget) {
				inAttackRange = true;
				abilityToUse = ability;
				break;
			}
		}
		if (!inAttackRange) {
			return null; //movement ability
		} else {
			return abilityToUse;
		}
	}

	private Unit GetRandomUnit () {
		List<Unit> availableUnits = new List<Unit> ();
		foreach (Unit unit in ai_units) {
			if (inputHandler.ValidUnitState (unit)) {
				availableUnits.Add (unit);
			}
		}
		int unitIndex = UnityEngine.Random.Range (0, availableUnits.Count - 1);

		foreach (Unit unit in ai_units) {
			if (unit == availableUnits[unitIndex]) {
				return unit;
			}
		}
		return null;
	}

	public Unit SelectClosestTarget (Unit unit) {
		// List<Node> nodesWithinRangeAI = abilityTargeting.GenerateTargetingForAI (unit, unit.transform.position, ability);
		List<Unit> playerUnits = worldManager.GetAllPlayerUnits ();
		if (playerUnits.Count <= 0) {
			return null;
		}

		Unit closestUnit = FindClosestPlayerUnit (unit, playerUnits);

		if (closestUnit == null) {
			Debug.LogError ("Could not find any units");
			return null;
		}

		return closestUnit;
	}
	private Unit FindClosestPlayerUnit (Unit unit, List<Unit> playerUnits) {
		int shortestDistance = 99;
		Unit closestUnit = null;
		foreach (Unit playerUnit in playerUnits) {
			int distanceFromUnit = pathfinding.GetDistance (
				grid.NodeFromWorldPosition (unit.transform.position),
				grid.NodeFromWorldPosition (playerUnit.transform.position));

			if (distanceFromUnit < shortestDistance) {
				shortestDistance = distanceFromUnit;
				closestUnit = playerUnit;
			}
		}
		return closestUnit;
	}
}