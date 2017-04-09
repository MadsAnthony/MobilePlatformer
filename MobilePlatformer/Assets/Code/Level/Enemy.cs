using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Piece {
	float gravity;
	void Update () {
		gravity -= 1f;
		gravity = Mathf.Clamp (gravity,-10,10);
		/*Check(dir*speed*-movingDir,
			() => {
				if (gravity<0f) {
					ChangeGravity(movingDir);
				}
			});*/
		Move (new Vector3(3,0));
		Move (new Vector3 (0, gravity, 0));
	}

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
	}

}
