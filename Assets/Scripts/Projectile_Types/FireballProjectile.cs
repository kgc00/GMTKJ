using System;
using System.Collections;
using UnityEngine;
public class FireballProjectile : ProjectileGMTK {

    private Vector3 targetPos;
    private Vector3 startPos;
    private float speed = 1f;
    private Coroutine currentRoutine;
    public event Action<Node> callback = delegate { };
    private Unit owner;
    GameGrid grid;

    //  private void Awake () {
    //      ProjectileInfo.sprite = 
    //      ProjectileInfo.speed = 5f;
    //      ProjectileInfo.sprite = 5f;
    //  }

    public void FireProjectile (Vector3 _startPos, Vector3 _targetPos, Unit _owner, Action<Node> onConnected = null) {
        SetRefs (_startPos, _targetPos, _owner, onConnected);
        currentRoutine = StartCoroutine (MoveToTarget (startPos, targetPos));
    }

    private void SetRefs (Vector3 _startPos, Vector3 _targetPos, Unit _owner, Action<Node> onConnected) {
        if (currentRoutine != null) {
            StopCoroutine (MoveToTarget (startPos, targetPos));
        }
        if (onConnected != null) {
            callback = onConnected;
        } else {
            callback = delegate { };
        }
        if (GameGrid.instance != null) {
            grid = GameGrid.instance;
        }
        owner = _owner;
        SetPositions (_startPos, _targetPos);
    }

    public void SetPositions (Vector3 _startPos, Vector3 _targetPos) {
        targetPos = _targetPos;
        startPos = _startPos;
    }

    IEnumerator MoveToTarget (Vector3 _startPos, Vector3 _targetPos, Action<Node> onConnected = null) {
        yield return new WaitForSeconds (.15f);
        while (true) {
            if (gameObject.transform.position != targetPos) {
                gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, targetPos, speed * Time.deltaTime);
            } else {
                callback (GameGrid.instance.NodeFromWorldPosition (gameObject.transform.position));
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
                if (callback != null) {
                    callback (grid.NodeFromWorldPosition (other.transform.position));
                    if (currentRoutine != null) {
                        StopCoroutine (currentRoutine);
                    }
                    onFinished ();
                }
            }
        }
    }
}