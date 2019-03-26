using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Enemy/SpawnData")]
public class SpawnData : ScriptableObject {
    public Vector3 spawnPosition;
    public GameObject enemy;

    public SpawnData (Vector3 _spawnPosition,
        GameObject _enemy) {
        spawnPosition = _spawnPosition;
        enemy = _enemy;
    }
}