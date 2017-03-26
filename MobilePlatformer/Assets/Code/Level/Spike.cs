﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Piece {

	public override void Init (PieceLevelData pieceLevelData) {
	}

	public override void Hit (Piece hitPiece)
	{
		if (hitPiece.Type == PieceType.Hero) {
			hitPiece.Destroy();
		}
	}

}
