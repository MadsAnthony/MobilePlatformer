using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBody : MonoBehaviour {

	private float gap = 0.01f;
	private Rigidbody rb;
	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected void Move(Vector3 dir, Action callbackInterrupted = null, Action callbackFinished = null) {
		Vector3 inputDir = dir * Time.deltaTime;
		Vector3 newDir = inputDir;
		RaycastHit hit;

		// TODO - Move this to another place - should not be done here.
		var hits = rb.SweepTestAll (inputDir, inputDir.magnitude);
		foreach(var ahit in hits) {
			if (ahit.collider.name.Contains ("Color")) {
				ahit.collider.gameObject.GetComponentInChildren<SpriteRenderer> ().color = Color.green;
			}
		}


		if (rb.SweepTest (inputDir, out hit, inputDir.magnitude)) {
			if (hit.collider.name.Contains ("Block")) {
				newDir = inputDir.normalized * (hit.distance - gap);
				if (callbackInterrupted != null) {
					callbackInterrupted ();
				}
			}
			if (hit.collider.name.Contains ("Spike")) {
				newDir = inputDir.normalized * (hit.distance - gap);
			}
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
