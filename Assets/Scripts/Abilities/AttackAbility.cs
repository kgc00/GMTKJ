using UnityEngine;

public abstract class AttackAbility : Ability
{
    public abstract void OnAbilityConnected(Unit targetedUnit);
}
