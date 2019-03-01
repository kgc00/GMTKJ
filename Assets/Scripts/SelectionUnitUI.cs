using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUnitUI : MonoBehaviour {
    void Awake () {
        UnitSelectionHandler.onUnitSelectedByPlayer += ShowSelection;
        UnitSelectionHandler.onUnitUnselectedByPlayer += HideSelection;
        UnitStateHandler.onUnitActing += HideSelectionParser;
    }

    private void OnDestroy () {
        UnitSelectionHandler.onUnitSelectedByPlayer -= ShowSelection;
        UnitSelectionHandler.onUnitUnselectedByPlayer -= HideSelection;
        UnitStateHandler.onUnitActing -= HideSelectionParser;
    }

    void Start () {

    }

    void ShowSelection (Unit _unit) {
        _unit.transform.Find ("Selected Canvas/Selected Image").GetComponent<Image> ().enabled = true;
    }

    void HideSelection (Unit _unit) {
        _unit.transform.Find ("Selected Canvas/Selected Image").GetComponent<Image> ().enabled = false;
    }

    void HideSelectionParser (Unit _unit, Ability abil) {
        switch (_unit.faction) {
            case Unit.Faction.Player:
                HideSelection (_unit, abil);
                break;
            case Unit.Faction.Enemy:
                HideSelection (_unit, abil);
                break;
            default:
                break;
        }
    }

    void HideSelection (Unit _unit, Ability abil) {
        _unit.transform.Find ("Selected Canvas/Selected Image").GetComponent<Image> ().enabled = false;
    }
}