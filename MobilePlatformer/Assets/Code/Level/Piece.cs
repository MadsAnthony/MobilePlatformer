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
	[HideInInspector]
	public bool IsPushable;

	public abstract void Hit (Piece hitPiece);

	public void Destroy() {
		GameObject.Destroy (gameObject);
	}
}
