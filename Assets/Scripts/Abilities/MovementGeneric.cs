using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Generic/MovementGeneric")]
public class MovementGeneric : MovementAbility
{
    public override void OnCalled(Unit unit)
    {
        FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>().SetState(unit, Unit.UnitState.planningMovement);
    }

    public override void OnCommited(Unit unit)
    {
    }

    public override void OnDestinationReached(Unit unit) { }

    public override void OnFinished(Unit unit)
    {
    }
}