using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelAsset))]
public class LevelInspector : Editor {
	public override void OnInspectorGUI()
	{
		LevelAsset myTarget = (LevelAsset)target;
		myTarget.someInt = EditorGUILayout.IntField("Experience", myTarget.someInt);
		EditorGUILayout.LabelField("Level", myTarget.levelName);
	}
}
