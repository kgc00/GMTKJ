using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField]
    Wave wave;

    void Start () {
        foreach (var spawn in wave.spawnData) {
            Instantiate (spawn.enemy, spawn.spawnPosition, Quaternion.identity);
        }
    }
}