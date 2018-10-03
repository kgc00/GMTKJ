using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Knight/BashKnight")]
public class BashKnight : AttackAbility
{
    [SerializeField]
    private float lengthOfStun = 2.0f;
    UnitStateHandler stateHandler;
    public override void OnCalled(Unit unit)
    {
        if (!stateHandler)
        {
            stateHandler = FindObjectOfType<UnitStateHandler>().GetComponent<UnitStateHandler>();
        }
        stateHandler.SetState(unit, Unit.UnitState.planningAttack);
    }

    public override void OnAbilityConnected(Unit targetedUnit)
    {
        if (targetedUnit.currentUnitState != Unit.UnitState.cooldown)
        {
            UnitStateHandler.onUnitStunned(targetedUnit, lengthOfStun);
            stateHandler.SetState(targetedUnit, Unit.UnitState.cooldown);
        }
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
