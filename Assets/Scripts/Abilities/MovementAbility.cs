using UnityEngine;

[CreateAssetMenu(menuName = "Ability/MovementAbility")]
public class MovementAbility : Ability
{
    public override void OnCalled(Unit unit)
    {
        FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>().SetState(unit, Unit.UnitState.planningMovement);
    }

    public override void OnAbilityConnected(Unit unit)
    {

    }
    public override void OnCommited(Unit unit)
    {
        throw new System.NotImplementedException();
    }

    public override void OnFinished(Unit unit)
    {
        throw new System.NotImplementedException();
    }
}