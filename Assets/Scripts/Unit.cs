﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IDamageable {
    public enum SelectionState {
        selected,
        notSelected
        };
        public enum UnitState {
        planningAction,
        acting,
        cooldown,
        idle
        };
        public enum Faction {
        Player,
        Enemy
        };
        [SerializeField]
        public Faction faction;
        [SerializeField]
        public UnitState currentUnitState;
        public SelectionState currentSelectionState;
        public int maxHealth, currentHealth, attackPower;
        public static event Action<Unit> OnUnitDeath = delegate { };

        public static event Action<Unit, int, int, int> OnDamageTaken = delegate { };
        private CollisionDetection colDet;
        private GameGrid gridRef;
        public Ability currentAbility = null;
        public bool isAlive = true;

        void Awake () {
        currentUnitState = UnitState.idle;
        currentSelectionState = SelectionState.notSelected;
        currentHealth = maxHealth;
        attackPower = 1;

        gridRef = FindObjectOfType<GameGrid> ().GetComponent<GameGrid> ();

        colDet = GetComponentInChildren<Collider> ().gameObject.AddComponent<CollisionDetection> ();
        colDet.Initializer (GetComponentInChildren<BoxCollider> (), gridRef);
        colDet.enabled = false;

    }
    protected void UnitDeath () {
        this.GetComponentInChildren<SpriteRenderer> ().enabled = false;
        this.isAlive = false;
        OnUnitDeath (this);
        Destroy (gameObject, 2.0f);
    }

    public void TakeDamage (int incomingDamage) {
        OnDamageTaken (this, currentHealth, maxHealth, incomingDamage);
        currentHealth -= incomingDamage;
        CheckForUnitDeath ();
    }

    protected void CheckForUnitDeath () {
        if (currentHealth <= 0) {
            UnitDeath ();
        }
    }

    internal void EnableDet (Action<Unit> onCollision) {
        colDet.EnableAlerts (onCollision);
    }

    internal void EnableDetWithAlerts (Action<Unit> onCollision, Action<List<Node>> nodesCollector) {
        colDet.EnableAlertsWithNodes (onCollision, nodesCollector);
    }

    public void DisableDet (Ability abil) {
        colDet.DisableAlerts (abil);
        colDet.enabled = false;
    }

    public void SetCurrentAbility (Ability abil) {
        currentAbility = abil;
    }
}