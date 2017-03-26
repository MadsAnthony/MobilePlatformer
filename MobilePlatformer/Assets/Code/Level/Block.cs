using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Piece {

	public GameObject[] SideGameObjects;
	BlockPieceLevelData.SideType[] sides = new BlockPieceLevelData.SideType[4];

	public override void Init (PieceLevelData pieceLevelData) {
		if (String.IsNullOrEmpty (pieceLevelData.specificDataJson)) return;
		var specific = pieceLevelData.GetSpecificData<BlockPieceLevelData>();

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
				sideGameObject.GetComponent<SpriteRenderer> ().color = Color.grey;
			}
			i++;
		}
	}

	public override void Hit (Piece hitPiece)
	{
	}

}
