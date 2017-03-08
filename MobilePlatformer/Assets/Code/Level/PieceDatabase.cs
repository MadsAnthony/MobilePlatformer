using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PieceDatabase", menuName = "Level/New PieceDatabase", order = 1)]
public class PieceDatabase : ScriptableObject {
	public List<PieceData> pieces = new List<PieceData>();
}

public enum PieceType {
	// Blocks
	BlockNormal,
	BlockColor,
	BlockNonSticky,
	Spike,

	// Other
	Hero
};

[Serializable]
public class PieceData {
	public PieceType type;
	public Piece prefab;

	public PieceData(PieceType type) {
		this.type = type;
	}
}
