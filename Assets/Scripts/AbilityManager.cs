using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Unit))]
public class AbilityManager : MonoBehaviour {

    //ABILITY MANAGER
    // called by input manager to determine what attack is to be used
    // contains a modular list of abilities the unit can call.
    //
    // All logic for the abilities will be on the abilities themselves.

    // private AbilityUI ui;
    [SerializeField]
    List<Ability> equippedAbilities;
    [SerializeField]
    private bool equipAbilitiesInEditor;
    private AbilityUI abilityUI;
    Unit unit;
    private int currentAttackSlot;
    void Awake () {
        if (!equipAbilitiesInEditor) {
            equippedAbilities = UpdateAbilities ();
        }
        unit = GetComponent<Unit> ();
        abilityUI = FindObjectOfType<AbilityUI> ().GetComponent<AbilityUI> ();
    }

    private List<Ability> UpdateAbilities () {
        if (GetComponentsInChildren<Ability> () != null) {
            foreach (Ability ability in GetComponentsInChildren<Ability> ()) {
                equippedAbilities.Add (ability);
            }
            return equippedAbilities;
        }
        Debug.LogError ("No abilities assigned to unit");
        return new List<Ability> ();
    }

    public List<Ability> ReturnEquippedAbilities () {
        return equippedAbilities;
    }

    public bool AnimateAbilitySelection (int v) {
        if (equippedAbilities[v] != null) {
            currentAttackSlot = v;
            abilityUI.RequestAnimation (unit, v, true);
            return true;
        }
        return false;
    }

    public bool AnimateAbilityUse (int v) {
        if (unit.faction == Unit.Faction.Player) {
            if (equippedAbilities[v] != null) {
                abilityUI.RequestAnimation (unit, v, false);
                return true;
            }
        }

        return false;
    }

    public int ReturnCurrentAttack () {
        return currentAttackSlot;
    }

    public Ability.AbilityInfo ReturnAbilityInfo () {
        return equippedAbilities[currentAttackSlot].abilityInfo;
    }

    public Ability ReturnAbility () {
        return equippedAbilities[currentAttackSlot];
    }

    public Ability GetCurrentAbility (int v) {
        return equippedAbilities[v];
    }
}