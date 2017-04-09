using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Piece {

	public GameObject[] SideGameObjects;
	PieceLevelData pieceLevelData;
	BlockPieceLevelData.SideType[] sides = new BlockPieceLevelData.SideType[4];

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
		if (String.IsNullOrEmpty (pieceLevelData.specificDataJson)) return;
		var specific = pieceLevelData.GetSpecificData<BlockPieceLevelData>();

		this.pieceLevelData = pieceLevelData;
		int i = 0;
		foreach (var side in specific.sides) {
			sides[i] = side;
			i++;
		}

		i = 0;
		foreach (var sideGameObject in SideGameObjects) {
			sideGameObject.SetActive (false);
			if (specific.sides [i] == BlockPieceLevelData.SideType.Normal) {
				sideGameObject.SetActive (true);
				sideGameObject.GetComponent<SpriteRenderer> ().color = new Color (0.2f, 0.2f, 0.2f, 1);
			}
			if (specific.sides [i] == BlockPieceLevelData.SideType.Sticky) {
				sideGameObject.SetActive (true);
				sideGameObject.GetComponent<SpriteRenderer> ().color = new Color (0.6f, 0.6f, 0.6f, 1);
			}
			if (specific.sides [i] == BlockPieceLevelData.SideType.Colorable) {
				sideGameObject.SetActive (true);
				sideGameObject.GetComponent<SpriteRenderer> ().color = Color.white;
				gameLogic.coloredBlocksGoal++;
			}
			i++;
		}
	}

	public bool IsSticky(Vector3 incommingDir) {
		float tmpThreshold = 0.1f;
		bool endResult = false;
		if (incommingDir.x < -tmpThreshold) {
			endResult |= IsSideSticky(sides[(int)Direction.Right]);
		}
		if (incommingDir.x > tmpThreshold) {
			endResult |= IsSideSticky(sides[(int)Direction.Left]);
		}
		if (incommingDir.y > tmpThreshold) {
			endResult |= IsSideSticky(sides[(int)Direction.Down]);
		}
		if (incommingDir.y < -tmpThreshold) {
			endResult |= IsSideSticky(sides[(int)Direction.Up]);
		}
		return endResult;
	}

	public bool IsSideSticky(BlockPieceLevelData.SideType sideType) {
		return (sideType == BlockPieceLevelData.SideType.Sticky || sideType == BlockPieceLevelData.SideType.Colorable);
	}

	Direction GetDirectionFromVector(Vector3 direction) {
		float tmpThreshold = 0.1f;

		if (direction.x < -tmpThreshold) {
			return Direction.Right;
		}
		if (direction.x > tmpThreshold) {
			return Direction.Left;
		}
		if (direction.y > tmpThreshold) {
			return Direction.Down;
		}
		if (direction.y < -tmpThreshold) {
			return Direction.Up;
		}

		return Direction.Up;
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Hero) {
			Direction tmpDir = GetDirectionFromVector (direction);

			if (sides [(int)tmpDir] == BlockPieceLevelData.SideType.Colorable && SideGameObjects [(int)tmpDir].GetComponent<SpriteRenderer> ().color != Color.green) {
				SideGameObjects [(int)tmpDir].GetComponent<SpriteRenderer> ().color = Color.green;
				Director.GameEventManager.Emit (GameEventType.BlockColored);
			}
		}

		if (hitPiece.Type == PieceType.Enemy1) {
			Direction tmpDir = GetDirectionFromVector (direction);

			if (sides [(int)tmpDir] == BlockPieceLevelData.SideType.Colorable && SideGameObjects [(int)tmpDir].GetComponent<SpriteRenderer> ().color == Color.green) {
				SideGameObjects [(int)tmpDir].GetComponent<SpriteRenderer> ().color = Color.white;
				Director.GameEventManager.Emit (GameEventType.BlockUnColored);
			}
		}
	}
}
