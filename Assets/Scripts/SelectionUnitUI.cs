using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUnitUI : MonoBehaviour
{

    public Image selectionImage;

    void Awake()
    {
        selectionImage = transform.Find("Selected Image").GetComponent<Image>();
        UnitSelectionHandler.onUnitSelected += ShowSelection;
        UnitStateHandler.onUnitPastPlanning += HideSelection;
        this.transform.Find("Selected Image").GetComponent<Image>().enabled = false;
    }
    
    void ShowSelection(Unit unit)
    {
        unit.transform.Find("Selected Canvas/Selected Image").GetComponent<Image>().enabled = true;
    }

    void HideSelection(Unit unit)
    {
        unit.transform.Find("Selected Canvas/Selected Image").GetComponent<Image>().enabled = false;
    }
}
