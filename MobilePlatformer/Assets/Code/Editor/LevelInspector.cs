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
	private Texture2D blockTexture;
	private Texture2D blockSideTexture;
	private Texture2D blockCornerTexture;
	private Vector2 selectedIndex;

	private Rect levelGridRect = new Rect(100, 200, 420, 620);
	private float cellSize = 20;
	int cellType = 1;
	PieceType pieceType;
	public Direction direction;
	public BlockPieceLevelData.SideType blockSideType;
	Vector2 scrollPos;
	Vector2 mouseDownPos;


	public override void OnInspectorGUI()
	{
		blockTexture  = AssetDatabase.LoadAssetAtPath("Assets/Textures/block.png", typeof(Texture2D)) as Texture2D;
		blockSideTexture  = AssetDatabase.LoadAssetAtPath("Assets/Textures/blockSide.png", typeof(Texture2D)) as Texture2D;
		blockCornerTexture  = AssetDatabase.LoadAssetAtPath("Assets/Textures/blockCorner.png", typeof(Texture2D)) as Texture2D;
		cellTexture  = AssetDatabase.LoadAssetAtPath("Assets/Textures/squareWithBorder.png", typeof(Texture2D)) as Texture2D;
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
			"Hero", "Blocks", "Modify Block"
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

		if (cellType == 2) {
		blockSideType = (BlockPieceLevelData.SideType)EditorGUILayout.Popup("SideType", (int)blockSideType, Enum.GetNames (typeof(BlockPieceLevelData.SideType)));
		}

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
						AddPiece (selectedIndex,pieceType);
					}
					if (cellType == 2) {
						ModifyBlock (selectedIndex, blockSideType);
					}
				}
				if (Event.current.button == 1) {
					if (cellType == 1) {
						RemovePiece (selectedIndex);
					}
					if (cellType == 2) {
						ModifyBlock (selectedIndex,BlockPieceLevelData.SideType.Normal);
					}
				}
			} else {
				if (Event.current.button == 0) {
					PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
					if (existingPiece != null) {
						if (!selectedPieceGroup.pieceIds.Contains (existingPiece.id)) {
							selectedPieceGroup.pieceIds.Add (existingPiece.id);
						}
					}
					if (startPointId != "-1") {
						GetGroupMovementWithId(startPointId).startPoint = selectedIndex;
					}
					if (endPointId != "-1") {
						GetGroupMovementWithId(endPointId).endPoint = selectedIndex;
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
		ReorderableListGUI.ListField<PieceGroupData>(myTarget.pieceGroups, PieceGroupDrawer);


		if (GUI.changed) {
			EditorUtility.SetDirty (myTarget);
		}
		serializedObject.ApplyModifiedProperties();
	}

	void AddPiece(Vector2 index, PieceType pieceType) {
		LevelAsset myTarget = (LevelAsset)target;

		PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
		if (existingPiece != null) {
			myTarget.pieces.Remove (existingPiece);
		}
		PieceLevelData newPiece = new PieceLevelData (pieceType, index, direction);
		myTarget.pieces.Add (newPiece);

		if (pieceType == PieceType.BlockNonSticky) {
			UpdateBlock (index);
		}
		UpdateNeighborBlocks(index);
	}

	void RemovePiece(Vector2 index) {
		LevelAsset myTarget = (LevelAsset)target;

		PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
		if (existingPiece != null) {
			myTarget.pieces.Remove (existingPiece);
		}

		UpdateNeighborBlocks(index);
	}

	void ModifyBlock(Vector2 index, BlockPieceLevelData.SideType sideType) {
		LevelAsset myTarget = (LevelAsset)target;

		PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
		if (existingPiece != null) {
			var specific = existingPiece.GetSpecificData<BlockPieceLevelData> ();
			if (specific.sides[(int)direction] != BlockPieceLevelData.SideType.None) {
				specific.sides [(int)direction] = sideType;
			}
			existingPiece.SaveSpecificData (specific);
		}
	}

	Vector2 GetNeighborIndex(Vector2 index, Direction dir) {
		switch (dir) {
		case Direction.Up:
			return index + (Vector2.up * -1);
		case Direction.Right:
			return index + (Vector2.right);
		case Direction.Down:
			return index + (Vector2.down * -1);
		case Direction.Left:
			return index + (Vector2.left);
		}
		return Vector2.zero;
	}

	void UpdateNeighborBlocks(Vector2 index) {
		// Sides
		for (int i = 0; i < 4; i++) {
			var tmpIndex = GetNeighborIndex (index, (Direction)i);
			if (GetPieceWithPos (tmpIndex) != null && GetPieceWithPos (tmpIndex).type == PieceType.BlockNonSticky) UpdateBlock (tmpIndex);
		}

		//Corners
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				var addIndex = new Vector2 (i - 1, j - 1);
				if (addIndex.x == 0 || addIndex.y == 0) continue;
				var tmpIndex = index + addIndex;
				if (GetPieceWithPos (tmpIndex) != null && GetPieceWithPos (tmpIndex).type == PieceType.BlockNonSticky) UpdateBlock (tmpIndex);
			}
		}
	}

	bool IsPieceOfPieceType(PieceLevelData piece, PieceType type) {
		if (piece == null) return false;
		return piece.type == type;
	}

	void UpdateBlock(Vector2 index) {
		PieceLevelData piece = GetPieceWithPos (index);
		var specific = piece.GetSpecificData<BlockPieceLevelData> ();

		// Sides
		for (int i = 0; i < 4; i++) {
			if (IsPieceOfPieceType (GetPieceWithPos (GetNeighborIndex (index, (Direction)i)), PieceType.BlockNonSticky)) {
				specific.sides [i] = BlockPieceLevelData.SideType.None;
			} else {
				if (specific.sides [i] == BlockPieceLevelData.SideType.None) {
					specific.sides [i] = BlockPieceLevelData.SideType.Normal;
				}
			}
		}

		// Corners
		for (int i = 0; i < 4; i++) {
			specific.corners [i] = BlockPieceLevelData.SideType.None;
			var tmpPiece1 = GetPieceWithPos (GetNeighborIndex (index, (Direction)i));
			var tmpPiece2 = GetPieceWithPos (GetNeighborIndex (index, (Direction)((4+i-1)%4)));
			if (tmpPiece1 != null && tmpPiece2 != null) {
				if (tmpPiece1.type == PieceType.BlockNonSticky && tmpPiece1.GetSpecificData<BlockPieceLevelData> ().sides [(4+i-1)%4] != BlockPieceLevelData.SideType.None &&
					tmpPiece2.type == PieceType.BlockNonSticky && tmpPiece2.GetSpecificData<BlockPieceLevelData> ().sides [i] != BlockPieceLevelData.SideType.None) {
					specific.corners [i] = BlockPieceLevelData.SideType.Normal;
				}
			}
		}
		piece.SaveSpecificData (specific);
	}

	PieceGroupData selectedPieceGroup = null;
	string startPointId = "-1";
	string endPointId = "-1";

	PieceGroupData PieceGroupDrawer(Rect rect, PieceGroupData value) {
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

			if (isOn) {
				ReorderableListGUI.ListField<GroupMovement> (value.moves, PieceGroupMovesDrawer);
			}
		}
		return value;
	}

	GroupMovement PieceGroupMovesDrawer(Rect rect, GroupMovement value) {
		var r = new Rect (rect);
		if (value != null) {
			r.width = 20;
			bool isChoosingStartPoint = EditorGUI.Toggle (r, startPointId == value.id);
			if (isChoosingStartPoint) {
				startPointId = value.id;
			} else {
				if (startPointId == value.id) {
					startPointId = "-1";
				}
			}

			r.x += 40;

			bool isChoosingEndPoint = EditorGUI.Toggle (r, endPointId == value.id);
			if (isChoosingEndPoint) {
				endPointId = value.id;
			} else {
				if (endPointId == value.id) {
					endPointId = "-1";
				}
			}

			r.x += 80;
			r.width = 40;
			value.delay = EditorGUI.FloatField (r, value.delay);
			r.x += 80;
			r.width = 40;
			value.time = EditorGUI.FloatField (r, value.time);
			r.x += 80;
			r.width = 80;
			value.animationCurve = EditorGUI.CurveField (r, value.animationCurve);
			r.x += 100;
			r.width = 80;
			value.maxT = EditorGUI.FloatField (r, value.maxT);
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

	GroupMovement GetGroupMovementWithId(string id) {
		foreach(GroupMovement move in selectedPieceGroup.moves) {
			if (move.id == id) {
				return move;
			}
		}
		return null;
	}

	bool IsPositionWithinGrid(Vector2 pos, float rightMargin = 20,  float downMargin = 20) {
		return 	pos.x > levelGridRect.x && pos.x < levelGridRect.x + levelGridRect.width -rightMargin &&
				pos.y > levelGridRect.y && pos.y < levelGridRect.y + levelGridRect.height-downMargin;
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

				PieceLevelData tmpPiece = null;
				Texture2D tmpTexture = cellTexture;
				foreach(PieceLevelData piece in ((LevelAsset)target).pieces) {
					if (x == piece.pos.x && y == piece.pos.y) {
						tmpPiece = piece;
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
							GUI.color = Color.grey;
						}
						if (piece.type == PieceType.Collectable) {
							GUI.color = Color.yellow;
						}
						if (piece.type == PieceType.BlockDestructible) {
							GUI.color = new Color(0.2f,0.2f,0.2f,1);
							tmpTexture = blockDestructibleTexture;
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

				if (tmpPiece != null && tmpPiece.type == PieceType.BlockNonSticky) {
					var specific = tmpPiece.GetSpecificData<BlockPieceLevelData> ();
					if (!String.IsNullOrEmpty (tmpPiece.specificDataJson)) {
						GUI.color = Color.cyan;
						int i = 0;
						foreach (var side in specific.sides) {
							if (side != BlockPieceLevelData.SideType.None) {
								if (side == BlockPieceLevelData.SideType.Normal) {
									GUI.color = new Color(0.1f,0.1f,0.1f,1);
								}
								if (side == BlockPieceLevelData.SideType.Sticky) {
									GUI.color = new Color(0.4f,0.4f,0.4f,1);
								}
								if (side == BlockPieceLevelData.SideType.Colorable) {
									GUI.color = Color.white;
								}
								GUIUtility.RotateAroundPivot((int)i*90, rect.center);
								GUI.DrawTexture (rect, blockSideTexture, ScaleMode.ScaleToFit);
								GUI.matrix = prevMatrix;
							}
							i++;
						}
						foreach (var corner in specific.corners) {
							if (corner != BlockPieceLevelData.SideType.None) {
								if (corner == BlockPieceLevelData.SideType.Normal) {
									GUI.color = new Color(0.1f,0.1f,0.1f,1);
								}
								if (corner == BlockPieceLevelData.SideType.Sticky) {
									GUI.color = new Color(0.4f,0.4f,0.4f,1);
								}
								if (corner == BlockPieceLevelData.SideType.Colorable) {
									GUI.color = Color.white;
								}
								GUIUtility.RotateAroundPivot((int)i*90, rect.center);
								GUI.DrawTexture (rect, blockCornerTexture, ScaleMode.ScaleToFit);
								GUI.matrix = prevMatrix;
							}
							i++;
						}
					}
				}
				GUI.matrix = prevMatrix;


			}
		}
		if (selectedPieceGroup != null) {
			foreach(var move in selectedPieceGroup.moves) {
				Handles.color = Color.red;
				Handles.DrawLine (move.startPoint*cellSize+new Vector2(cellSize,cellSize)*0.5f,move.endPoint*cellSize+new Vector2(cellSize,cellSize)*0.5f);
				Handles.color = Color.white;
			}
		}

		GUI.color = Color.white;
	}
}
#endif
