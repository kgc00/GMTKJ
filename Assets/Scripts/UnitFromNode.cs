using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitFromNode
{
    public static Unit SingleUnitFromNode(Node _selectedNode)
    {
        Unit affectedUnit;
        Collider[] hitColliders = Physics.OverlapSphere(_selectedNode.worldPosition,
        GameGrid.instance.nodeRadius / 2, GameGrid.instance.allyMask);
        foreach (Collider collider in hitColliders)
        {
            affectedUnit = collider.gameObject.GetComponentInParent<Unit>();
            if (affectedUnit != null)
            {
                return affectedUnit;
            }
        }
        return null;
    }
}
