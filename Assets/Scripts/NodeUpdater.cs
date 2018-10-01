using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeUpdater : MonoBehaviour
{
    GameGrid grid;
    WorldManager worldManager;
    private void Awake()
    {
        grid = FindObjectOfType<GameGrid>().GetComponent<GameGrid>();
        worldManager = FindObjectOfType<WorldManager>().GetComponent<WorldManager>();
        List<Unit> allUnits = worldManager.GetAllUnits();
        if (allUnits != null)
        {
            foreach (Unit unit in allUnits)
            {
                Physics.IgnoreCollision(unit.GetComponentInChildren<Collider>(), GetComponent<Collider>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Node tile = other.GetComponent<Node>();
        if (tile != null)
        {
            grid.TryAddNodeToUnitList(tile);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Node tile = other.GetComponent<Node>();
        if (tile != null)
        {
            grid.TryRemoveNodeToUnitList(tile);
        }
    }
}