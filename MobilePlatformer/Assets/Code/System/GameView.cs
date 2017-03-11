using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameView : UIView {
	public GameLogic gameLogic;
	public Text goalText;
	public Text collectableText;

	public Camera camera;

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
		collectableText.text = gameLogic.CollectablesCollected+"/"+gameLogic.collectablesGoal;

		var distance = gameLogic.hero.transform.position - camera.transform.position;
		camera.transform.position += Time.deltaTime*3*new Vector3(distance.x,distance.y,0);

		camera.transform.position = new Vector3(Mathf.Clamp(camera.transform.position.x,0,gameLogic.level.levelSize.x-20),Mathf.Clamp(camera.transform.position.y,Mathf.Min(-(gameLogic.level.levelSize.y-30),0),0),camera.transform.position.z);
	}
}
