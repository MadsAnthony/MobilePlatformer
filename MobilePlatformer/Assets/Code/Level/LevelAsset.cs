using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level", menuName = "Level/New Level", order = 1)]
public class LevelAsset : ScriptableObject {
	public List<BlockObject> blocks = new List<BlockObject>();
	public Vector2 heroPos;

	public string levelName;

	public int someInt;
}

public enum BlockType {Normal,Color,Spike};

[Serializable]
public class BlockObject {
	public BlockType type;
	public Vector2 pos;

	public BlockObject(BlockType type, Vector2 pos) {
		this.type = type;
		this.pos  = pos;
	}
}
