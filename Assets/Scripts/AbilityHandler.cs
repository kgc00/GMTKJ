using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    public static void HandleIncomingAbility(Unit unit, Ability ability)
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