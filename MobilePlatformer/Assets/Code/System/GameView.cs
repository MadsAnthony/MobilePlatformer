using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameView : UIView {
	public GameLogic gameLogic;
	public Text goalText;

	protected override void OnStart () {
		Director.GameEventManager.OnGameEvent += HandleGameEvent;
	}

	protected override void OnRemoving() {
		Director.GameEventManager.OnGameEvent -= HandleGameEvent;
	}

	void HandleGameEvent(GameEvent e) {
		switch (e.type) {
		case GameEventType.LevelCompleted:
			Director.Instance.levelIndex += 1;
			if (Director.Instance.levelIndex >= Director.LevelDatabase.levels.Count) {
				SceneManager.LoadScene ("IntroScene");
			} else {
				Director.TransitionManager.PlayTransition (() => {SceneManager.LoadScene ("LevelScene");},0.1f);
			}
			break;
		}
	}

	// Update is called once per frame
	void Update () {
		goalText.text = gameLogic.CurrentColoredBlocks+"/"+gameLogic.coloredBlocksGoal;
	}
}
