using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{

    [SerializeField]
    private List<Image> abilityImages;
    [SerializeField]
    private AnimationClip[] clips;

    void Start()
    {
        UnitSelectionHandler.onUnitSelected += PopulateAbilityPanel;
        StopAnimations();
    }

    private void StopAnimations()
    {
        foreach (Image panel in abilityImages)
        {
            panel.GetComponent<Animation>().Stop();
        }
    }

    public void PopulateAbilityPanel(Unit _unit)
    {
        List<Ability> abilities = _unit.GetComponent<AbilityManager>().ReturnEquippedAbilities();
        for (int i = 0; i < abilities.Count; i++)
        {
            if (i <= abilityImages.Count)
            {
                abilityImages[i].sprite = abilities[i].abilityInfo.abilityIcon;
                abilityImages[i].color = Color.white;
                abilityImages[i].enabled = true;
            }
        }
    }

    public void RequestAnimation(Unit _unit, int _slot, bool _forSelection)
    {
        if (_forSelection)
        {
            AnimateIconSelected(_unit, _slot);
        }
        else
        {
            AnimateIconCooldown(_unit, _slot);
        }
    }
    private void AnimateIconSelected(Unit _unit, int _slot)
    {
        abilityImages[_slot].GetComponent<Animation>().clip = clips[0];
        abilityImages[_slot].GetComponent<Animation>().Play();
    }

    private void AnimateIconCooldown(Unit _unit, int _slot)
    {
        abilityImages[_slot].GetComponent<Animation>().clip = clips[1];
        abilityImages[_slot].GetComponent<Animation>().Play();
    }
}
