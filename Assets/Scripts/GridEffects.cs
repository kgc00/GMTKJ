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
    private List<GameObject> pathList;
    private List<GameObject> nodesToRemovedFromPath;

    void Start()
    {
        UnitMovement.onGenerateMovementRange += InitiateMovementHighlights;
        UnitMovement.onGeneratePath += GeneratePath;
        AttackTargeting.onGenerateAttackRange += InitiateAttackHighlights;
        UnitStateHandler.onUnitPastPlanning += ClearHighlights;
        pathList = new List<GameObject>();
        nodesToRemovedFromPath = new List<GameObject>();
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
            selectionArray[arrayCount] = Instantiate(selectionHighlight, node.gameObject.transform);
            // for some reason setting the transform gets use a local position of 1 on the x
            // so we fix that here.  better solution needed.
            selectionArray[arrayCount].transform.localPosition = new Vector3(0, 0, 0);
            selectionArray[arrayCount].GetComponent<SpriteRenderer>().sprite = _highlightToUse;
            selectionArray[arrayCount].GetComponent<SpriteRenderer>().sortingOrder = 1;
            selectionArray[arrayCount].name = _name;
            arrayCount++;
        }
    }

    private void GeneratePath(List<Node> _nodesToHighlight)
    {
        List<GameObject> incomingPathList = CreateNewPath(_nodesToHighlight);
        RemoveOldPath(incomingPathList);
    }

    private List<GameObject> CreateNewPath(List<Node> _nodesToHighlight)
    {
        List<GameObject> incomingPathList = _nodesToHighlight.ConvertAll(node => node.gameObject);

        foreach (GameObject highlight in incomingPathList)
        {
            // foreach node see if we've already generated a  highlight
            if (highlight.transform.Find(pathName))
            {
            }
            else
            {
                // otherwise, create a highlight, add it to the list.
                GameObject temp = Instantiate(selectionHighlight, highlight.transform);
                temp.transform.localPosition = new Vector3(0, 0, 0);
                temp.GetComponent<SpriteRenderer>().sprite = pathHighlight;
                temp.GetComponent<SpriteRenderer>().sortingOrder = 2;
                temp.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
                temp.GetComponent<Animation>().enabled = false;
                temp.name = pathName;
                pathList.Add(temp);
            }
        }
        return incomingPathList;
    }

    private void RemoveOldPath(List<GameObject> incomingPathList)
    {
        foreach (GameObject highlight in pathList)
        {
            if (!incomingPathList.Contains(highlight.transform.parent.gameObject))
            {
                Destroy(highlight);
            }
        }
        pathList.RemoveAll(highlight => !incomingPathList.Contains(highlight.transform.parent.gameObject));
    }

    private void ClearHighlights(Unit _unit)
    {
        if (selectionArray != null)
        {
            foreach (GameObject highlight in selectionArray)
            {
                Destroy(highlight);
            }
            Array.Clear(selectionArray, 0, selectionArray.Length);
        }
        if (pathList != null){
            foreach (GameObject highlight in pathList)
            {
                Destroy(highlight);
            }
            pathList.Clear();
        }
    }
}
