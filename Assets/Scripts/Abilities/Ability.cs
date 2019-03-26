using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject {
    public AbilityInfo abilityInfo;
    public enum TargetingBehavior {
        line,
        square
    }

    public enum AbilityType {
        attack,
        movement,
        buff
    }

    [System.Serializable]
    public struct AbilityInfo {
        public int attackPower;
        public int attackRange;
        public float cooldownTime;
        public Sprite abilityIcon;
        public TargetingBehavior targetingBehavior;
        public AbilityType abilityType;
        public AbilityTargetingData infoTheSecond;
        public List<Node> nodesInAbilityRange;

        [SerializeField]
        public AbilityInfo (int _attackpower, float _cooldownTime, int _attackRange, AbilityType _abilityType,
            Sprite _abilityIcon, TargetingBehavior _targetingBehavior, System.Action _onCalled, AbilityTargetingData _infoTheSecond,
            List<Node> _nodesInAbilityRange = null) {
            attackPower = _attackpower;
            cooldownTime = _cooldownTime;
            attackRange = _attackRange;
            abilityType = _abilityType;
            abilityIcon = _abilityIcon;
            targetingBehavior = _targetingBehavior;
            infoTheSecond = _infoTheSecond;
            nodesInAbilityRange = _nodesInAbilityRange;
        }
    }

    public abstract void OnCalled (Unit unit);
    public abstract void OnCommited (Unit unit);
    public abstract void OnFinished (Unit unit);
}