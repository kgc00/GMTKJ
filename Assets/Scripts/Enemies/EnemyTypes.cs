using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Enemy/EnemyTypes")]
public class EnemyTypes : ScriptableObject {
    [SerializeField]
    public List<GameObject> enemyTypes;
}