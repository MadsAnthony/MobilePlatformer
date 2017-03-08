using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Piece : MonoBehaviour {
	public PieceType Type  { get; set;}
	public bool IsPassable { get; set;}

	public abstract void Hit (Piece hitPiece);
}
