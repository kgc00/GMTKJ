using UnityEngine;

public class AbilityHandler : MonoBehaviour
{

    public void Start()
    {
        UnitStateHandler.onUnitPlanningAction += HandleIncomingAbility;
    }

    public static void HandleIncomingAbility(Unit unit, Ability ability)
    {
        ability.OnCalled(unit);
    }

    private static void SwitchThing_Legacy(Ability ability)
    {
        switch (ability.abilityInfo.abilityType)
        {
            case Ability.AbilityType.attack:
                break;
            case Ability.AbilityType.movement:
                break;
            case Ability.AbilityType.buff:
                break;
            default:
                break;
        }
    }
}