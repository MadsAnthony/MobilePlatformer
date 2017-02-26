using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : DynamicBody {
	float gravity;
	float speed = 8;
	float maxGravity = 50;
	Vector3 dir;

	Vector3[] dirs = new Vector3[4]{new Vector3(-1,0,0),
									 new Vector3(0,-1,0),
									 new Vector3(1,0,0),
									 new Vector3(0,1,0)
									};
	int dirsIndex;

	int movingDir = -1;
	bool isOnGround;

	// Use this for initialization
	void Start () {
		dir = dirs[dirsIndex];
	}

	// Update is called once per frame
	void Update () {
		gravity -= 1f;
		if (Input.GetKeyDown (KeyCode.UpArrow) && isOnGround) {
			gravity = 20f;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			movingDir = -1;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			movingDir = 1;
		}

		Move(dir*speed*movingDir,() => { if (isOnGround) {ChangeGravity(1*-movingDir);}});

		Vector3 gravityDir = new Vector3 (dir.y,-dir.x,0);

		isOnGround = false;
		Move(gravityDir*Mathf.Clamp(gravity,-maxGravity,maxGravity),() => {gravity = 0; isOnGround = true;},() => {Check(dir*speed*-movingDir,() => { if (gravity<0f) {ChangeGravity(movingDir);}});});
	}

	void CheckSideForGround() {
		
	}

	void ChangeGravity(int delta) {
		dirsIndex += delta;
		if (dirsIndex < 0) {
			dirsIndex = dirs.Length-1;
		}
		dir = dirs [dirsIndex%dirs.Length];
		gravity = 0;
	}
}
