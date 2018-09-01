﻿using System.Collections;
using UnityEngine;
public class Node
{

    public bool walkable;
    public Vector3 worldPosition;
    public Node parent;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}
