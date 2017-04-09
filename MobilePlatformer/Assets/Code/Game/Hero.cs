using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : DynamicBody {
	protected override void OnUpdate() {
		if (!stopMoving) {
			TouchInput ();
			KeyboardInput ();
		}
	}

	void KeyboardInput() {
		if (Input.GetKeyDown (KeyCode.UpArrow) && IsOnGround && gravity<=0) {
			Jump ();
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			gravity = -maxGravity;
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			Jump ();
		}
		if (Input.GetKeyUp (KeyCode.UpArrow)) {
			EndJump ();
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			movingDir = -1;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			movingDir = 1;
		}
	}

	Vector3 mouseClickPos;
	float threshold = Mathf.Cos(Mathf.PI*0.25f);
	bool touchConsumed = true;
	bool upMovementConsumed = false;
	void TouchInput() {
		if (Input.GetMouseButtonDown(0) && touchConsumed) {
			touchConsumed = false;
			mouseClickPos = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp (0)) {
			EndJump ();
			touchConsumed = true;
			upMovementConsumed = false;
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

			if (verticalDotProduct < -threshold  && IsOnGround && !touchConsumed && !upMovementConsumed && noGravityT>=1 && gravity<=0) {
				Jump ();
				upMovementConsumed = true;
			}
			if (verticalDotProduct > threshold && !IsOnGround && !touchConsumed) {
				gravity = -maxGravity;
				touchConsumed = true;
			}
		}
	}

	protected override void ChangeGravity(int delta) {
		base.ChangeGravity(delta);
		// consume all touch inputs - because controls have been changed/rotated.
		touchConsumed = true;
	}

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
		SetGravity (pieceLevelData.dir);
	}

	public override void Hit (Piece hitPiece, Vector3 direction) {
		if (hitPiece.Type == PieceType.Enemy1) {
			Destroy ();
		}
	}
}
