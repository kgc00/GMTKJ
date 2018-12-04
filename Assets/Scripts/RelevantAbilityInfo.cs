using UnityEngine;

public struct RelevantAbilityInfo
{
    public Vector3 startPos;
    public Vector3 targetPos;
    public int slot;
    public RelevantAbilityInfo(Vector3 _startPos, Vector3 _targetPos, int _slot)
    {
        startPos = _startPos;
        targetPos = _targetPos;
        slot = _slot;
    }
}
