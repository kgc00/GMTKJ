using System.Collections;
using UnityEngine;
public class Node : MonoBehaviour
{

    public bool walkable;
    public Sprite nodeSprite;
    public Vector3 worldPosition;
    public Node parent;
    public int gridX;
    public int gridY;
    public enum NodeType { plains, rough };
    public enum OccupiedByUnit { noUnit, enemy, ally };
    public NodeType nodeType;
    public OccupiedByUnit occupiedByUnit;

    public int gCost;
    public int hCost;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY,
    NodeType _nodeType, OccupiedByUnit _occupiedByUnit, Sprite _nodeSprite)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        nodeType = _nodeType;
        occupiedByUnit = _occupiedByUnit;
        nodeSprite = _nodeSprite;
    }

    public Node SetReferences(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY,
    NodeType _nodeType, OccupiedByUnit _occupiedByUnit, Sprite _nodeSprite)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        nodeType = _nodeType;
        occupiedByUnit = _occupiedByUnit;
        nodeSprite = _nodeSprite;
        return this;
    }
}
