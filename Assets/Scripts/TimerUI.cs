using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour {
	Image[] children = new Image[2];
	UnitTimer timer;

	void Awake(){
		timer = GetComponentInParent<UnitTimer>();
		timer.onTimerStarted += StartTimer;
		timer.onTimerRemoved += StopTimer;
		children = GetComponentsInChildren<Image>();
		StopTimer();
	}
	void StartTimer(){
		foreach (Image image in children)
		{
			image.enabled = true;
		}
	}

	void StopTimer(){
		foreach (Image image in children)
		{
			image.enabled = false;
		}
	}
}
