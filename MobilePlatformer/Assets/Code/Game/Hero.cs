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

		Move(gravityDir*Mathf.Clamp(gravity,-maxGravity,maxGravity),
			() => {
					if (gravity<-(maxGravity*0.5f)) {
						Director.CameraShake();
					}
					gravity = 0; 
					isOnGround = true;
				},
			() => {
				Check(dir*speed*-movingDir,
						() => {
								if (gravity<0f) {
										ChangeGravity(movingDir);
									}
								});
				});
	}

	Vector3 mouseClickPos;
	float threshold = 0.4f;
	bool touchConsumed = true;
	void TouchInput() {
		if (Input.GetMouseButtonDown(0) && touchConsumed) {
			touchConsumed = false;
			mouseClickPos = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp (0)) {
			touchConsumed = true;
		}
		if (Input.GetMouseButton(0)) {
			Vector3 mouseDir = (mouseClickPos - Input.mousePosition);
			var horizontalDotProduct = Vector3.Dot (mouseDir.normalized, dir);
			var verticalDotProduct = Vector3.Dot (mouseDir.normalized, new Vector3(dir.y, -dir.x,dir.z));
			var verticalDotProductForce = Vector3.Dot (mouseDir, new Vector3(dir.y, -dir.x,dir.z));

			if (Mathf.Abs (horizontalDotProduct) > threshold && !touchConsumed) {
				int newMovingDir = -1 * (int)Mathf.Sign (horizontalDotProduct);
				touchConsumed = newMovingDir != movingDir;

				movingDir = newMovingDir;
			}

			if (verticalDotProduct < -threshold  && isOnGround && !touchConsumed) {
				Jump (Mathf.Clamp(-verticalDotProductForce,12,20));
				touchConsumed = true;
			}
			if (verticalDotProduct > threshold && isOnGround && !touchConsumed) {
				movingDir = 0;
				touchConsumed = true;
			}
			if (verticalDotProduct > threshold && !isOnGround && !touchConsumed) {
				gravity = -Mathf.Abs(verticalDotProductForce);
				touchConsumed = true;
			}
		}
	}

	void Jump(float jumpForce = 16) {
		gravity = jumpForce;
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
