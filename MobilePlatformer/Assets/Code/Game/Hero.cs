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

		TouchInput ();
		if (Input.GetKeyDown (KeyCode.UpArrow) && isOnGround) {
			Jump ();
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			movingDir = -1;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			movingDir = 1;
		}

		Move(dir*speed*movingDir,() => { if (true || isOnGround) {ChangeGravity(1*-movingDir);} else {movingDir = 0;}});

		Vector3 gravityDir = new Vector3 (dir.y,-dir.x,0);

		isOnGround = false;
		Move(gravityDir*Mathf.Clamp(gravity,-maxGravity,maxGravity),() => {gravity = 0; isOnGround = true;},() => {Check(dir*speed*-movingDir,() => { if (gravity<0f) {ChangeGravity(movingDir);}});});
	}

	Vector3 mouseClickPos;
	float threshold = 0.4f;
	void TouchInput() {
		if (Input.GetMouseButtonDown(0)) {
			mouseClickPos = Input.mousePosition;
		}
		if (Input.GetMouseButton(0)) {
			Vector3 mouseDir = (mouseClickPos - Input.mousePosition).normalized;
			var horizontalDotProduct = Vector3.Dot (mouseDir, dir);
			var verticalDotProduct = Vector3.Dot (mouseDir, new Vector3(dir.y, -dir.x,dir.z));

			if (Mathf.Abs (horizontalDotProduct) > threshold) {
				movingDir = -1*(int)Mathf.Sign(horizontalDotProduct);
			}

			if (verticalDotProduct < -threshold  && isOnGround) {
				Jump ();
			}
			if (verticalDotProduct > threshold && isOnGround) {
				movingDir = 0;
			}
		}
	}

	void Jump() {
		gravity = 16f;
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
