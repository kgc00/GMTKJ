using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField]
    Wave wave;

    void Start () {
        foreach (var spawn in wave.spawnData) {
            try {
                Unit currentSpawn = Instantiate (spawn.enemy, spawn.spawnPosition, Quaternion.identity).GetComponent<Unit> ();
                WorldManager.instance.AddUnitToMasterList (currentSpawn);
                WorldManager.instance.PlaceUnitIntoCorrectSubList (currentSpawn);
            } catch (System.Exception e) {
                Debug.LogError (e);
            }
        }
    }
}