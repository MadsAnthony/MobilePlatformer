using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Piece {
	float gravity;
	int movingDir = 1;
	float speed = 3;
	void Update () {
		gravity -= 1f;
		gravity = Mathf.Clamp (gravity,-10,10);

		Check(new Vector3(movingDir*speed,0), (Piece[] ps, bool b) => {
			if (ExistPiece(ps, (Piece p) => { return true;})) {
				movingDir *= -1;
			}
		}
		);

		Move (new Vector3(movingDir*speed,0));
		Move (new Vector3 (0, gravity, 0));
	}

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		float tmpThreshold = 0.1f;

		if (hitPiece.Type == PieceType.Hero) {
			if (direction.y < -tmpThreshold) {
				hitPiece.GetComponent<Hero> ().SmallJump ();
				Destroy (this.gameObject);
			} else {
				hitPiece.Destroy();
			}
		}
	}

}
