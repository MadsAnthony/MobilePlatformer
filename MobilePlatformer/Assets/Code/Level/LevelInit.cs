using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInit : MonoBehaviour {
	public LevelAsset level;

	private Vector3 levelStartPos = new Vector3(-9.5f,16,0);
	public GameObject normalBlockPrefab;
	public GameObject blockPrefab;
	public GameObject spikePrefab;
	public GameObject heroPrefab;

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

		foreach (var block in level.blocks) {
			GameObject tmpBlock = null;
			if (block.type == BlockType.Normal) {
				tmpBlock = Instantiate(normalBlockPrefab);
				tmpBlock.name = "Block"+i;
			}
			if (block.type == BlockType.Color) {
				tmpBlock = Instantiate(blockPrefab);
				tmpBlock.name = "ColorBlock"+i;
				gameLogic.coloredBlocksGoal++;
			}
			if (block.type == BlockType.Spike) {
				tmpBlock = Instantiate(spikePrefab);
				tmpBlock.name = "Spike"+i;
			}
			i ++;

			tmpBlock.transform.position = new Vector3(block.pos.x,-block.pos.y,0)+levelStartPos;
		}

		var hero = Instantiate(heroPrefab);
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
