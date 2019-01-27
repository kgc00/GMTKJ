using UnityEngine;

[CreateAssetMenu (menuName = "Ability/MovementAbility")]
public abstract class MovementAbility : Ability {
    public override void OnCalled (Unit unit) {
        FindObjectOfType<UnitStateHandler> ().GetComponent<UnitStateHandler> ().SetStatePlayerUnit (unit, Unit.UnitState.planningAction);
    }

    public override void OnCommited (Unit unit) { }

    public abstract void OnDestinationReached (Unit unit);

    public override void OnFinished (Unit unit) { }
}