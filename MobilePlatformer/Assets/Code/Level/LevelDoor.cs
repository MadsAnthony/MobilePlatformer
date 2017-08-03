using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : Piece {
	private PieceLevelData pieceLevelData;
	private GameLogic gameLogic;

	public override void Init (PieceLevelData pieceLevelData, GameLogic gameLogic) {
		this.pieceLevelData = pieceLevelData;
		this.gameLogic = gameLogic;
	}

	public override void Hit (Piece hitPiece, Vector3 direction)
	{
		if (hitPiece.Type == PieceType.Hero) {
			((Hero)hitPiece).OnALevelDoor ();
			var levelDoorText = Instantiate(Resources.Load("LevelDoorText")) as GameObject;
			levelDoorText.transform.position = new Vector3(transform.position.x,transform.position.y+2,transform.position.z);
			int levelIndex = pieceLevelData.GetSpecificData<LevelDoorPieceLevelData> ().levelIndex;
			Director.Instance.levelIndex = levelIndex;
			levelDoorText.GetComponentInChildren<TextMesh> ().text = "- level "+ (levelIndex+1)+" -";

			gameLogic.hero.OnIsOnLevelDoorChangeValue += (bool isOnALevelDoor)=>{if (!isOnALevelDoor) Destroy(levelDoorText);};
		}
	}
}
