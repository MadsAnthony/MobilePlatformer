﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rotorz.ReorderableList;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public enum LevelEditorTool {PlacePieces, ModifyBlock, Select};

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
	LevelEditorTool cellType = LevelEditorTool.Select;
	PieceType pieceType;
	public Direction direction;
	public BlockPieceLevelData.SideType blockSideType;
	Vector2 scrollPos;
	Vector2 mouseDownPos;

	Vector2 selectionAreaStartPoint;
	Vector2 selectionAreaEndPoint;


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

		var levelSizeX = EditorGUILayout.IntSlider((int)myTarget.levelSize.x,0,50);
		var levelSizeY = EditorGUILayout.IntSlider((int)myTarget.levelSize.y,0,50);

		myTarget.levelSize = new Vector2 (levelSizeX, levelSizeY);
		cellType = (LevelEditorTool)EditorGUILayout.Popup("Cell Type", (int)cellType, Enum.GetNames (typeof(LevelEditorTool)));

		EditorGUILayout.BeginHorizontal();
		if (cellType == LevelEditorTool.PlacePieces) {
			string[] pieceOptions = Enum.GetNames (typeof(PieceType));
			pieceType = (PieceType)EditorGUILayout.Popup ("Piece Type", (int)pieceType, pieceOptions);
		}

		direction = (Direction)EditorGUILayout.Popup("Direction", (int)direction, Enum.GetNames (typeof(Direction)));
		EditorGUILayout.EndHorizontal();

		if (cellType == LevelEditorTool.ModifyBlock) {
			blockSideType = (BlockPieceLevelData.SideType)EditorGUILayout.Popup("SideType", (int)blockSideType, Enum.GetNames (typeof(BlockPieceLevelData.SideType)));
		}

		if (Event.current.type == EventType.MouseDown) {
			mouseDownPos = Event.current.mousePosition;
		}


		if (Event.current.type == EventType.MouseUp && mouseDownPos.x != 0 && mouseDownPos.y != 0) {
			mouseDownPos = Vector2.zero;
		}

		if (SelectionOfPieces.Count > 0) {
			if (Event.current.keyCode == KeyCode.Delete) {
				DeleteSelectedPieces ();
			}
		}

		if (IsPositionWithinGrid(mouseDownPos) && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)) {
			selectedIndex = GetClosestCell (Event.current.mousePosition);

			if (selectedPieceGroup == null) {
				if (Event.current.button == 0) {
					if (cellType == LevelEditorTool.PlacePieces) {
						AddPiece (selectedIndex,pieceType);
					}
					if (cellType == LevelEditorTool.ModifyBlock) {
						ModifyBlock (selectedIndex, blockSideType);
					}
					if (cellType == LevelEditorTool.Select && SelectionOfPieces.Count>0) {
						if (Event.current.control) {
							CopySelectedPieces(selectedIndex, Event.current.mousePosition);
						}

						MoveSelectedPieces(selectedIndex, Event.current.mousePosition);
					}
				}
				if (Event.current.button == 1) {
					if (cellType == LevelEditorTool.PlacePieces) {
						RemovePiece (selectedIndex);
					}
					if (cellType == LevelEditorTool.ModifyBlock) {
						ModifyBlock (selectedIndex,BlockPieceLevelData.SideType.Normal);
					}
				}
			} else {
				if (Event.current.button == 0) {
					PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
					if (existingPiece != null) {
						if (!selectedPieceGroup.pieceIds.Contains (existingPiece.id)) {
							selectedPieceGroup.pieceIds.Add (existingPiece.id);
							UpdateBlock (selectedIndex);
							UpdateNeighborBlocks(selectedIndex);
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
						UpdateBlock (selectedIndex);
						UpdateNeighborBlocks(selectedIndex);
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

		ReorderableListGUI.Title("Selection");
		ReorderableListGUI.ListField<PieceLevelData>(SelectionOfPieces, SelectionOfPieceDrawer);

		ReorderableListGUI.Title("Groups");
		ReorderableListGUI.ListField<PieceGroupData>(myTarget.pieceGroups, PieceGroupDrawer);

		if (cellType == LevelEditorTool.Select && !IsDraggingSelection) {
			if (Event.current.type == EventType.MouseDown) {
				selectionAreaStartPoint = Event.current.mousePosition;
			}
			if (Event.current.type == EventType.MouseDrag) {
				selectionAreaEndPoint = Event.current.mousePosition - selectionAreaStartPoint;
			}
			if (Event.current.type == EventType.MouseUp) {
				Rect selectionRect = new Rect (selectionAreaStartPoint, selectionAreaEndPoint);
				AddSelectionOfPieces(selectionRect);
				selectionAreaStartPoint = Vector2.zero;
				selectionAreaEndPoint = Vector2.zero;
				EditorUtility.SetDirty (myTarget);
			}
		}
		if (cellType == LevelEditorTool.Select && IsDraggingSelection) {
			if (Event.current.type == EventType.MouseUp) {
				PlaceSelection();
				IsCopySelection = false;
				IsDraggingSelection = false;
			}
		}

		GUI.color = new Color(0,0,0.8f,0.5f);
		GUI.DrawTexture(new Rect (selectionAreaStartPoint.x, selectionAreaStartPoint.y, selectionAreaEndPoint.x, selectionAreaEndPoint.y),blockTexture,ScaleMode.StretchToFill);
		GUI.color = Color.white;


		if (GUI.changed) {
			EditorUtility.SetDirty (myTarget);
		}
		serializedObject.ApplyModifiedProperties();
	}

	List<PieceLevelData> SelectionOfPieces = new List<PieceLevelData>();
	bool IsDraggingSelection = false;
	bool IsCopySelection = false;
	Vector2 DragPoint;

	void AddSelectionOfPieces(Rect rect) {
		Vector2 startIndex = GetClosestCell (new Vector2(Mathf.Min(rect.xMin,rect.xMax),Mathf.Min(rect.yMin,rect.yMax)));
		Vector2 endIndex   = GetClosestCell (new Vector2(Mathf.Max(rect.xMin,rect.xMax),Mathf.Max(rect.yMin,rect.yMax)));

		SelectionOfPieces.Clear();
		foreach(PieceLevelData piece in ((LevelAsset)target).pieces) {
			if (piece.pos.x >= startIndex.x && piece.pos.x <= endIndex.x &&
				piece.pos.y >= startIndex.y && piece.pos.y <= endIndex.y) {
				SelectionOfPieces.Add (piece);
			}
		}
	}

	void DeleteSelectedPieces() {
		foreach (var selectedPiece in SelectionOfPieces) {
			RemovePiece (selectedPiece.pos);
		}
	}

	void CopySelectedPieces(Vector2 selectedIndex, Vector2 mousePos) {
		if (IsCopySelection) return;
		if (!IsDraggingSelection) {
			foreach (var selectedPiece in SelectionOfPieces) {
				if (selectedPiece.pos == selectedIndex) {
					IsDraggingSelection = true;
					DragPoint = selectedIndex;
					break;
				}
			}
		}

		List<PieceLevelData> newSelection = new List<PieceLevelData>();
		if (IsDraggingSelection) {
			
			string[] groupIds = GetPieceGroupIds (SelectionOfPieces.ToArray());

			//Dictionary<string,string> groupToNewGroup = new Dictionary<string,string>();
			List<PieceGroupData> newGroups = new List<PieceGroupData>();
			Dictionary<string,string> pieceToNewPiece = new Dictionary<string,string>();

			foreach (var groupId in groupIds) {
				PieceGroupData group = GetPieceGroup (groupId);
				var newGroup = CopyPieceGroup (group);
				newGroups.Add (newGroup);
			}

			foreach (var selectedPiece in SelectionOfPieces) {
				var newPiece = AddPiece (selectedPiece.pos, selectedPiece.type, false);
				newSelection.Add (newPiece);
				pieceToNewPiece.Add (selectedPiece.id, newPiece.id);
			}

			foreach (var newGroup in newGroups) {
				List<string> newPieceIdList = new List<String> ();
				foreach (var pieceId in newGroup.pieceIds) {
					if (pieceToNewPiece.ContainsKey (pieceId)) {
						newPieceIdList.Add (pieceToNewPiece [pieceId]);
					}
				}
				newGroup.pieceIds.Clear ();
				newGroup.pieceIds.AddRange (newPieceIdList.ToArray ());
			}

			IsCopySelection = true;
		}
		SelectionOfPieces = newSelection;
	}

	PieceGroupData CopyPieceGroup(PieceGroupData pieceGroup) {
		var newPieceGroup = new PieceGroupData();
		newPieceGroup.pieceIds.AddRange(pieceGroup.pieceIds.ToArray ());

		foreach (var move in pieceGroup.moves) {
			var newGroupMovement = new GroupMovement ();
			newGroupMovement.startPoint 	= move.startPoint;
			newGroupMovement.endPoint 		= move.endPoint;
			newGroupMovement.delay 			= move.delay;
			newGroupMovement.time 			= move.time;
			newGroupMovement.animationCurve = move.animationCurve;
			newGroupMovement.maxT 			= move.maxT;

			newPieceGroup.moves.Add (newGroupMovement);
		}

		((LevelAsset)target).pieceGroups.Add (newPieceGroup);

		return newPieceGroup;
	}

	void MoveSelectedPieces(Vector2 selectedIndex, Vector2 mousePos) {
		if (!IsDraggingSelection) {
			foreach (var selectedPiece in SelectionOfPieces) {
				if (selectedPiece.pos == selectedIndex) {
					IsDraggingSelection = true;
					DragPoint = selectedIndex;
					break;
				}
			}
		}
		string[] groupIds = GetPieceGroupIds (SelectionOfPieces.ToArray());

		if (IsDraggingSelection) {
			foreach (var groupId in groupIds) {
				PieceGroupData group = GetPieceGroup (groupId);
				if (group == null) continue;

				foreach (var groupMove in group.moves) {
					groupMove.startPoint += GetClosestCell (mousePos)-DragPoint;
					groupMove.endPoint 	 += GetClosestCell (mousePos)-DragPoint;
				}
			}

			foreach (var selectedPiece in SelectionOfPieces) {
				selectedPiece.pos += GetClosestCell (mousePos)-DragPoint;
			}
			DragPoint = GetClosestCell (mousePos);
		}
	}

	void PlaceSelection() {
		serializedObject.ApplyModifiedProperties();
		foreach (var selectedPiece in SelectionOfPieces) {
			var newPos = selectedPiece.pos;
			selectedPiece.pos = new Vector2 (-1, -1);
			RemovePiece (newPos);
			selectedPiece.pos = newPos;
			UpdateBlock (newPos);
			UpdateNeighborBlocks(newPos);
		}
	}

	PieceLevelData AddPiece(Vector2 index, PieceType pieceType, bool removeExisting = true) {
		LevelAsset myTarget = (LevelAsset)target;

		PieceLevelData existingPiece = GetPieceWithPos (selectedIndex);
		if (removeExisting && existingPiece != null) {
			myTarget.pieces.Remove (existingPiece);
		}
		PieceLevelData newPiece = new PieceLevelData (pieceType, index, direction);
		myTarget.pieces.Add (newPiece);

		if (pieceType == PieceType.Block) {
			UpdateBlock (index);
		}
		UpdateNeighborBlocks(index);

		return newPiece;
	}

	void RemovePiece(Vector2 index) {
		LevelAsset myTarget = (LevelAsset)target;

		PieceLevelData existingPiece = GetPieceWithPos (index);
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
			if (GetPieceWithPos (tmpIndex) != null && GetPieceWithPos (tmpIndex).type == PieceType.Block) UpdateBlock (tmpIndex);
		}

		//Corners
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				var addIndex = new Vector2 (i - 1, j - 1);
				if (addIndex.x == 0 || addIndex.y == 0) continue;
				var tmpIndex = index + addIndex;
				if (GetPieceWithPos (tmpIndex) != null && GetPieceWithPos (tmpIndex).type == PieceType.Block) UpdateBlock (tmpIndex);
			}
		}
	}

	bool IsPieceOfPieceType(PieceLevelData piece, PieceType type) {
		if (piece == null) return false;
		return piece.type == type;
	}

	string[] GetPieceGroupIds(PieceLevelData[] pieces) {
		List<String> groupdIds = new List<String> ();

		foreach (PieceLevelData piece in pieces) {
			var groupId = GetPieceGroupId(piece);
			if (groupId != "-1" && !groupdIds.Contains(groupId)) {
				groupdIds.Add (groupId);
			}
		}

		return groupdIds.ToArray ();
	}

	PieceGroupData GetPieceGroup(string groupId) {
		foreach(PieceGroupData pieceGroup in ((LevelAsset)target).pieceGroups) {
			if (pieceGroup.id == groupId) return pieceGroup;
		}
		return null;
	}

	string GetPieceGroupId(PieceLevelData piece) {
		foreach(PieceGroupData pieceGroup in ((LevelAsset)target).pieceGroups) {
			if (pieceGroup.pieceIds.Contains(piece.id)) return pieceGroup.id;
		}
		return "-1";
	}

	void UpdateBlock(Vector2 index) {
		PieceLevelData piece = GetPieceWithPos (index);
		if (piece == null || piece.type != PieceType.Block) return;

		string pieceGroupId = GetPieceGroupId (piece);
		var specific = piece.GetSpecificData<BlockPieceLevelData> ();

		// Sides
		for (int i = 0; i < 4; i++) {
			var tmpPiece = GetPieceWithPos (GetNeighborIndex (index, (Direction)i));
			if (tmpPiece != null && tmpPiece.type == PieceType.Block && (GetPieceGroupId (tmpPiece) == pieceGroupId)) {
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
				if (tmpPiece1.type == PieceType.Block && tmpPiece1.GetSpecificData<BlockPieceLevelData> ().sides [(4+i-1)%4] != BlockPieceLevelData.SideType.None &&
					tmpPiece2.type == PieceType.Block && tmpPiece2.GetSpecificData<BlockPieceLevelData> ().sides [i] != BlockPieceLevelData.SideType.None) {
					specific.corners [i] = BlockPieceLevelData.SideType.Normal;
				}
			}
		}
		piece.SaveSpecificData (specific);
	}

	PieceLevelData SelectionOfPieceDrawer(Rect rect, PieceLevelData value) {
		var r = new Rect (rect);
		if (value != null) {
			r.width = 30;
			EditorGUI.LabelField (r, "pos:");
			r.x += 30;
			r.width = 30;
			EditorGUI.IntField (r,(int)value.pos.x);
			r.x += 35;
			EditorGUI.IntField (r,(int)value.pos.y);
			r.x += 40;
			r.width = 70;
			value.type = (PieceType)EditorGUI.EnumPopup (r, value.type);

			r.x += 75;
			r.width = 70;
			value.dir = (Direction)EditorGUI.EnumPopup (r, value.dir);
		}
		return value;
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

	Vector2 GetClosestCell(Vector2 pos) {
		if (IsPositionWithinGrid(pos)) {
			pos -= new Vector2(levelGridRect.x,levelGridRect.y)-scrollPos;
			var selectedIndex = new Vector2(Mathf.FloorToInt(pos.x/cellSize),Mathf.FloorToInt(pos.y/cellSize));
			return selectedIndex;
		} else {
			var selectedIndex = new Vector2 (-1, -1);
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
						if (piece.type == PieceType.Enemy1) {
							GUI.color = Color.magenta;
						}
						if (piece.type == PieceType.PieceType2) {
							GUI.color = Color.grey;
						}
						if (piece.type == PieceType.Spike) {
							GUIUtility.RotateAroundPivot((int)piece.dir*90, rect.center);
							GUI.color = Color.red;
							tmpTexture = spikeTexture;
						}
						if (piece.type == PieceType.Hero) {
							GUI.color = Color.green;
						}
						if (piece.type == PieceType.Block) {
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
							GUI.color = new Color(GUI.color.r+0.5f,GUI.color.g+0.5f,GUI.color.b,GUI.color.a);
						}
						if (SelectionOfPieces.Exists(p => p.id == piece.id)) {
							GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b+0.5f,GUI.color.a);
						}
					}
				}

				GUI.DrawTexture(rect,tmpTexture,ScaleMode.ScaleToFit);

				if (tmpPiece != null && tmpPiece.type == PieceType.Block) {
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
