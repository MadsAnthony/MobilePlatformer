using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level", menuName = "Level/New level", order = 1)]
public class LevelAsset : ScriptableObject {
	public List<Vector2> gridObjects = new List<Vector2>();

	public Vector2 heroPos;
	public string levelName;

	public int someInt;
}
