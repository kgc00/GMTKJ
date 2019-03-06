using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridEffects : MonoBehaviour {
    private const string movementName = "Movement Highlight";
    private const string attackName = "Attack Highlight";
    private const string pathName = "Path Highlight";
    public float masterAlphaValue = 0f;
    [SerializeField]
    private GameObject selectionHighlight;
    private Dictionary<Unit, GameObject[]> allSelections;
    private List<GameObject> allActiveSelections;
    [SerializeField]
    private Sprite attackHighlight;
    [SerializeField]
    private Sprite movementHighlight;
    [SerializeField]
    private Sprite pathHighlight;
    private Dictionary<Unit, List<GameObject>> allPaths;

    void Start () {
        UnitStateHandler.onUnitActing += ClearHighlightsParser;
        GameGrid.requestingHighlights += SpawnHighlightsForNode;
        allPaths = new Dictionary<Unit, List<GameObject>> ();
        allSelections = new Dictionary<Unit, GameObject[]> ();
        allActiveSelections = new List<GameObject> ();
    }

    private void OnDestroy () {
        UnitStateHandler.onUnitActing -= ClearHighlightsParser;
        GameGrid.requestingHighlights -= SpawnHighlightsForNode;
    }

    private void Update () {
        masterAlphaValue = Mathf.PingPong (Time.time, 0.8f);
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
        _pathSpriteRenderer.color = new Color (
            _pathSpriteRenderer.color.r,
            _pathSpriteRenderer.color.g,
            _pathSpriteRenderer.color.b,
            .75f);
        _pathSpriteRenderer.sortingOrder = 2;
        _pathHighlightGO.name = pathName;

        _movementHighlightGO.SetActive (false);
        _attackHighlightGO.SetActive (false);
        _pathHighlightGO.SetActive (false);
    }

    public void InitiateAbilityHighlights (Unit _unit, List<Node> _nodesToHighlight) {
        if (allSelections.ContainsKey (_unit)) {
            ClearHighlights (_unit);
        }
        ActivateHighlightForNodes (_nodesToHighlight, attackHighlight, attackName, _unit);
    }

    public void RenderSelectorHighlights (List<Node> _nodesToHighlight, Unit unit) {
        if (unit.faction == Unit.Faction.Enemy) {
            GeneratePath (_nodesToHighlight, unit);
        } else {
            GeneratePath (_nodesToHighlight, unit);
        }
    }

    private void ActivateHighlightForNodes (
        List<Node> _nodesToHighlight, Sprite _highlightToUse, string _name,
        Unit _unit
    ) {
        allSelections[_unit] = new GameObject[_nodesToHighlight.Count];
        int _arrayCount = 0;
        foreach (Node node in _nodesToHighlight) {
            allSelections[_unit][_arrayCount] = node.gameObject;
            allSelections[_unit][_arrayCount].transform.Find (_name).gameObject.SetActive (true);
            if (!allActiveSelections.Contains (node.gameObject)) {
                node.ToggleAnimation (node.gameObject.transform.Find (_name).GetComponent<SpriteRenderer> ());

            }
            allActiveSelections.Add (node.gameObject);
            _arrayCount++;
        }
    }

    private void GeneratePath (List<Node> _nodesToHighlight, Unit unit) {
        // Debug.Log ("Calling Generate Path for: " + unit.name);
        if (!allPaths.ContainsKey (unit)) {
            CreateNewPath (_nodesToHighlight, unit);
        }
        List<GameObject> _incomingPathList = AssignAndRenderPath (_nodesToHighlight, unit);
        RemoveOldPath (_incomingPathList, unit);
    }

    private void CreateNewPath (List<Node> _nodesToHighlight, Unit unit) {
        List<GameObject> tempPathList = new List<GameObject> ();
        foreach (Node node in _nodesToHighlight) {
            tempPathList.Add (node.transform.Find (pathName).gameObject);
        }
        allPaths.Add (unit, tempPathList);
    }

    private List<GameObject> AssignAndRenderPath (List<Node> _nodesToHighlight, Unit unit) {
        List<GameObject> _incomingPathList = _nodesToHighlight.ConvertAll (node => node.gameObject);

        // Debug.Log ("inc path list is: " + _incomingPathList.Count ());
        foreach (GameObject _nodeGameObject in _incomingPathList) {
            // foreach node see if we've already generated a  highlight
            if (allSelections[unit].Contains (_nodeGameObject)) {
                if (_nodeGameObject.transform.Find (pathName)) {
                    _nodeGameObject.transform.Find (pathName).gameObject.SetActive (true);
                    if (allPaths.ContainsKey (unit)) {
                        if (!allPaths[unit].Contains (_nodeGameObject.transform.Find (pathName).gameObject)) {
                            allPaths[unit].Add (_nodeGameObject.transform.Find (pathName).gameObject);
                        } else {
                            // Debug.Log ("this node is already contained by allPaths");
                        }
                    } else {
                        Debug.LogError ("allPaths does not contain a key for this unit");
                    }
                } else {
                    Debug.LogError ("cannot find the requested child: path sprite");
                }
            } else {
                Debug.LogError ("Cant find Node in selections list");
            }
        }
        return _incomingPathList;
    }

    private void RemoveOldPath (List<GameObject> _incomingPathList, Unit unit) {
        if (allPaths.ContainsKey (unit)) {
            foreach (GameObject _childHighlight in allPaths[unit]) {
                if (!_incomingPathList.Contains (_childHighlight.transform.parent.gameObject)) {
                    _childHighlight.SetActive (false);
                }
            }
            allPaths[unit].RemoveAll (_nodeGameObjects => !_incomingPathList.Contains (
                _nodeGameObjects.transform.parent.gameObject
            ));
        }
    }

    private void ClearHighlightsParser (Unit unit, Ability ability) {
        switch (unit.faction) {
            case Unit.Faction.Player:
                ClearHighlights (unit);
                break;
            case Unit.Faction.Enemy:
                ClearHighlights (unit);
                break;
            default:
                break;
        }
    }

    public void ClearHighlights (Unit _unit) {
        if (allSelections.ContainsKey (_unit)) {
            // better way to check?
            if (allSelections[_unit][0] != null) {
                // make giant set

                // sort through set to see if any duplicates are present
                var duplicates = allActiveSelections
                    .GroupBy (x => x)
                    .Where (x => x.Count () > 1)
                    .Select (x => x.Key);
                // if so, remove them from set, but don't disable the highlights
                foreach (GameObject _tileGameObject in allSelections[_unit]) {
                    if (duplicates.Contains (_tileGameObject)) {
                        // do nothing?
                    } else {
                        if (_tileGameObject.transform.Find (movementName).gameObject.activeInHierarchy) {
                            Node node = _tileGameObject.GetComponent<Node> ();
                            node.GetComponent<Node> ().ToggleAnimation (node.gameObject.transform.Find (movementName).GetComponent<SpriteRenderer> ());
                            _tileGameObject.transform.Find (movementName).gameObject.SetActive (false);
                        } else if (_tileGameObject.transform.Find (attackName).gameObject.activeInHierarchy) {
                            Node node = _tileGameObject.GetComponent<Node> ();
                            node.GetComponent<Node> ().ToggleAnimation (node.gameObject.transform.Find (attackName).GetComponent<SpriteRenderer> ());
                            _tileGameObject.transform.Find (attackName).gameObject.SetActive (false);
                        }
                    }

                    allActiveSelections.Remove (_tileGameObject);
                }
            }
            Array.Clear (allSelections[_unit], 0, allSelections[_unit].Length);
        }

        if (allPaths.ContainsKey (_unit)) {
            if (allPaths[_unit].Count > 0) {
                foreach (GameObject _pathHighlight in allPaths[_unit]) {
                    _pathHighlight.SetActive (false);
                }
            }
            allPaths[_unit].Clear ();
        }
    }
}