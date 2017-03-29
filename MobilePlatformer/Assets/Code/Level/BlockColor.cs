using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColor : Piece {

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Hero) {
			if (GetComponentInChildren<SpriteRenderer> ().color != Color.green) {
				GetComponentInChildren<SpriteRenderer> ().color = Color.green;
				Director.GameEventManager.Emit (GameEventType.BlockColored);
			}
		}
	}
}
