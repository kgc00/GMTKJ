using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Ability")]
public abstract class Ability : ScriptableObject
{
    public AbilityInfo abilityInfo;
    public enum TargetingBehavior
    {
        line, square
    }

    public enum AbilityType
    {
        attack, movement, buff
    }


    [System.Serializable]
    public struct AbilityInfo
    {
        public int attackPower;
        public int attackRange;
        public float cooldownTime;
        public Sprite abilityIcon;
        public TargetingBehavior targetingBehavior;
        public AbilityType abilityType;

        [SerializeField]
        public AbilityInfo(int _attackpower, float _cooldownTime, int _attackRange, AbilityType _abilityType,
        Sprite _abilityIcon, TargetingBehavior _targetingBehavior, System.Action _onCalled)
        {
            attackPower = _attackpower;
            cooldownTime = _cooldownTime;
            attackRange = _attackRange;
            abilityType = _abilityType;
            abilityIcon = _abilityIcon;
            targetingBehavior = _targetingBehavior;
        }
    }

    public abstract void OnCalled();
    public abstract void OnCommited();
    public abstract void OnFinished();
}