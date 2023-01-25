using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Performance : MonoBehaviour
{
	[SerializeField, Range(0.1f, 1f)]
	private float _timeBetweenUpdates = 0.5f;

	private float _timeSinceLastUpdate = 0f;

	private int fpsCount = 0;
	private float fpsTime = 0f;

	private float fps = 0f;
	private float mspf = 0f;
	
	private void Update() {
		_timeSinceLastUpdate += Time.deltaTime;
		if (_timeSinceLastUpdate >= _timeBetweenUpdates) {
			_timeSinceLastUpdate = 0f;
			fps = fpsCount / fpsTime;
			mspf = fpsTime / fpsCount * 1000f;
			fpsCount = 0;
			fpsTime = 0f;
		}
		else {
			fpsCount++;
			fpsTime += Time.deltaTime;
		}
	}

	private void OnGUI() {
		GUILayout.Label("FPS: " + (fps));
		GUILayout.Label("MS: " + (mspf) + "ms");
	}
}
