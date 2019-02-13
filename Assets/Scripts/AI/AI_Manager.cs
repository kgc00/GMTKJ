using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {
	List<Unit> ai_units = new List<Unit> ();
	private AI_InputHandler inputHandler;
	Command currentCommand = null;
	float timeBetweenCommands = 5f;
	WorldManager worldManager;
	AStar pathfinding;
	GameGrid grid;
	public MovementHandler movementHandler;
	PathRequestManager requestManager;

	private void Start () {
		grid = GameGrid.instance;
		worldManager = WorldManager.instance;
		pathfinding = FindObjectOfType<AStar> ().GetComponent<AStar> ();
		inputHandler = FindObjectOfType<AI_InputHandler> ().GetComponent<AI_InputHandler> ();
		movementHandler = FindObjectOfType<MovementHandler> ().GetComponent<MovementHandler> ();
		requestManager = FindObjectOfType<PathRequestManager> ().GetComponent<PathRequestManager> ();
		if (ai_units.Count > 0) {
			// StartManagerCommands ();
			StartCoroutine ("WaitForNextCommand", timeBetweenCommands);
		}
	}
	internal void Store_AI_Units (List<Unit> inc_ai_units) {
		ai_units = inc_ai_units;
	}
	internal void StartManagerCommands () {
		if (currentCommand == null) {
			Unit unitToControl = GetRandomUnit ();
			if (unitToControl != null) {
				Node targetNode = grid.NodeFromWorldPosition (SelectClosestTarget (unitToControl).transform.position);
				Ability abil = DetermineAbility (unitToControl, targetNode);
				unitToControl.SetCurrentAbility (abil);
				if (abil is MovementAbility) {
					QueryForNode (abil, unitToControl, targetNode, unitToControl);
					return;
				} else {
					currentCommand = new AI_Ability_Command ();
					currentCommand.execute (abil, unitToControl, targetNode, inputHandler);
					StartCoroutine ("WaitForNextCommand", timeBetweenCommands);
				}
			} else {
				Debug.Log ("No units availablse");
			}
		}
	}

	private void QueryForNode (Ability abil, Unit unitToControl, Node targetNode, Unit unit) {
		grid.ResetNodeCosts ();
		List<Node> possibleNodes = inputHandler.ReturnPossibleNodes (abil, unitToControl, targetNode);
		PathRequestManager.PathRequest _newRequest = new PathRequestManager.PathRequest (
			unitToControl.transform.position, targetNode.transform.position, SomeFakeCallback, SomeFakeMethod);
		requestManager.pathRequestQueue.Enqueue (_newRequest);
		if (requestManager.processing ()) {
			requestManager.NewPathLogic ();
		}
		StartCoroutine (
			movementHandler.GeneratePathForAI (unitToControl.transform.position,
				targetNode.transform.position, abil, unitToControl, targetNode,
				possibleNodes, PathRequestFinishedForAI)
		);
	}

	private void PathRequestFinishedForAI (
		Ability abil,
		Unit unitToControl,
		Node targetNode,
		List<Node> possibleNodes) {
		Node newTarget = SortNodes (possibleNodes, abil);
		FinishExecution (abil, unitToControl, targetNode, newTarget);
		grid.ResetNodeCosts ();
	}

	private void FinishExecution (Ability abil, Unit unitToControl, Node targetNode, Node newTarget) {
		if (newTarget != null) {
			// newTarget.GetComponent<SpriteRenderer> ().enabled = false;
			ExecuteMovementRequest (unitToControl, newTarget, abil);
		}
	}

	private void ExecuteMovementRequest (Unit unitToControl, Node targetNode, Ability abil) {
		currentCommand = new AI_Ability_Command ();
		currentCommand.execute (abil, unitToControl, targetNode, inputHandler);
		StartCoroutine ("WaitForNextCommand", timeBetweenCommands);
	}

	private Node SortNodes (List<Node> possibleNodes, Ability abil) {
		Node correctNode = null;
		int highestCost = -1;
		foreach (Node node in possibleNodes) {
			if (node.gCost > highestCost && node.gCost <= abil.abilityInfo.attackRange) {
				if (grid.UnitFromNode (node) != null) {
					// a unit will be on the node and it will not be a valid place to move
				} else {
					highestCost = node.gCost;
					correctNode = node;
				}
			}
		}
		return correctNode;
	}

	IEnumerator WaitForNextCommand (float timeToWait) {
		yield return new WaitForSeconds (timeToWait);
		currentCommand = null;
		StartManagerCommands ();
		yield break;
	}

	private Ability DetermineAbility (Unit unitToControl, Node targetNode) {
		AbilityManager ability_manager = unitToControl.GetComponent<AbilityManager> ();
		List<Ability> abilityList = ability_manager.ReturnEquippedAbilities ();
		int distanceFromTarget = pathfinding.GetDistance (
			grid.NodeFromWorldPosition (unitToControl.transform.position),
			targetNode);
		bool inAttackRange = false;
		Ability attackAbilityToUse = null;
		Ability movementAbilityToUse = null;
		foreach (Ability ability in abilityList) {
			if (ability is MovementAbility) {
				movementAbilityToUse = ability;
			}
			if (ability.abilityInfo.attackRange >= distanceFromTarget) {
				inAttackRange = true;
				attackAbilityToUse = ability;
			}
		}
		// Debug.Log (attackAbilityToUse + " " + movementAbilityToUse);
		if (!inAttackRange) {
			return movementAbilityToUse;
		} else {
			return attackAbilityToUse;
		}
	}

	private Unit GetRandomUnit () {
		List<Unit> availableUnits = new List<Unit> ();
		foreach (Unit unit in ai_units) {
			if (inputHandler.ValidUnitState (unit)) {
				availableUnits.Add (unit);
			}
		}

		if (availableUnits.Count <= 0) {
			return null;
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

	void SomeFakeCallback (Vector3[] path, bool success, Unit unit, Action<Unit> onDestReached) {

	}

	void SomeFakeMethod (Unit unit) {

	}
}