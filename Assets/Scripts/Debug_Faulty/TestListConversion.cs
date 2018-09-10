using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestListConversion : MonoBehaviour {

	[SerializeField]
	Unit[] allUnits;
	[SerializeField]
	List<GameObject> allUnitsAsGameObjects;

	// Use this for initialization
	void Start () {
		allUnits = FindObjectsOfType<Unit>();
		// allUnitsAsGameObjects = allUnits.;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
