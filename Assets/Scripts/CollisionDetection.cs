using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField]
    private bool alertsEnabled;
    event Action<Unit> onCollision;
    event Action<List<Node>> withNodes;
    Collider col;
    GameGrid grid;
    private List<Node> nodesTraversedSinceEnabled;
    private int nodesInMemory = 20;
    public static event Action<List<Node>> noderino = delegate { };

    public void Initializer(Collider _col, GameGrid _grid)
    {
        col = _col;
        grid = _grid;
        alertsEnabled = false;
        nodesTraversedSinceEnabled = new List<Node>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Node tile = other.GetComponent<Node>();
        if (tile != null)
        {
            grid.TryAddNodeToUnitList(tile);
            if (alertsEnabled)
            {
                nodesTraversedSinceEnabled.Add(tile);
            }
        }
        if (other.GetComponentInParent<Unit>())
        {
            if (alertsEnabled)
            {
                if (withNodes != null)
                {
                    withNodes(nodesTraversedSinceEnabled);
                    nodesTraversedSinceEnabled.Clear();
                }
                onCollision(other.GetComponentInParent<Unit>());
            }
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

    internal void DisableAlerts(object abil)
    {
        alertsEnabled = false;
        onCollision = null;
        withNodes = null;
    }

    internal void EnableAlerts(Action<Unit> collisionCallback)
    {
        alertsEnabled = true;
        onCollision = collisionCallback;
    }
    internal void EnableAlertsWithNodes(Action<Unit> collisionCallback, Action<List<Node>> nodesCollector)
    {
        alertsEnabled = true;
        onCollision = collisionCallback;
        withNodes = nodesCollector;
        nodesTraversedSinceEnabled.Add(grid.NodeFromWorldPosition(this.transform.position));
    }
}