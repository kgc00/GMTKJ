﻿using System;
using System.Collections;
using UnityEngine;

public class PiercingArrow : ProjectileGMTK {

	private Vector3 targetPos;
	private Vector3 startPos;
	private float speed = 1f;
	private Coroutine currentRoutine;
	public event Action<Vector3> callback = delegate { };
	private Unit owner;

	//  private void Awake () {
	//      ProjectileInfo.sprite = 
	//      ProjectileInfo.speed = 5f;
	//      ProjectileInfo.sprite = 5f;
	//  }

	public void FireProjectile (Vector3 _startPos, Vector3 _targetPos, Unit _owner, Action<Vector3> onConnected = null) {
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
		GetComponent<SphereCollider> ().enabled = false;
		Destroy (gameObject, .25f);
	}

	private void OnTriggerEnter (Collider other) {
		if (other.GetComponentInParent<Unit> ()) {
			if (other.transform.parent.gameObject != owner.gameObject) {
				if (callback != null) {
					if (currentRoutine != null) {
						StopCoroutine (currentRoutine);
					}
					callback (other.transform.position);
					onFinished ();
				}
			}
		}
	}
}