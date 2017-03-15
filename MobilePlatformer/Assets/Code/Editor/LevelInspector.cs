using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rotorz.ReorderableList;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelAsset))]
public class LevelInspector : Editor {

	private Texture2D cellTexture;
	private Texture2D spikeTexture;
	private Texture2D blockDestructibleTexture;
	private Vector2 selectedIndex;

	private Rect levelGridRect = new Rect(100, 200, 420, 620);
	private float cellSize = 20;
	int cellType = 1;
	PieceType pieceType;
	public Direction direction;
	Vector2 scrollPos;
	Vector2 mouseDownPos;


	public override void OnInspectorGUI()
	{
		cellTexture  = AssetDatabase.LoadAssetAtPath("Assets/Textures/squareWithBorder.png", typeof(Texture2D)) as Texture2D;
		spikeTexture = AssetDatabase.LoadAssetAtPath("Assets/Textures/spike.png", typeof(Texture2D)) as Texture2D;
		blockDestructibleTexture = AssetDatabase.LoadAssetAtPath("Assets/Textures/squareDestructible.png", typeof(Texture2D)) as Texture2D;

		LevelAsset myTarget = (LevelAsset)target;

		if (GUILayout.Button ("Play")) {
			EditorApplication.isPlaying = false;
			EditorSceneManager.OpenScene ("Assets/Scenes/LevelScene.unity");
			var LevelInit = GameObject.Find ("LevelInit").GetComponent<LevelInit> ();
			LevelInit.level = myTarget;
			EditorApplication.isPlaying = true;
		}
		if (GUILayout.Button ("Clear")) {
			myTarget.pieces.Clear();
		}

		string[] cellOptions = new string[]
		{
			"Hero", "Blocks"
		};

		var levelSizeX = EditorGUILayout.IntSlider((int)myTarget.levelSize.x,0,50);
		var levelSizeY = EditorGUILayout.IntSlider((int)myTarget.levelSize.y,0,50);

		myTarget.levelSize = new Vector2 (levelSizeX, levelSizeY);
		cellType = EditorGUILayout.Popup("Cell Type", (int)cellType, cellOptions);

		EditorGUILayout.BeginHorizontal();
		if (cellType == 1) {
			string[] pieceOptions = Enum.GetNames (typeof(PieceType));
			pieceType = (PieceType)EditorGUILayout.Popup ("Piece Type", (int)pieceType, pieceOptions);
		}

		direction = (Direction)EditorGUILayout.Popup("Direction", (int)direction, Enum.GetNames (typeof(Direction)));
		EditorGUILayout.EndHorizontal();

		if (Event.current.type == EventType.MouseDown) {
			mouseDownPos = Event.current.mousePosition;
		}

		if (Event.current.type == EventType.MouseUp && mouseDownPos.x != 0 && mouseDownPos.y != 0) {
			mouseDownPos = Vector2.zero;
		}

		if (IsPositionWithinGrid(mouseDownPos) && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)) {
			GetClosestCell (Event.current.mousePosition);

			if (selectedPieceGroup == null) {
				if (Event.current.button == 0) {
					if (cellType == 0) {
						myTarget.heroPos = selectedIndex;
					}
					if (cellType == 1) {
						PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
						if (existingPiece != null)
							myTarget.pieces.Remove (existingPiece);
						myTarget.pieces.Add (new PieceLevelData (pieceType, selectedIndex, direction));
					}
				}
				if (Event.current.button == 1) {
					PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
					if (existingPiece != null)
						myTarget.pieces.Remove (existingPiece);
				}
			} else {
				if (Event.current.button == 0) {
					PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
					if (existingPiece != null) {
						if (!selectedPieceGroup.pieceIds.Contains (existingPiece.id)) {
							selectedPieceGroup.pieceIds.Add (existingPiece.id);
						}
					}
				}
				if (Event.current.button == 1) {
					PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
					if (existingPiece != null) {
						selectedPieceGroup.pieceIds.Remove (existingPiece.id);
					}
				}
			}
			EditorUtility.SetDirty (myTarget);
		}

		GUILayout.Space (levelGridRect.height+50);
		GUILayout.BeginArea(levelGridRect);
		var levelSize = ((LevelAsset)target).levelSize;
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos,GUILayout.Width(Mathf.Min(levelGridRect.width,levelSize.x*(cellSize+1))),GUILayout.Height(Mathf.Min(levelGridRect.height,levelSize.y*(cellSize+1))));
		DrawGrid ();
		EditorGUILayout.EndScrollView ();
		GUILayout.EndArea();
		ReorderableListGUI.Title("Groups");
		ReorderableListGUI.ListField<PieceGroup>(myTarget.pieceGroups, PieceGroupDrawer);


		if (GUI.changed) {
			EditorUtility.SetDirty (myTarget);
		}
		serializedObject.ApplyModifiedProperties();
	}

	PieceGroup selectedPieceGroup = null;
	PieceGroup PieceGroupDrawer(Rect rect, PieceGroup value) {
		var r = new Rect (rect);
		if (value != null) {
			r.width = 20;
			bool isOn = EditorGUI.Toggle (r, selectedPieceGroup == value);
			if (isOn) {
				selectedPieceGroup = value;
			} else {
				if (selectedPieceGroup == value) {
					selectedPieceGroup = null;
				}
			}
			string[] cellOptions = new string[]
			{
				"Select pieces to group", "Select anchor piece", "Select move to"
			};
			r.x += 40;
			r.width = 50;
			EditorGUI.Popup (r, 0, cellOptions);
		}
		return value;
	}

	PieceLevelData GetPieceWithPos(Vector2 pos) {
		foreach(PieceLevelData piece in ((LevelAsset)target).pieces) {
			if (piece.pos == pos) {
				return piece;
			}
		}
		return null;
	}

	bool IsPositionWithinGrid(Vector2 pos) {
		return pos.x > levelGridRect.x && pos.x < levelGridRect.x + levelGridRect.width &&
			   pos.y > levelGridRect.y && pos.y < levelGridRect.y + levelGridRect.height;
	}

	Vector2 GetClosestCell(Vector2 mousePos) {
		if (IsPositionWithinGrid(mousePos)) {
			mousePos -= new Vector2(levelGridRect.x,levelGridRect.y)-scrollPos;
			selectedIndex = new Vector2(Mathf.FloorToInt(mousePos.x/cellSize),Mathf.FloorToInt(mousePos.y/cellSize));
			return selectedIndex;
		} else {
			selectedIndex = new Vector2 (-1, -1);
			return selectedIndex;
		}
	}

	void DrawGrid() {
		var levelSize = ((LevelAsset)target).levelSize;

		GUILayout.Space (levelSize.y*cellSize);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space (levelSize.x*cellSize);
		EditorGUILayout.EndHorizontal();

		for (int x = 0; x<levelSize.x;x++) {
			for (int y = 0; y<levelSize.y;y++) {
				var prevMatrix = GUI.matrix;
				var rect = new Rect (x*cellSize, y*cellSize, cellSize, cellSize);
				GUI.color = Color.white;

				Texture2D tmpTexture = cellTexture;
				foreach(PieceLevelData piece in ((LevelAsset)target).pieces) {
					if (x == piece.pos.x && y == piece.pos.y) {
						if (String.IsNullOrEmpty (piece.id)) {
							//remove this at some point.
							piece.id = Guid.NewGuid ().ToString ();
						}
						if (piece.type == PieceType.BlockNormal) {
							GUI.color = new Color(0.2f,0.2f,0.2f,1);
						}
						if (piece.type == PieceType.BlockColor) {
							GUI.color = Color.grey;
						}
						if (piece.type == PieceType.Spike) {
							GUIUtility.RotateAroundPivot((int)piece.dir*90, rect.center);
							GUI.color = Color.red;
							tmpTexture = spikeTexture;
						}
						if (piece.type == PieceType.BlockNonSticky) {
							GUI.color = Color.black;
						}
						if (piece.type == PieceType.Collectable) {
							GUI.color = Color.yellow;
						}
						if (piece.type == PieceType.BlockDestructible) {
							GUI.color = new Color(0.2f,0.2f,0.2f,1);
							tmpTexture = blockDestructibleTexture;
						}
						if (piece.type == PieceType.BlockMoving) {
							GUI.color = new Color(0.2f,0.2f,0.8f,1);
						}
						if (piece.type == PieceType.Ball) {
							GUI.color = new Color(0.2f,0.8f,0.8f,1);
						}

						if (selectedPieceGroup != null && selectedPieceGroup.pieceIds.Contains(piece.id)) {
							GUI.color = new Color(GUI.color.r+0.5f,GUI.color.g+0.5f,GUI.color.b+0.5f,GUI.color.a);
						}
					}
				}

				if (x ==  ((LevelAsset)target).heroPos.x && y == ((LevelAsset)target).heroPos.y) {
					GUI.color = Color.green;
				}
				GUI.DrawTexture(rect,tmpTexture,ScaleMode.ScaleToFit);
				GUI.matrix = prevMatrix;
			}
		}
		GUI.color = Color.white;
	}
}
#endif