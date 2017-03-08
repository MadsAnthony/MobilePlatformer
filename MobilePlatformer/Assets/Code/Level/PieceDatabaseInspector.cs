using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(PieceDatabase))]
public class PieceDatabaseInspector : Editor {
	public override void OnInspectorGUI()
	{
		PieceDatabase myTarget = (PieceDatabase)target;

		string[] pieceTypes = Enum.GetNames (typeof(PieceType));
		foreach(var pieceTypeStr in pieceTypes) {
			PieceType pieceType = (PieceType)Enum.Parse (typeof(PieceType), pieceTypeStr);
			if (GetPieceWithType (pieceType) == null) {
				myTarget.pieces.Add (new PieceData(pieceType));
			}
		}
		foreach (PieceData piece in ((PieceDatabase)target).pieces) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (piece.type.ToString());
			piece.prefab = (Piece)EditorGUILayout.ObjectField (piece.prefab,(typeof(Piece)));
			if (piece.prefab != null) piece.prefab.IsPassable = EditorGUILayout.Toggle (piece.prefab.IsPassable);
			EditorGUILayout.EndHorizontal ();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty (myTarget);
		}
	}

	PieceData GetPieceWithType(PieceType type) {
		foreach(PieceData piece in ((PieceDatabase)target).pieces) {
			if (piece.type == type) {
				return piece;
			}
		}
		return null;
	}
}
#endif