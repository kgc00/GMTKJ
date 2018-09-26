using UnityEngine;

[CreateAssetMenu]
public class Ability : ScriptableObject
{
    public enum TargetingBehavior
    {
        line, square
    }

    [System.Serializable]
    public struct AbilityInfo
    {
        public int attackPower;
        public int attackRange;
        public float cooldownTime;
        public Sprite abilityIcon;
        public TargetingBehavior targetingBehavior;

        [SerializeField]
        public AbilityInfo(int _attackpower, float _cooldownTime, int _attackRange,
        Sprite _abilityIcon, TargetingBehavior _targetingBehavior)
        {
            attackPower = _attackpower;
            cooldownTime = _cooldownTime;
            attackRange = _attackRange;
            abilityIcon = _abilityIcon;
            targetingBehavior = _targetingBehavior;
        }
    }
    public AbilityInfo abilityInfo;
}