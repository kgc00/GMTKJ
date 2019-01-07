using System;
using System.Collections;
using UnityEngine;
public class KnightThrownProjectile : ProjectileGMTK {

    private Vector3 targetPos;
    private Vector3 startPos;
    private float speed = 1f;
    private Coroutine currentRoutine;
    public event Action<Unit> callback = delegate { };
    private Unit owner;

    //  private void Awake () {
    //      ProjectileInfo.sprite = 
    //      ProjectileInfo.speed = 5f;
    //      ProjectileInfo.sprite = 5f;
    //  }

    public void thingo (Vector3 _startPos, Vector3 _targetPos, Unit _owner, Action<Unit> onConnected = null) {
        if (currentRoutine != null) {
            StopCoroutine (MoveToTarget (startPos, targetPos));
        }
        if (onConnected != null) {
            callback = onConnected;
        } else {
            callback = delegate { };
        }
        owner = _owner;
        SetPositions (_startPos, _targetPos);
        currentRoutine = StartCoroutine (MoveToTarget (startPos, targetPos));
    }
    public void SetPositions (Vector3 _startPos, Vector3 _targetPos) {
        targetPos = _targetPos;
        startPos = _startPos;
    }

    IEnumerator MoveToTarget (Vector3 _startPos, Vector3 _targetPos, Action<Unit> onConnected = null) {
        yield return new WaitForSeconds (.15f);
        while (true) {
            if (gameObject.transform.position != targetPos) {
                gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, targetPos, speed * Time.deltaTime);
            } else {
                onFinished ();
                yield break;
            }
            yield return null;
        }
    }

    private void onFinished () {
        Destroy (gameObject, .25f);
    }

    private void OnTriggerEnter (Collider other) {
        if (other.GetComponentInParent<Unit> ()) {
            if (other.transform.parent.gameObject != owner.gameObject) {
                Debug.Log ("got thru if statement: " + other.transform);
                if (callback != null) {
                    callback (other.GetComponentInParent<Unit> ());
                } else {
                    Debug.Log ("hit a collider");
                }
            } else {
                Debug.Log (other);
            }
        }
    }
}