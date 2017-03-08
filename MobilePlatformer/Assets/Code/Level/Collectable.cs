using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : Piece {

	public override void Hit (Piece hitPiece)
	{
		if (hitPiece.Type == PieceType.Hero) {
			Director.GameEventManager.Emit (GameEventType.CollectableCollected);
			Destroy(this.gameObject);
		}
	}
}
