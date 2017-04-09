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
		if (noGravityT < 1) {
			noGravityT += 0.025f;
		}
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
					if (gravity<=0) {
					// When ground is hit
					if (gravity<=-(maxGravity)) {
						Director.Sounds.breakSound.Play ();
						Director.CameraShake();
					}

					gravity = 0;
					IsOnGround = true;

					// If all ground blocks are non sticky, then fall down.
					if (AllPiece(ps, (Piece p) => {return p.Type==PieceType.Block && !((Block)p).IsSticky(tmpMoveDir);}) && dirsIndex%4 != 0) {
						movingDir = dirsIndex%4 != 2? 0 : movingDir*-1;
						ChangeGravity(-dirsIndex);
						}
					} else {
					// When ceil is hit

					// If ceiling is block then stop any jump.
					if (ExistPiece(ps, (Piece p) => {return p.Type==PieceType.Block;})) {
						gravity = 1;
						EndJump();
					}
					// If one ceil block are sticky, then stick to that.
					if (ExistPiece(ps, (Piece p) => {return p.Type==PieceType.Block && ((Block)p).IsSticky(tmpMoveDir);})) {
						IsOnGround = true;
						gravity = 0;
						movingDir *= -1;
						ChangeGravity(2);
					}
				}
				},
			() => {
				// if falling, then check if it is possible to stick to nearby wall.
				IsOnGround = false;
				Check(dir*speed*-movingDir, (Piece[] ps, bool b) => {
					if (gravity<0f) {
						if (ExistPiece(ps, (Piece p) => { return p.Type==PieceType.Block && ((Block)p).IsSticky(dir*speed*-movingDir);})) {
							ChangeGravity(movingDir);
							}
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
	void TouchInput() {
		if (Input.GetMouseButtonDown(0) && touchConsumed) {
			touchConsumed = false;
			mouseClickPos = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp (0)) {
			EndJump ();
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
				touchConsumed = true;
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

	void EndJump() {
		noGravityT = 1;
	}

	public void SmallJump(float jumpForce = 12) {
		gravity = jumpForce;
	}

	bool stopMoving = false;
	public void StopMoving() {
		stopMoving = true;
		movingDir = 0;
	}

	public void SetGravity(Direction dir) {
		if (dir == Direction.Up) {
			dirsIndex = 2;
		}
		if (dir == Direction.Left) {
			dirsIndex = 3;
		}
		if (dir == Direction.Right) {
			dirsIndex = 1;
		}
		if (dir == Direction.Down) {
			dirsIndex = 0;
		}
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
		if (hitPiece.Type == PieceType.Enemy1) {
			Destroy ();
		}
	}
}
