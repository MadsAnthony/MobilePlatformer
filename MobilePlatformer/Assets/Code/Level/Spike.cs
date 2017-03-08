using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spike : Piece {

	public override void Hit (Piece hitPiece)
	{
		if (hitPiece.Type == PieceType.Hero) {
			SceneManager.LoadScene ("LevelScene");
		}
	}

}
