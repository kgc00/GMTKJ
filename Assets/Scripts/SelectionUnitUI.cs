using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUnitUI : MonoBehaviour {
    void Awake () {
        UnitSelectionHandler.onUnitSelected += ShowSelection;
        UnitSelectionHandler.onUnitUnselected += HideSelection;
        UnitStateHandler.onUnitActing += HideSelection;
    }

    void Start () {

    }

    void ShowSelection (Unit _unit) {
        _unit.transform.Find ("Selected Canvas/Selected Image").GetComponent<Image> ().enabled = true;
    }

    void HideSelection (Unit _unit) {
        _unit.transform.Find ("Selected Canvas/Selected Image").GetComponent<Image> ().enabled = false;
    }

    void HideSelection (Unit _unit, Ability abil) {
        _unit.transform.Find ("Selected Canvas/Selected Image").GetComponent<Image> ().enabled = false;
    }
}