using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUnitUI : MonoBehaviour
{

    public Image selectionImage;
    UnitStateHandler unitStateHandler;

    void Awake()
    {
        selectionImage = transform.Find("Selected Image").GetComponent<Image>();
        unitStateHandler = GetComponentInParent<UnitStateHandler>();
        unitStateHandler.onUnitSelected += ShowSelection;
        unitStateHandler.onUnitPastPlanning += HideSelection;
        HideSelection();
    }
    void ShowSelection(Unit unit)
    {
        selectionImage.enabled = true;
    }

    void HideSelection()
    {
        selectionImage.enabled = false;
    }
}
