using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Piece {
	float gravity;
	float speed = 7;
	public float maxGravity = 50;
	Vector3 dir;

	Vector3[] dirs = new Vector3[4]{new Vector3(-1,0,0),
									 new Vector3(0,-1,0),
									 new Vector3(1,0,0),
									 new Vector3(0,1,0)
									};
	int dirsIndex;

	int movingDir = -1;
	bool isOnGround;

	public float Gravity {get { return gravity;}}
	// Use this for initialization
	void Start () {
		dir = dirs[dirsIndex];
	}

	float noGravityT = 1;
	// Update is called once per frame
	void Update () {
		noGravityT = Mathf.Clamp (noGravityT, 0, 1);

		gravity -= 1f*noGravityT;

		if (!stopMoving) {
			TouchInput ();
			KeyboardInput ();
		}
		Move(dir*speed*movingDir,(Piece[] ps, bool b) => { if (ExistPiece(ps, (Piece p) => { return p.Type==PieceType.Block && ((Block)p).IsSticky(dir*speed*movingDir);})) {ChangeGravity(1*-movingDir);}});

		Vector3 gravityDir = new Vector3 (dir.y,-dir.x,0);

		Vector3 tmpMoveDir = gravityDir * Mathf.Clamp (gravity, -maxGravity, maxGravity);
		Move(tmpMoveDir,
			(Piece[] ps, bool b) => {
					if (gravity<=-(maxGravity)) {
						Director.Sounds.breakSound.Play ();
						Director.CameraShake();
					}
					if (gravity<=0) {
						gravity = 0;
						IsOnGround = true;
					if (AllPiece(ps, (Piece p) => {return p.Type==PieceType.Block && !((Block)p).IsSticky(tmpMoveDir);}) && dirsIndex%4 != 0) {
							movingDir = dirsIndex%4 != 2? 0 : movingDir*-1;
							ChangeGravity(-dirsIndex);
						}
					} else {
					if (ExistPiece(ps, (Piece p) => {return p.Type==PieceType.Block && ((Block)p).IsSticky(tmpMoveDir);})) {
						IsOnGround = true;
						gravity = 0;
						movingDir *= -1;
						ChangeGravity(2);
					}
				}
				},
			() => {
				IsOnGround = false;
				Check(dir*speed*-movingDir,
						() => {
								if (gravity<0f) {
										ChangeGravity(movingDir);
									}
								});
				});
	}

	public bool IsOnGround { 
		get { 
			return isOnGround;
		} 
		set {
			if (value == true) {
				CancelInvoke("IsNotOnGround");
				isOnGround = value;
			} else {
				Invoke("IsNotOnGround",0.1f);
			}
		}
	}

	void IsNotOnGround() {
		isOnGround = false;
	}

	public bool ExistPiece(Piece[] pieces, Predicate<Piece> condition) {
		foreach (Piece piece in pieces) {
			if (condition(piece)) return true;
		}
		return false;
	}

	public bool AllPiece(Piece[] pieces, Predicate<Piece> condition) {
		foreach (Piece piece in pieces) {
			if (!condition(piece)) return false;
		}
		return true;
	}

	void KeyboardInput() {
		if (Input.GetKeyDown (KeyCode.UpArrow) && IsOnGround && gravity<=0) {
			Jump ();
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			gravity = -maxGravity;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			noGravityT += 0.025f;
		}
		if (Input.GetKeyUp (KeyCode.UpArrow)) {
			noGravityT = 1;
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
	void TouchInput() {
		if (Input.GetMouseButtonDown(0) && touchConsumed) {
			touchConsumed = false;
			mouseClickPos = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp (0)) {
			noGravityT = 1;
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

			if (verticalDotProduct < -threshold  && IsOnGround && !touchConsumed && noGravityT>=1 && gravity<=0) {
				Jump ();
			}
			if (verticalDotProduct < -threshold && !touchConsumed) {
				noGravityT += 0.025f;

				if (noGravityT >= 1) {
					noGravityT = 1;
					touchConsumed = true;
				}
			}
			if (verticalDotProduct > threshold && !IsOnGround && !touchConsumed) {
				gravity = -maxGravity;
				touchConsumed = true;
			}
		}
	}

	void Jump(float jumpForce = 12) {
		Director.Sounds.jump.Play ();
		gravity = jumpForce;
		noGravityT = 0;
	}

	bool stopMoving = false;
	public void StopMoving() {
		stopMoving = true;
		movingDir = 0;
	}

	void ChangeGravity(int delta) {
		dirsIndex += delta;
		if (dirsIndex < 0) {
			dirsIndex = dirs.Length-1;
		}
		dir = dirs [dirsIndex%dirs.Length];
		gravity = 0;

		// consume all touch inputs - because controls have been changed/rotated.
		touchConsumed = true;
	}

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
	}

	public override void Hit (Piece hitPiece, Vector3 direction) {
	}
}
