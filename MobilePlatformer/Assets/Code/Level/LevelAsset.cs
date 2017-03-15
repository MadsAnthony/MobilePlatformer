using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level", menuName = "Level/New Level", order = 1)]
public class LevelAsset : ScriptableObject {
	public Vector2 levelSize = new Vector2(20,30);
	public List<PieceLevelData> pieces = new List<PieceLevelData>();
	public Vector2 heroPos;

	public string levelName;

}

public enum BlockType {Normal, Color, Spike, NonSticky, Collectable};
public enum Direction {Up, Right, Down, Left};

[Serializable]
public class PieceLevelData {
	public PieceType type;
	public Vector2 pos;
	public Direction dir;
	//public SpecificPieceLevelData specific;

	public PieceLevelData(PieceType type, Vector2 pos, Direction dir) {
		this.type = type;
		this.pos  = pos;
		this.dir  = dir;
	}
}

/*
[Serializable]
public class SpecificPieceLevelData {
	public Vector2 moveDir;
}
*/