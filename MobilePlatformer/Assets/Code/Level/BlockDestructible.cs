using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDestructible : Piece {

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Hero && hitPiece.GetComponent<Hero>().Gravity<=-hitPiece.GetComponent<Hero>().maxGravity) {
			IsPassable = true;
			Destroy (this.gameObject);
		}
	}

}
