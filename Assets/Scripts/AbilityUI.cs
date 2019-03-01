using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour {

    [SerializeField]
    private Image[] abilityImages;
    [SerializeField]
    private AnimationClip[] clips;

    void Start () {
        AssignImages ();
        ResetAbilityPanelImages ();
        UnitSelectionHandler.onUnitSelectedByPlayer += PopulateAbilityPanel;
        UnitSelectionHandler.onUnitUnselectedByPlayer += ResetAbilityPanelImages;
        StopAnimations ();
    }

    private void OnDestroy () {
        UnitSelectionHandler.onUnitSelectedByPlayer -= PopulateAbilityPanel;
        UnitSelectionHandler.onUnitUnselectedByPlayer -= ResetAbilityPanelImages;
    }

    private void AssignImages () {
        abilityImages = new Image[4];
        for (int i = 0; i < abilityImages.Length; i++) {
            abilityImages[i] = GameObject.FindWithTag ("UI_Holder")
                .transform.Find ("Ability Holder")
                .transform.Find (i.ToString ())
                .GetComponent<Image> ();

        }
    }

    private void StopAnimations () {
        foreach (Image image in abilityImages) {
            image.GetComponent<Animation> ().Stop ();
        }
    }

    public void ResetAbilityPanelImages (Unit _unit = null) {
        foreach (Image image in abilityImages) {
            image.sprite = null;
            image.color = Color.white;
            image.enabled = true;
        }
    }
    public void PopulateAbilityPanel (Unit _unit) {
        List<Ability> abilities = _unit.GetComponent<AbilityManager> ().ReturnEquippedAbilities ();
        for (int i = 0; i < abilities.Count; i++) {
            if (i <= abilityImages.Length) {
                abilityImages[i].sprite = abilities[i].abilityInfo.abilityIcon;
                abilityImages[i].color = Color.white;
                abilityImages[i].enabled = true;
            }
        }
    }

    public void RequestAnimation (Unit _unit, int _slot, bool _forSelection) {
        if (_forSelection) {
            AnimateIconSelected (_unit, _slot);
        } else {
            AnimateIconCooldown (_unit, _slot);
        }
    }
    private void AnimateIconSelected (Unit _unit, int _slot) {
        abilityImages[_slot].GetComponent<Animation> ().clip = clips[0];
        abilityImages[_slot].GetComponent<Animation> ().Play ();
    }

    private void AnimateIconCooldown (Unit _unit, int _slot) {
        abilityImages[_slot].GetComponent<Animation> ().clip = clips[1];
        abilityImages[_slot].GetComponent<Animation> ().Play ();
    }
}