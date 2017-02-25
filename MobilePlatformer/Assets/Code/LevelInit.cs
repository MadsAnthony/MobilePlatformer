using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInit : MonoBehaviour {
	public LevelAsset level;

	private Vector3 levelStartPos = new Vector3(-9.5f,16,0);
	public GameObject blockPrefab;
	public GameObject heroPrefab;
	// Use this for initialization
	void Start () {
		InitializeLevel();
	}

	void InitializeLevel() {
		foreach (var cell in level.gridObjects) {
			var tmpBlock = Instantiate(blockPrefab);
			tmpBlock.transform.position = new Vector3(cell.x,-cell.y,0)+levelStartPos;
		}

		var hero = Instantiate(heroPrefab);
		hero.transform.position = new Vector3(level.heroPos.x,-level.heroPos.y,0)+levelStartPos;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
