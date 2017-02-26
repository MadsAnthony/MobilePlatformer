using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInit : MonoBehaviour {
	public LevelAsset level;

	private Vector3 levelStartPos = new Vector3(-9.5f,16,0);
	public GameObject blockPrefab;
	public GameObject heroPrefab;
	// Use this for initialization
	void Start () {
		if (Director.Instance.levelIndex >= 0) {
			level = Director.Instance.levelDatabase.levels[Director.Instance.levelIndex];
		}

		InitializeLevel();
	}

	void InitializeLevel() {
		int i = 0;
		foreach (var cell in level.gridObjects) {
			var tmpBlock = Instantiate(blockPrefab);
			i ++;
			tmpBlock.name = "Block"+i;
			tmpBlock.transform.position = new Vector3(cell.x,-cell.y,0)+levelStartPos;
		}

		var hero = Instantiate(heroPrefab);
		hero.transform.position = new Vector3(level.heroPos.x,-level.heroPos.y+0.05f,0)+levelStartPos;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.R)) {
			SceneManager.LoadScene (0);
		}
	}
}
