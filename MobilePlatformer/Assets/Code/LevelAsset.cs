using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Level", menuName = "Level/New level", order = 1)]
public class LevelAsset :ScriptableObject {
	public string levelName;

	public int someInt;
}
