using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : DynamicBody {
	float gravity;
	float speed = 8;
	float maxGravity = 50;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		gravity -= 2f;
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			gravity = 30f;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			Move(new Vector3(speed,0,0));
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			Move(new Vector3(-speed,0,0));
		}
		Move(new Vector3(0,Mathf.Clamp(gravity,-maxGravity,maxGravity),0),() => {gravity = 0;});
	}
}
