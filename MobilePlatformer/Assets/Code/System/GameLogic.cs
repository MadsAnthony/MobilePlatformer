using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {
	public int collectablesCollected;
	public int collectablesGoal;
	public int coloredBlocksGoal;

	public int CurrentColoredBlocks {get {return currentColoredBlocks;}}
	int currentColoredBlocks = 0;
	public int CollectablesCollected { get; set;}

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
			currentColoredBlocks++;
			if (currentColoredBlocks >= coloredBlocksGoal) {
				Director.GameEventManager.Emit (GameEventType.LevelCompleted);
			}
			break;
		case GameEventType.CollectableCollected:
			CollectablesCollected++;
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
