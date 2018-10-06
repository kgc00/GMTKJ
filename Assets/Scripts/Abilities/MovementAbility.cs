using UnityEngine;

[CreateAssetMenu(menuName = "Ability/MovementAbility")]
public abstract class MovementAbility : Ability
{
    public override void OnCalled(Unit unit)
    {
        FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>().SetState(unit, Unit.UnitState.planningMovement);
    }

    public override void OnCommited(Unit unit)
    {
    }

    public abstract void OnDestinationReached(Unit unit);

    public override void OnFinished(Unit unit)
    {
    }
}