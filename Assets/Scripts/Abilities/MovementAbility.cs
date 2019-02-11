using UnityEngine;

[CreateAssetMenu (menuName = "Ability/MovementAbility")]
public abstract class MovementAbility : Ability {
    public override void OnCalled (Unit unit) {
        FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ().SetUnitState (unit, Unit.UnitState.planningAction);
    }

    public override void OnCommited (Unit unit) { }

    public abstract void OnDestinationReached (Unit unit);

    public override void OnFinished (Unit unit) { }
}