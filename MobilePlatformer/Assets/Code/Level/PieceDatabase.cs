using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PieceDatabase", menuName = "Level/New PieceDatabase", order = 1)]
public class PieceDatabase : ScriptableObject {
	public List<PieceData> pieces = new List<PieceData>();

	public PieceData GetPieceData(PieceType type) {
		foreach(PieceData pieceData in pieces) {
			if (pieceData.type == type) {
				return pieceData;
			}
		}
		return null;
	}
}

// ATTENTION: Always add new entries at the end and be careful when removing entries (as enums are serialized to integers).
public enum PieceType {
	PieceType1,
	PieceType2,
	Block,
	Spike,
	Hero,
	Collectable,
	BlockDestructible,
	Ball
};

[Serializable]
public class PieceData {
	public PieceType type;
	public Piece prefab;

	public PieceData(PieceType type) {
		this.type = type;
	}
}
