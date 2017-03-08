using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColor : Piece {

	public override void Hit (Piece hitPiece)
	{
		if (hitPiece.Type == PieceType.Hero) {
			if (GetComponentInChildren<SpriteRenderer> ().color != Color.green) {
				GetComponentInChildren<SpriteRenderer> ().color = Color.green;
				Director.GameEventManager.Emit (GameEventType.BlockColored);
			}
		}
	}
}
