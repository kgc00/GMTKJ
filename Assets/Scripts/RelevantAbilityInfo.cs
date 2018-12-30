using UnityEngine;

public struct AbilityTargetingData
{
    public Vector3 startPos;
    public Vector3 targetPos;
    public int slot;
    public AbilityTargetingData(Vector3 _startPos, Vector3 _targetPos, int _slot)
    {
        startPos = _startPos;
        targetPos = _targetPos;
        slot = _slot;
    }
}
