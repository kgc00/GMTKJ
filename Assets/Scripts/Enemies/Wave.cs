using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Enemy/Wave")]
public class Wave : ScriptableObject {
    public int waveNumber;
    public List<SpawnData> spawnData;

    public Wave (int _waveNumber,
        List<SpawnData> _spawnData) {
        waveNumber = _waveNumber;
        spawnData = _spawnData;
    }
}