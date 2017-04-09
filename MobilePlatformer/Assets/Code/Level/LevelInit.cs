using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInit : MonoBehaviour {
	public LevelAsset level;

	private Vector3 levelStartPos = new Vector3(-9.5f,16,0);

	private GameLogic gameLogic;

	private Dictionary<string, Piece> pieces = new Dictionary<string, Piece>();
	// Use this for initialization
	void Start () {
		gameLogic = GetComponent<GameLogic>();
		if (Director.Instance.levelIndex >= 0) {
			level = Director.LevelDatabase.levels[Director.Instance.levelIndex];
		}
		gameLogic.level = level;

		InitializeLevel();
	}

	void InitializeLevel() {
		int i = 0;

		foreach (var piece in level.pieces) {
			PieceData pieceData = Director.PieceDatabase.GetPieceData (piece.type);
			var tmpPiece = Instantiate(pieceData.prefab);
			tmpPiece.name = "Piece"+i;

			tmpPiece.Init(piece, gameLogic);

			if (piece.type == PieceType.Collectable) {
				gameLogic.collectablesGoal++;
			}
			if (piece.type == PieceType.Hero) {
				gameLogic.hero = tmpPiece.GetComponent<Hero> ();
			}

			tmpPiece.transform.eulerAngles = new Vector3(tmpPiece.transform.eulerAngles.x,tmpPiece.transform.eulerAngles.y,((int)piece.dir)*-90);
			tmpPiece.transform.position = new Vector3(piece.pos.x,-piece.pos.y,0)+levelStartPos;
			i++;

			// Remove this condition at some time.
			if (!string.IsNullOrEmpty(piece.id)) {
				pieces.Add (piece.id, tmpPiece);
			}
		}

		foreach (PieceGroupData pieceGroup in level.pieceGroups) {
			var go = new GameObject();
			PieceGroup goPieceGroup = go.AddComponent<PieceGroup> ();
			goPieceGroup.pieceGroupData = pieceGroup;
			foreach (var pieceId in pieceGroup.pieceIds) {
				goPieceGroup.pieces.Add(pieces [pieceId]);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.R)) {
			SceneManager.LoadScene ("LevelScene");
		}
	}
}
