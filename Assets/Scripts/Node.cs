using System.Collections;
using UnityEngine;
public class Node : MonoBehaviour {

    public bool walkable;
    public Sprite nodeSprite;
    public Vector3 worldPosition;
    public Node parent;
    public int gridX;
    public int gridY;
    public enum NodeType { plains, rough };
 public enum OccupiedByUnit { noUnit, enemy, ally };
 private GridEffects gridfx;
 public NodeType nodeType;
 public OccupiedByUnit occupiedByUnit;
 public bool shouldAnimate = false;
 SpriteRenderer rend = null;
 public int gCost;
 public int hCost;
 public int fCost {
 get {
 return gCost + hCost;
        }
    }

    public void ToggleAnimation (SpriteRenderer renderer) {
        if (shouldAnimate) {
            shouldAnimate = false;
            rend = null;
        } else {
            shouldAnimate = true;
            rend = renderer;
        }
    }

    private void Update () {
        if (rend && shouldAnimate) {
            rend.color = new Color (rend.color.r, rend.color.g, rend.color.b, gridfx.masterAlphaValue);
        }
    }

    public Node SetReferences (bool _walkable, Vector3 _worldPos, int _gridX, int _gridY,
        NodeType _nodeType, OccupiedByUnit _occupiedByUnit, Sprite _nodeSprite, GridEffects _gridfx) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        nodeType = _nodeType;
        occupiedByUnit = _occupiedByUnit;
        nodeSprite = _nodeSprite;
        gridfx = _gridfx;
        return this;
    }
}