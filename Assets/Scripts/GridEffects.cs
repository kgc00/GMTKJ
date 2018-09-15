using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridEffects : MonoBehaviour
{
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
    private List<GameObject> nodesToRemovedFromPath;

    void Start()
    {
        UnitMovement.onGenerateMovementRange += InitiateMovementHighlights;
        UnitMovement.onGeneratePath += GeneratePath;
        AttackTargeting.onGenerateAttackRange += InitiateAttackHighlights;
        UnitStateHandler.onUnitPastPlanning += ClearHighlights;
        GameGrid.requestingHighlights += SpawnHighlightsForNode;
        pathList = new List<GameObject>();
        nodesToRemovedFromPath = new List<GameObject>();
    }

    private void SpawnHighlightsForNode(Node _node)
    {
        GameObject movementHighlightGO = Instantiate(selectionHighlight, _node.gameObject.transform);
        movementHighlightGO.transform.localPosition = new Vector3(0, 0, 0);
        SpriteRenderer movementSpriteRenderer = movementHighlightGO.GetComponent<SpriteRenderer>();
        movementSpriteRenderer.sprite = movementHighlight;
        movementSpriteRenderer.sortingOrder = 1;
        movementHighlightGO.name = movementName;
        movementHighlightGO.GetComponent<Animation>().playAutomatically = false;
        GameObject attackHighlightGO = Instantiate(selectionHighlight, _node.gameObject.transform);
        attackHighlightGO.transform.localPosition = new Vector3(0, 0, 0);
        SpriteRenderer attackSpriteRenderer = attackHighlightGO.GetComponent<SpriteRenderer>();
        attackSpriteRenderer.sprite = attackHighlight;
        attackSpriteRenderer.sortingOrder = 1;
        attackHighlightGO.name = attackName;
        attackHighlightGO.GetComponent<Animation>().playAutomatically = false;
        GameObject pathHighlightGO = Instantiate(selectionHighlight, _node.gameObject.transform);
        pathHighlightGO.transform.localPosition = new Vector3(0, 0, 0);
        SpriteRenderer pathSpriteRenderer = pathHighlightGO.GetComponent<SpriteRenderer>();
        pathSpriteRenderer.sprite = this.pathHighlight;
        pathSpriteRenderer.sortingOrder = 2;
        pathHighlightGO.name = pathName;
        // come back here and polish
        pathHighlightGO.GetComponent<Animation>().enabled = false;

        movementHighlightGO.SetActive(false);
        attackHighlightGO.SetActive(false);
        pathHighlightGO.SetActive(false);
    }

    private void InitiateMovementHighlights(List<Node> _nodesToHighlight)
    {
        CreateHighlightForNodes(_nodesToHighlight, movementHighlight, movementName);
    }

    private void InitiateAttackHighlights(List<Node> _nodesToHighlight)
    {
        CreateHighlightForNodes(_nodesToHighlight, attackHighlight, attackName);
    }

    private void CreateHighlightForNodes(List<Node> _nodesToHighlight, Sprite _highlightToUse, string _name)
    {
        selectionArray = new GameObject[_nodesToHighlight.Count];
        int arrayCount = 0;
        foreach (Node node in _nodesToHighlight)
        {
            selectionArray[arrayCount] = node.gameObject;
            // selectionArray[arrayCount].transform.Find(_name).gameObject.SetActive(true);
            arrayCount++;
        }
    }

    private void GeneratePath(List<Node> _nodesToHighlight)
    {
        List<GameObject> _incomingPathList = CreateNewPath(_nodesToHighlight);
        RemoveOldPath(_incomingPathList);
    }

    private List<GameObject> CreateNewPath(List<Node> _nodesToHighlight)
    {
        List<GameObject> _incomingPathList = _nodesToHighlight.ConvertAll(node => node.gameObject);

        foreach (GameObject _nodeGameObject in _incomingPathList)
        {
            // foreach node see if we've already generated a  highlight
            if (_nodeGameObject.transform.Find(pathName))
            {
                // _nodeGameObject.transform.Find(pathName).gameObject.SetActive(true);
                if (!pathList.Contains(_nodeGameObject.transform.Find(pathName).gameObject))
                {
                    // pathList.Add(_nodeGameObject.transform.Find(pathName).gameObject);
                }
                else
                {
                }
            }
        }
        return _incomingPathList;
    }

    private void RemoveOldPath(List<GameObject> _incomingPathList)
    {
        foreach (GameObject _childHighlight in pathList)
        {
            if (!_incomingPathList.Contains(_childHighlight.transform.parent.gameObject))
            {
                _childHighlight.SetActive(false);
            }
        }
        pathList.RemoveAll(_nodeGameObjects => !_incomingPathList.Contains(_nodeGameObjects.transform.parent.gameObject));
    }

    private void ClearHighlights(Unit _unit)
    {
        if (selectionArray != null)
        {
            foreach (GameObject highlight in selectionArray)
            {
                highlight.transform.Find(pathName).gameObject.SetActive(false);
                highlight.transform.Find(attackName).gameObject.SetActive(false);
                highlight.transform.Find(movementName).gameObject.SetActive(false);
            }
            Array.Clear(selectionArray, 0, selectionArray.Length);
        }
        if (pathList != null)
        {
            foreach (GameObject highlight in pathList)
            {
                highlight.SetActive(false);
            }
            pathList.Clear();
        }
    }
}
