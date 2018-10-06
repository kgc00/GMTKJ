using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Generic/AttackGeneric")]
public class AttackGeneric : AttackAbility
{
    public override void OnCalled(Unit unit)
    {
        FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>().SetState(unit, Unit.UnitState.planningAttack);
    }

    public override void OnAbilityConnected(Unit targetedUnit)
    {
        throw new System.NotImplementedException();
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
