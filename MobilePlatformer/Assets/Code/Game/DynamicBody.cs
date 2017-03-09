using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DynamicBody : Piece {

	private float gap = 0.01f;
	private Rigidbody rb;
	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody>();
	}

	protected void Move(Vector3 dir, Action<string> callbackInterrupted = null, Action callbackFinished = null) {
		Vector3 inputDir = dir * Time.deltaTime;
		Vector3 newDir = inputDir;
		bool newDirHasBeenSet = false;

		int i = 0;
		Vector3 tmpDir;
		var hits = rb.SweepTestAll (inputDir, inputDir.magnitude);
		foreach(var hit in hits) {
			var piece = hit.collider.GetComponent<Piece> ();
			tmpDir = inputDir.normalized * (hit.distance - gap);

			// check if the rest of the hits are approximately at the same distance, if not then check the next of the rest.
			if (i >0 && newDirHasBeenSet && !newDir.Equals (tmpDir)) continue;

			piece.Hit(this);
			if (!piece.IsPassable) {
				newDirHasBeenSet = true;
				newDir = tmpDir;

				// only call callbackInterrupted on the first hit
				if (i == 0 && callbackInterrupted != null) {
					callbackInterrupted (hit.collider.name);
				}
			}
			i++;
		}
		this.transform.position += newDir;
		if (callbackFinished != null) {
			callbackFinished ();
		}
	}

	protected void Check(Vector3 dir, Action callbackInterrupted = null, Action callbackFinished = null) {
		Vector3 inputDir = dir * Time.deltaTime;
		Vector3 newDir = inputDir;
		RaycastHit hit;

		if (rb.SweepTest (inputDir, out hit, inputDir.magnitude)) {
			if (hit.collider.name.Contains ("Block")) {
				newDir = inputDir.normalized * (hit.distance - gap);

				if (callbackInterrupted != null) {
					callbackInterrupted ();
				}
			}
		}
		if (callbackFinished != null) {
			callbackFinished ();
		}
	}
}
