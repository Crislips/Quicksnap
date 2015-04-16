using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetCamera : MonoBehaviour {
	public bool editMode = true;
	public GameObject fpCamera; // First-person Camera

	public bool ________________;

	public int shotNum;
	public GUIText shotCounter, shotRating;
	public GUITexture checkMark;
	void Start() {
		// Find the GUI components
		GameObject go = GameObject.Find("ShotCounter");
		shotCounter = go.GetComponent<GUIText>();
		go = GameObject.Find("ShotRating");
		shotRating = go.GetComponent<GUIText>();
		go = GameObject.Find("_Check_64");
		checkMark = go.GetComponent<GUITexture>();
		// Hide the checkMark
		checkMark.enabled = false;
		// Load all the shots from PlayerPrefs
		Shot.LoadShots();
		// If there were shots stored in PlayerPrefs
		if (Shot.shots.Count>0) {
			shotNum = 0;
			ShowShot(Shot.shots[shotNum]);
		}
		// Hide the cursor (Note: this doesn't work in the Unity Editor unless
		// the Game pane is set to Maximize on Play.)
		Screen.showCursor = false;
	}

	void Update () {
		Shot sh;
		// Mouse Input
		if (Input.GetMouseButtonDown(0)) { // Left mouse button
			sh = new Shot();
			// Grab the position and rotation of fpCamera
			sh.position = fpCamera.transform.position;
			sh.rotation = fpCamera.transform.rotation;
			// Shoot a ray from the camera and see what it hits
			Ray ray = new Ray(sh.position, fpCamera.transform.forward);
			RaycastHit hit;
			if ( Physics.Raycast(ray, out hit) ) {
				sh.target = hit.point;
			}
			// Position _TargetCamera with the Shot
			ShowShot(sh);

			Utils.tr( sh.ToXML() );

			// Record a new shot
			Shot.shots.Add(sh);
			shotNum = Shot.shots.Count-1;

		}

		// Keyboard Input
		// Use Q and E to cycle Shots
		// Note: Either of these will throw an error if Shot.shots is empty.
		if (Input.GetKeyDown(KeyCode.Q)) {
			shotNum--;
			if (shotNum < 0) shotNum = Shot.shots.Count-1;
			ShowShot(Shot.shots[shotNum]);
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			shotNum++;
			if (shotNum >= Shot.shots.Count) shotNum = 0;
			ShowShot(Shot.shots[shotNum]);
		}
		// If in editMode & Left Shift is held down...
		if (editMode && Input.GetKey(KeyCode.LeftShift)) {
			// Use Shift-S to Save
			if (Input.GetKeyDown(KeyCode.S)) {
				Shot.SaveShots();
			}
			// Use Shift-X to output XML to Console
			if (Input.GetKeyDown(KeyCode.X)) {
				Utils.tr(Shot.XML);
			}
		}

		// Update the GUITexts
		shotCounter.text = (shotNum+1).ToString()+" of "+Shot.shots.Count;
		if (Shot.shots.Count == 0) shotCounter.text = "No shots exist";
		// ^ Shot.shots.Count doesn't require .ToString() because it is assumed
		// when the left side of the + operator is a string
		shotRating.text = ""; // This line will be replaced later
	}
	public void ShowShot(Shot sh) {
		// Position _TargetCamera with the Shot
		transform.position = sh.position;
		transform.rotation = sh.rotation;
	}
}