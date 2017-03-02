using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {
	public int coloredBlocksGoal;

	public int CurrentColoredBlocks {get {return currentColoredBlocks;}}
	int currentColoredBlocks = 0;

	public Hero hero;
	// Use this for initialization
	void Start () {
		Director.GameEventManager.OnGameEvent += HandleGameEvent;
	}

	void OnDestroy() {
		Director.GameEventManager.OnGameEvent -= HandleGameEvent;
	}

	void HandleGameEvent(GameEvent e) {
		switch (e.type) {
		case GameEventType.BlockColored:
			if (currentColoredBlocks % 2 == 0) {
				Director.Sounds.splat.Play ();
			}
			currentColoredBlocks++;
			if (currentColoredBlocks >= coloredBlocksGoal) {
				Director.GameEventManager.Emit (GameEventType.LevelCompleted);
			}
			break;
		case GameEventType.LevelCompleted:
			hero.StopMoving ();
			break;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
