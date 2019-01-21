using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SigilObject : ProjectileGMTK {

	Unit owner;
	Action<Unit> callback;
	internal void SpawnSigil (Unit unit, Action<Unit> explode) {
		owner = unit;
		callback = explode;
		StartCoroutine ("StartLifetime");
	}

	private void OnTriggerEnter (Collider other) {
		if (other.GetComponentInParent<Unit> ()) {
			if (callback != null && owner != null) {
				if (other.transform.parent.gameObject != owner.gameObject) {
					callback (other.GetComponentInParent<Unit> ());
				}
			}
		}
	}

	IEnumerator StartLifetime () {
		yield return new WaitForSeconds (9.75f);
		Destroy (gameObject, .25f);
		yield break;
	}
}