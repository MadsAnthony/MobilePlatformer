using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInit : MonoBehaviour {
	public LevelAsset level;

	private Vector3 levelStartPos = new Vector3(-9.5f,16,0);

	private GameLogic gameLogic;
	// Use this for initialization
	void Start () {
		gameLogic = GetComponent<GameLogic>();
		if (Director.Instance.levelIndex >= 0) {
			level = Director.LevelDatabase.levels[Director.Instance.levelIndex];
		}

		InitializeLevel();
	}

	void InitializeLevel() {
		int i = 0;

		foreach (var piece in level.pieces) {
			PieceData pieceData = Director.PieceDatabase.GetPieceData (piece.type);
			var tmpBlock = Instantiate(pieceData.prefab);

			if (piece.type == PieceType.BlockNormal) {
				tmpBlock.name = "Block"+i;
			}

			if (piece.type == PieceType.BlockColor) {
				gameLogic.coloredBlocksGoal++;
				tmpBlock.name = "ColorBlock"+i;
			}

			if (piece.type == PieceType.Spike) {
				tmpBlock.name = "Spike"+i;
			}

			if (piece.type == PieceType.BlockNonSticky) {
				tmpBlock.name = "NonSticky"+i;
			}

			if (piece.type == PieceType.Collectable) {
				tmpBlock.name = "Collectable"+i;
				gameLogic.collectablesGoal++;
			}
			if (piece.type == PieceType.BlockDestructible) {
				tmpBlock.name = "Destructable"+i;
			}

			tmpBlock.transform.eulerAngles = new Vector3(tmpBlock.transform.eulerAngles.x,tmpBlock.transform.eulerAngles.y,((int)piece.dir)*-90);
			tmpBlock.transform.position = new Vector3(piece.pos.x,-piece.pos.y,0)+levelStartPos;
		}

		var hero = Instantiate(Director.PieceDatabase.GetPieceData (PieceType.Hero).prefab);
		hero.transform.position = new Vector3(level.heroPos.x,-level.heroPos.y+0.05f,0)+levelStartPos;

		gameLogic.hero = hero.GetComponent<Hero> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.R)) {
			SceneManager.LoadScene ("LevelScene");
		}
	}
}
