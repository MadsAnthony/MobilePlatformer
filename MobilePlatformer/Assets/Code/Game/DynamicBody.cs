using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class DynamicBody : Piece {

	private float gap = 0.01f;
	private Rigidbody rb;
	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody>();
	}
		
	protected Vector3 Move(Vector3 dir, Action<string> callbackInterrupted = null, Action callbackFinished = null, bool useDeltaTime = true) {
		Vector3 inputDir = dir * (useDeltaTime? Time.deltaTime : 1);
		Vector3 newDir = inputDir;
		bool newDirHasBeenSet = false;

		int i = 0;
		Vector3 tmpDir;
		var hits = rb.SweepTestAll (inputDir, inputDir.magnitude);
		List<RaycastHit> sortedHits = hits.ToList ();
		sortedHits.Sort ((x, y) => { 
			if(x.distance == y.distance) {
				return 0;
			} else if (x.distance < y.distance){
				return -1;
			}else {
				return 1;
			};							
		});
		foreach(var hit in sortedHits) {
			var piece = hit.collider.GetComponent<Piece> ();
			tmpDir = inputDir.normalized * (hit.distance - gap);

			// check if the rest of the hits are approximately at the same distance, if they are then they should be hit.
			if (i >0 && newDirHasBeenSet && !newDir.Equals (tmpDir)) continue;

			// if pushable has set newDir, do not hit pieces that are farther away (but do hit pieces before).
			if (i > 0 && tmpDir.magnitude > newDir.magnitude) continue;

			piece.Hit(this);
			if (!piece.IsPassable && !piece.IsPushable) {
				newDirHasBeenSet = true;
				newDir = tmpDir;

				// only call callbackInterrupted on the first hit
				if (i == 0 && callbackInterrupted != null) {
					callbackInterrupted (hit.collider.name);
				}
			}
			if (piece.IsPushable) {
				if ((DynamicBody)piece != null) {
					newDir = tmpDir+((DynamicBody)piece).Move ((inputDir-tmpDir),null,null,false);
					if (i == 0 && callbackInterrupted != null) {
						callbackInterrupted (hit.collider.name);
					}
				}
			}
			i++;
		}
		this.transform.position += newDir;
		if (callbackFinished != null) {
			callbackFinished ();
		}
		return newDir;
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
