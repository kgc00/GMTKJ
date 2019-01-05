using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEffects : MonoBehaviour {
    private const string movementName = "Movement Highlight";
    private const string attackName = "Attack Highlight";
    private const string pathName = "Path Highlight";
    [SerializeField]
    private GameObject selectionHighlight;
    [SerializeField]
    private GameObject[] selectionArray;
    [SerializeField]
    private Sprite attackHighlight;
    [SerializeField]
    private Sprite movementHighlight;
    [SerializeField]
    private Sprite pathHighlight;
    [SerializeField]
    private List<GameObject> pathList;

    void Start () {
        UnitMovement.onGenerateMovementRange += InitiateMovementHighlights;
        UnitMovement.onGeneratePath += GeneratePath;
        UnitStateHandler.onUnitActing += ClearHighlights;
        GameGrid.requestingHighlights += SpawnHighlightsForNode;
        pathList = new List<GameObject> ();
    }

    private void SpawnHighlightsForNode (Node _node) {
        GameObject _movementHighlightGO = Instantiate (selectionHighlight, _node.gameObject.transform);
        _movementHighlightGO.transform.localPosition = new Vector3 (0, 0, 0);
        SpriteRenderer _movementSpriteRenderer = _movementHighlightGO.GetComponent<SpriteRenderer> ();
        _movementSpriteRenderer.sprite = movementHighlight;
        _movementSpriteRenderer.sortingOrder = 1;
        _movementHighlightGO.name = movementName;
        GameObject _attackHighlightGO = Instantiate (selectionHighlight, _node.gameObject.transform);
        _attackHighlightGO.transform.localPosition = new Vector3 (0, 0, 0);
        SpriteRenderer _attackSpriteRenderer = _attackHighlightGO.GetComponent<SpriteRenderer> ();
        _attackSpriteRenderer.sprite = attackHighlight;
        _attackSpriteRenderer.sortingOrder = 1;
        _attackHighlightGO.name = attackName;
        GameObject _pathHighlightGO = Instantiate (selectionHighlight, _node.gameObject.transform);
        _pathHighlightGO.transform.localPosition = new Vector3 (0, 0, 0);
        SpriteRenderer _pathSpriteRenderer = _pathHighlightGO.GetComponent<SpriteRenderer> ();
        _pathSpriteRenderer.sprite = this.pathHighlight;
        _pathSpriteRenderer.sortingOrder = 2;
        _pathHighlightGO.name = pathName;
        // come back here and polish
        _pathHighlightGO.GetComponent<Animation> ().enabled = false;

        _movementHighlightGO.SetActive (false);
        _attackHighlightGO.SetActive (false);
        _pathHighlightGO.SetActive (false);
    }

    private void InitiateMovementHighlights (Unit _unit, List<Node> _nodesToHighlight) {
        if (selectionArray.Length > 0) {
            ClearHighlights (_unit);
        }
        ActivateHighlightForNodes (_nodesToHighlight, movementHighlight, movementName);
    }

    public void InitiateAbilityHighlights (Unit _unit, List<Node> _nodesToHighlight) {
        if (selectionArray.Length > 0) {
            ClearHighlights (_unit);
        }
        ActivateHighlightForNodes (_nodesToHighlight, attackHighlight, attackName);
    }

    public void RenderSelectorHighlights (List<Node> _nodesToHighlight) {
        GeneratePath (_nodesToHighlight);
    }

    private void ActivateHighlightForNodes (
        List<Node> _nodesToHighlight, Sprite _highlightToUse, string _name
    ) {
        selectionArray = new GameObject[_nodesToHighlight.Count];
        int _arrayCount = 0;
        foreach (Node node in _nodesToHighlight) {
            selectionArray[_arrayCount] = node.gameObject;
            selectionArray[_arrayCount].transform.Find (_name).gameObject.SetActive (true);
            _arrayCount++;
        }
    }

    private void GeneratePath (List<Node> _nodesToHighlight) {
        List<GameObject> _incomingPathList = CreateNewPath (_nodesToHighlight);
        RemoveOldPath (_incomingPathList);
    }

    private List<GameObject> CreateNewPath (List<Node> _nodesToHighlight) {
        List<GameObject> _incomingPathList = _nodesToHighlight.ConvertAll (node => node.gameObject);

        foreach (GameObject _nodeGameObject in _incomingPathList) {
            // foreach node see if we've already generated a  highlight
            if (_nodeGameObject.transform.Find (pathName)) {
                _nodeGameObject.transform.Find (pathName).gameObject.SetActive (true);
                if (!pathList.Contains (_nodeGameObject.transform.Find (pathName).gameObject)) {
                    pathList.Add (_nodeGameObject.transform.Find (pathName).gameObject);
                } else { }
            }
        }
        return _incomingPathList;
    }

    private void RemoveOldPath (List<GameObject> _incomingPathList) {
        foreach (GameObject _childHighlight in pathList) {
            if (!_incomingPathList.Contains (_childHighlight.transform.parent.gameObject)) {
                _childHighlight.SetActive (false);
            }
        }
        pathList.RemoveAll (_nodeGameObjects => !_incomingPathList.Contains (_nodeGameObjects.transform.parent.gameObject));
    }

    private void ClearHighlights (Unit _unit) {
        if (selectionArray != null) {
            // better way to check?
            if (selectionArray[0] != null) {
                foreach (GameObject _tileGameObject in selectionArray) {
                    if (_tileGameObject.transform.Find (movementName).gameObject.activeInHierarchy) {
                        _tileGameObject.transform.Find (movementName).gameObject.SetActive (false);
                    } else if (_tileGameObject.transform.Find (attackName).gameObject.activeInHierarchy) {
                        _tileGameObject.transform.Find (attackName).gameObject.SetActive (false);
                    }
                }
            }
            Array.Clear (selectionArray, 0, selectionArray.Length);
        }
        if (pathList != null) {
            if (pathList.Count > 0) {
                foreach (GameObject _pathHighlight in pathList) {
                    _pathHighlight.SetActive (false);
                }
            }
            pathList.Clear ();
        }
    }

    private void ClearHighlights (Unit _unit, Ability abil) {
        if (selectionArray != null) {
            // better way to check?
            if (selectionArray[0] != null) {
                foreach (GameObject _tileGameObject in selectionArray) {
                    if (_tileGameObject.transform.Find (movementName).gameObject.activeInHierarchy) {
                        _tileGameObject.transform.Find (movementName).gameObject.SetActive (false);
                    } else if (_tileGameObject.transform.Find (attackName).gameObject.activeInHierarchy) {
                        _tileGameObject.transform.Find (attackName).gameObject.SetActive (false);
                    }
                }
            }
            Array.Clear (selectionArray, 0, selectionArray.Length);
        }
        if (pathList != null) {
            if (pathList.Count > 0) {
                foreach (GameObject _pathHighlight in pathList) {
                    _pathHighlight.SetActive (false);
                }
            }
            pathList.Clear ();
        }
    }
    private void ClearHighlights () {
        if (selectionArray != null) {
            // better way to check?
            if (selectionArray[0] != null) {
                foreach (GameObject _tileGameObject in selectionArray) {
                    if (_tileGameObject.transform.Find (movementName).gameObject.activeInHierarchy) {
                        _tileGameObject.transform.Find (movementName).gameObject.SetActive (false);
                    } else if (_tileGameObject.transform.Find (attackName).gameObject.activeInHierarchy) {
                        _tileGameObject.transform.Find (attackName).gameObject.SetActive (false);
                    }
                }
            }
            Array.Clear (selectionArray, 0, selectionArray.Length);
        }
        if (pathList != null) {
            if (pathList.Count > 0) {
                Debug.Log ("pathlist");
                foreach (GameObject _pathHighlight in pathList) {
                    _pathHighlight.SetActive (false);
                }
            }
            pathList.Clear ();
        }
    }
}