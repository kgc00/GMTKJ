using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {
	List<Unit> ai_units = new List<Unit> ();
	private AI_InputHandler inputHandler;
	AI_Command currentCommand = null;
	float timeBetweenCommands = 2f;
	float delay = 2f;
	float delay_short = 1f;
	WorldManager worldManager;
	AStar pathfinding;
	GameGrid grid;
	public MovementHandler movementHandler;
	PathRequestManager requestManager;
	[SerializeField]
	bool waitingForCommand = false;
	[SerializeField]
	bool waitingForNull = false;
	[SerializeField]
	bool givingCommands = false;

	private void Start () {
		grid = GameGrid.instance;
		worldManager = WorldManager.instance;
		WorldManager.AddUnitToAIList += AddUnitToAIList;
		WorldManager.RemoveUnitFromAIList += RemoveUnitFromAIList;
		pathfinding = FindObjectOfType<AStar> ().GetComponent<AStar> ();
		inputHandler = FindObjectOfType<AI_InputHandler> ().GetComponent<AI_InputHandler> ();
		movementHandler = FindObjectOfType<MovementHandler> ().GetComponent<MovementHandler> ();
		requestManager = FindObjectOfType<PathRequestManager> ().GetComponent<PathRequestManager> ();
		if (ai_units.Count > 0) {
			SortWaitCommands (timeBetweenCommands);
		} else {
			Debug.Log ("no enemies");
		}
	}

	private void OnDestroy () {
		WorldManager.AddUnitToAIList -= AddUnitToAIList;
		WorldManager.RemoveUnitFromAIList -= RemoveUnitFromAIList;
	}
	internal void Store_AI_Units (List<Unit> inc_ai_units) {
		ai_units = inc_ai_units;
	}

	internal void AddUnitToAIList (Unit unit) {
		if (!ai_units.Contains (unit)) {
			ai_units.Add (unit);
		}
		if (currentCommand == null) {
			SortWaitCommands (timeBetweenCommands);
		}
	}

	internal void RemoveUnitFromAIList (Unit unit) {
		if (ai_units.Contains (unit)) {
			ai_units.Remove (unit);
		}
	}

	internal void StartManagerCommands () {
		if (currentCommand == null) {
			if (worldManager.GetAllPlayerUnits ().Count > 0) {

				givingCommands = true;
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
						currentCommand.execute (abil, unitToControl, targetNode, inputHandler, timeBetweenCommands);
						givingCommands = false;
						SortWaitCommands (timeBetweenCommands);
					}
				} else {
					Debug.Log ("No units available");
					SortWaitCommands (timeBetweenCommands);
				}
			} else {
				Debug.Log ("probably messed up in the wait for commands loop");
				// CycleCommands ();
			}
		} else {
			Debug.Log ("all players died");
		}
	}

	// private void CycleCommands () {
	// 	if (waitingForNull) {

	// 	} else {
	// 		StartCoroutine ("WaitForNoCommands");
	// 	}
	// }

	// IEnumerator WaitForNoCommands () {
	// 	waitingForNull = true;
	// 	while (currentCommand != null) {
	// 		yield return new WaitForSeconds (.1f);
	// 	}
	// 	waitingForNull = false;
	// 	if (waitingForCommand) {
	// 		Debug.Log ("still waiting on command");
	// 	}
	// 	SortWaitCommands (timeBetweenCommands);
	// }

	private void RetryCommandUnit (Unit unit) {
		SortWaitCommands (.1f);
		unit.currentUnitState = Unit.UnitState.idle;
	}

	private void QueryForNode (Ability abil, Unit unitToControl, Node targetNode, Unit unit) {
		grid.ResetNodeCosts ();
		List<Node> possibleNodes = inputHandler.ReturnPossibleNodes (abil, unitToControl, targetNode);

		// Action<Ability, Unit, Node, List<Node>> test = AIPathTestFunc;
		// (abil, unitToControl, targetNode, possibleNodes)

		PathRequestManager.PathRequest _newRequest = new PathRequestManager.PathRequest (
			unitToControl.transform.position,
			targetNode.transform.position,
			SomeFakeCallback,
			SomeFakeMethod,
			RetryCommandUnit);

		PathRequestManager.RequestPath (
			unitToControl.transform.position,
			targetNode.transform.position,
			SomeFakeCallback,
			movementHandler,
			unit,
			SomeFakeMethod,
			RetryCommandUnit,
			PathRequestFinishedForAI,
			abil,
			possibleNodes);

		// requestManager.pathRequestQueue.Enqueue (_newRequest);
		// if (requestManager.canProcessNewRequest ()) {
		// 	requestManager.NewPathLogic ();
		// 	StartCoroutine (
		// 		movementHandler.GeneratePathForAI (unitToControl.transform.position,
		// 			targetNode.transform.position, abil, unitToControl, targetNode,
		// 			possibleNodes, PathRequestFinishedForAI)
		// 	);
		// } else {
		// 	Debug.LogError ("i dont know what to do here");
		// 	SortWaitCommands(timeBetweenCommands);
		// }
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

	// private void AIPathTestFunc (
	// 	Ability abil,
	// 	Unit unitToControl,
	// 	Node targetNode,
	// 	List<Node> possibleNodes) {
	// 	Node newTarget = SortNodes (possibleNodes, abil);
	// 	FinishExecution (abil, unitToControl, targetNode, newTarget);
	// 	grid.ResetNodeCosts ();
	// }

	private void FinishExecution (Ability abil, Unit unitToControl, Node targetNode, Node newTarget) {
		if (newTarget != null && unitToControl.isAlive) {
			ExecuteMovementRequest (unitToControl, newTarget, abil);
		}
	}

	private void ExecuteMovementRequest (Unit unitToControl, Node targetNode, Ability abil) {
		currentCommand = new AI_Ability_Command ();
		currentCommand.execute (abil, unitToControl, targetNode, inputHandler, delay);
		givingCommands = false;
		SortWaitCommands (timeBetweenCommands + delay);
	}

	public void ResetAICommands () {
		currentCommand = new AI_Ability_Command ();
		SortWaitCommands (timeBetweenCommands + delay_short);
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

	private void SortWaitCommands (float timeToWait) {
		if (waitingForCommand) {

		} else {
			waitingForCommand = true;
			StartCoroutine ("WaitForNextCommand", timeToWait);
		}
	}

	IEnumerator WaitForNextCommand (float timeToWait) {
		yield return new WaitForSeconds (timeToWait);
		currentCommand = null;
		waitingForCommand = false;
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
				switch (ability.abilityInfo.targetingBehavior) {
					case Ability.TargetingBehavior.line:
						// if it is line targeting, we need to be directly facing the target
						if (grid.NodeFromWorldPosition (
								unitToControl.transform.position
							).transform.position.x ==
							targetNode.transform.position.x ||
							grid.NodeFromWorldPosition (
								unitToControl.transform.position
							).transform.position.y ==
							targetNode.transform.position.y
						) {
							inAttackRange = true;
							attackAbilityToUse = ability;
						}
						break;
					case Ability.TargetingBehavior.square:
						inAttackRange = true;
						attackAbilityToUse = ability;
						break;
					default:
						break;
				}
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