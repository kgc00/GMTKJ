using UnityEngine;

public class AbilityHandler : MonoBehaviour {

    public void Start () {
        UnitStateHandler.onUnitPlanningAction += HandleIncomingAbility;
    }

    public static void HandleIncomingAbility (Unit unit, Ability ability) {
        ability.OnCalled (unit);
    }
}