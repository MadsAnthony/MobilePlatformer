using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Piece : MonoBehaviour {
	[HideInInspector]
	public PieceType Type;
	[HideInInspector]
	public bool IsPassable;

	public abstract void Hit (Piece hitPiece);
}
