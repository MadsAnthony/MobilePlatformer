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
	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Hero) {
			float tmpThreshold = 0.1f;
			if (direction.x < -tmpThreshold) {
				if (sides [(int)Direction.Right] == BlockPieceLevelData.SideType.Colorable && SideGameObjects[(int)Direction.Right].GetComponent<SpriteRenderer> ().color != Color.green) {
					SideGameObjects[(int)Direction.Right].GetComponent<SpriteRenderer> ().color = Color.green;
					Director.GameEventManager.Emit (GameEventType.BlockColored);
				}
			}
			if (direction.x > tmpThreshold) {
				if (sides [(int)Direction.Left] == BlockPieceLevelData.SideType.Colorable && SideGameObjects[(int)Direction.Left].GetComponent<SpriteRenderer> ().color != Color.green) {
					SideGameObjects[(int)Direction.Left].GetComponent<SpriteRenderer> ().color = Color.green;
					Director.GameEventManager.Emit (GameEventType.BlockColored);
				}
			}
			if (direction.y > tmpThreshold) {
				if (sides [(int)Direction.Down] == BlockPieceLevelData.SideType.Colorable && SideGameObjects[(int)Direction.Down].GetComponent<SpriteRenderer> ().color != Color.green) {
					SideGameObjects[(int)Direction.Down].GetComponent<SpriteRenderer> ().color = Color.green;
					Director.GameEventManager.Emit (GameEventType.BlockColored);
				}
			}
			if (direction.y < -tmpThreshold) {
				if (sides[(int)Direction.Up] == BlockPieceLevelData.SideType.Colorable && SideGameObjects[(int)Direction.Up].GetComponent<SpriteRenderer> ().color != Color.green) {
					SideGameObjects[(int)Direction.Up].GetComponent<SpriteRenderer> ().color = Color.green;
					Director.GameEventManager.Emit (GameEventType.BlockColored);
				}
			}
		}
	}
}
