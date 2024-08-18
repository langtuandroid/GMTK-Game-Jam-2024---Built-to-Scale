using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class SceepleScript : MonoBehaviour {

	public SceepleSpawnerScript.Sceeple stats;
	public SplineFollower splineFollower;

	void Start() {
		splineFollower.follow = true;
	}

	// Update is called once per frame
	void Update() {

	}
}
