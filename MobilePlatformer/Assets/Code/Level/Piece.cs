using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Piece : MonoBehaviour {
	[SerializeField][HideInInspector]
	public PieceType Type;
	[SerializeField][HideInInspector]
	public bool IsPassable;

	public abstract void Hit (Piece hitPiece);
}
