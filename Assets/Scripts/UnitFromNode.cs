using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UnitFromNode {
    public static Unit SingleUnitFromNode (Node _selectedNode) {
        try {
            Unit affectedUnit;
            Collider[] allyColliders = Physics.OverlapSphere (_selectedNode.worldPosition, GameGrid.instance.nodeRadius / 2, GameGrid.instance.allyMask);
            Collider[] enemyColliders = Physics.OverlapSphere (_selectedNode.worldPosition, GameGrid.instance.nodeRadius / 2, GameGrid.instance.enemyMask);
            var hitColliders = allyColliders.Select (x => x).Concat (enemyColliders.Select (x => x));

            foreach (Collider collider in hitColliders) {
                affectedUnit = collider.gameObject.GetComponentInParent<Unit> ();
                if (affectedUnit != null) {
                    return affectedUnit;
                }
            }
        } catch (System.Exception e) {
            Debug.Log (e);
        }
        return null;
    }
}