using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelAsset))]
public class LevelInspector : Editor {

	private Texture2D cellTexture;
	private Texture2D spikeTexture;
	private Vector2 selectedIndex;

	private Vector2 gridStartPos = new Vector2 (100,200);
	private float cellSize = 20;
	int cellType = 1;
	BlockType blockType;
	public Direction direction;

	public override void OnInspectorGUI()
	{
		cellTexture  = AssetDatabase.LoadAssetAtPath("Assets/Textures/squareWithBorder.png", typeof(Texture2D)) as Texture2D;
		spikeTexture = AssetDatabase.LoadAssetAtPath("Assets/Textures/spike.png", typeof(Texture2D)) as Texture2D;

		LevelAsset myTarget = (LevelAsset)target;
		myTarget.someInt = EditorGUILayout.IntField ("Experience", myTarget.someInt);
		EditorGUILayout.LabelField ("Level", myTarget.levelName);

		if (GUILayout.Button ("Play")) {
			EditorApplication.isPlaying = false;
			EditorSceneManager.OpenScene ("Assets/Scenes/LevelScene.unity");
			var LevelInit = GameObject.Find ("LevelInit").GetComponent<LevelInit> ();
			LevelInit.level = myTarget;
			EditorApplication.isPlaying = true;
		}
		if (GUILayout.Button ("Clear")) {
			myTarget.blocks.Clear();
		}

		string[] cellOptions = new string[]
		{
			"Hero", "Blocks"
		};

		cellType = EditorGUILayout.Popup("Cell Type", (int)cellType, cellOptions);

		if (cellType == 1) {
			string[] blockOptions = Enum.GetNames (typeof(BlockType));
			blockType = (BlockType)EditorGUILayout.Popup ("Block Type", (int)blockType, blockOptions);
		}

		direction = (Direction)EditorGUILayout.Popup("Direction", (int)direction, Enum.GetNames (typeof(Direction)));

		if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) {
			GetClosestCell (Event.current.mousePosition);
		}
		if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) {
			if (Event.current.button == 0) {
				if (cellType == 0) {
					myTarget.heroPos = selectedIndex;
				}
				if (cellType == 1) {
					BlockObject existingBlock = GetBlockWithPos (selectedIndex);
					if (existingBlock != null) myTarget.blocks.Remove (existingBlock);
					myTarget.blocks.Add (new BlockObject (blockType, selectedIndex, direction));
				}
			}
			if (Event.current.button == 1) {
				BlockObject existingBlock = GetBlockWithPos (selectedIndex);
				if (existingBlock != null) myTarget.blocks.Remove(existingBlock);
			}
			EditorUtility.SetDirty (myTarget);
		}

		DrawGrid ();

		if (GUI.changed) {
			EditorUtility.SetDirty (myTarget);
		}
	}

	BlockObject GetBlockWithPos(Vector2 pos) {
		foreach(BlockObject block in ((LevelAsset)target).blocks) {
			if (pos == block.pos) {
				return block;
			}
		}
		return null;
	}

	Vector2 GetClosestCell(Vector2 mousePos) {
		mousePos -= gridStartPos;
		selectedIndex = new Vector2(Mathf.FloorToInt(mousePos.x/cellSize),Mathf.FloorToInt(mousePos.y/cellSize));
		return selectedIndex;
	}

	void DrawGrid() {
		for (int x = 0; x<Level.GRIDSIZE.x;x++) {
			for (int y = 0; y<Level.GRIDSIZE.y;y++) {
				var rect = new Rect (gridStartPos.x+x*cellSize, gridStartPos.y+y*cellSize, cellSize, cellSize);
				GUI.color = Color.white;

				Texture2D tmpTexture = cellTexture;
				foreach(BlockObject block in ((LevelAsset)target).blocks) {
					if (x == block.pos.x && y == block.pos.y) {
						if (block.type == BlockType.Normal) {
							GUI.color = new Color(0.2f,0.2f,0.2f,1);
							break;
						}
						if (block.type == BlockType.Color) {
							GUI.color = Color.grey;
							break;
						}
						if (block.type == BlockType.Spike) {
							GUI.color = Color.red;
							tmpTexture = spikeTexture;
							break;
						}
						if (block.type == BlockType.NonSticky) {
							GUI.color = Color.black;
							break;
						}
					}
				}

				if (x ==  ((LevelAsset)target).heroPos.x && y == ((LevelAsset)target).heroPos.y) {
					GUI.color = Color.green;
				}
				GUI.DrawTexture(rect,tmpTexture,ScaleMode.ScaleToFit);
			}
		}
	}
}
#endif