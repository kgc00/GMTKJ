 using UnityEngine;
 public abstract class ProjectileGMTK : MonoBehaviour {
     public struct ProjectileInfo {
         public Sprite sprite;
         public float speed;
         public ProjectileInfo (Sprite _sprite, float _speed) {
             sprite = _sprite;
             speed = _speed;
         }
     }
 }