using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingInformation {

	public Vector3 startingPoint;
	public Vector3 targetPoint;
	public TargetingInformation(Vector3 _startingPoint, Vector3 _targetPoint) {
		startingPoint = _startingPoint;
		targetPoint = _targetPoint;
	}
}
