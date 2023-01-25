using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Replace this whole class, create a proper gravity system

public class Player : MonoBehaviour {
	public float speed = 6f;
	public float sprintSpeed = 20f;
	
	public float mouseSensitivity = 10f;
	
	private Vector3 _movement;
	
	[SerializeField]
	private Transform _cameraTransform;

	private void Awake() {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update() {
		if(Input.GetMouseButton(0)) {
			MouseClick(true);
		}
		if(Input.GetMouseButton(1)) {
			MouseClick(false);
		}
		
		// Get input
		float x = Input.GetAxisRaw("Horizontal");
		float z = Input.GetAxisRaw("Vertical");
		
		// Set movement vector
		_movement.Set(x, 0f, z);
		if (Input.GetKey(KeyCode.LeftShift)) {
			_movement = _movement.normalized * (sprintSpeed * Time.deltaTime);
		}
		else {
			_movement = _movement.normalized * (speed * Time.deltaTime);
		}

		// Move player
		transform.Translate(_movement, _cameraTransform);
		
		// Rotate player
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
		
		transform.Rotate(Vector3.up * mouseX);
		_cameraTransform.Rotate(Vector3.left * mouseY);
	}

	private void MouseClick(bool add) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 300f, 1 << 3)) {
			if (add) {
				hit.transform.gameObject.GetComponent<MarchingCubes>().PaintAddVoxels(hit);
			}
			else {
				hit.transform.gameObject.GetComponent<MarchingCubes>().PaintRemoveVoxels(hit);
			}
		}
	}

	private void OnGUI() {
		Texture cursorTexture = Texture2D.whiteTexture;
		GUI.DrawTexture(new Rect(new Vector2(Screen.width/2f, Screen.height/2f), new Vector2(cursorTexture.height, cursorTexture.width)), cursorTexture);
	}
}
