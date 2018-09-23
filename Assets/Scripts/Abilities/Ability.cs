using UnityEngine;

[CreateAssetMenu]
public class Ability : ScriptableObject
{
    public enum TargettingBehavior
    {
        line, square
    }
    [SerializeField]
    public TargettingBehavior targettingBehavior;
    [System.Serializable]
    public struct AbilityInfo
    {
        public int attackPower;
        public float cooldownTime;
        public Sprite abilityIcon;

        [SerializeField]
        public AbilityInfo(int _attackpower, float _cooldownTime, Sprite _abilityIcon)
        {
            attackPower = _attackpower;
            cooldownTime = _cooldownTime;
            abilityIcon = _abilityIcon;
        }
    }
    public AbilityInfo abilityInfo;
}