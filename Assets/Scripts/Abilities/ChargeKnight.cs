using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Knight/ChargeKnight")]
public class ChargeKnight : AttackAbility
{
    UnitStateHandler stateHandler;
    public override void OnCalled(Unit unit)
    {
        if (!stateHandler)
        {
            stateHandler = FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>();
        }
        stateHandler.SetState(unit, Unit.UnitState.planningAttack);
    }

    public override void OnAbilityConnected(Unit unit) { }

    public override void OnCommited(Unit unit)
    {
        throw new System.NotImplementedException();
    }

    public override void OnFinished(Unit unit)
    {
        throw new System.NotImplementedException();
    }
}
