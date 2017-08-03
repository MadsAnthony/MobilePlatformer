using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameView : UIView {
	public GameLogic gameLogic;
	public Text goalText;
	public Text collectableText;
	public Text timer;
	public Text timerBest;

	public Camera camera;

	LevelSaveData prevLevelProgress;

	protected override void OnStart () {
		prevLevelProgress = Director.SaveData.GetLevelSaveDataEntry (Director.Instance.LevelIndex.ToString ());
		if (prevLevelProgress != null) {
			TimeSpan timeSpan = TimeSpan.FromSeconds (prevLevelProgress.time);
			timerBest.text = string.Format ("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		} else {
			timerBest.text = String.Empty;
		}

		Director.GameEventManager.OnGameEvent += HandleGameEvent;
	}

	protected override void OnRemoving() {
		Director.GameEventManager.OnGameEvent -= HandleGameEvent;
	}

	void HandleGameEvent(GameEvent e) {
		switch (e.type) {
		case GameEventType.LevelCompleted:
			var tempDict = Director.SaveData.LevelProgress;
			float bestTime = 0;
			if (prevLevelProgress != null) {
				bestTime = Math.Min (prevLevelProgress.time, gameLogic.time);
			} else {
				bestTime = gameLogic.time;
			}
			tempDict[Director.Instance.LevelIndex.ToString()] = new LevelSaveData(true,gameLogic.CollectablesCollected,bestTime);
			Director.SaveData.LevelProgress = tempDict;

			Director.Instance.LevelIndex += 1;
			if (Director.Instance.LevelIndex >= Director.LevelDatabase.levels.Count) {
				SceneManager.LoadScene ("LevelSelectScene");
			} else {
				Director.TransitionManager.PlayTransition (() => {SceneManager.LoadScene ("LevelScene");},0.1f,Director.TransitionManager.FadeToBlack(),Director.TransitionManager.FadeOut());
			}
			break;
		}
	}

	// Update is called once per frame
	void Update () {
		goalText.text = gameLogic.CurrentColoredBlocks+"/"+gameLogic.coloredBlocksGoal;
		collectableText.text = gameLogic.CollectablesCollected+"/"+gameLogic.collectablesGoal;

		TimeSpan timeSpan = TimeSpan.FromSeconds(gameLogic.time);
		timer.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

		if (gameLogic.hero != null) {
			var distance = gameLogic.hero.transform.position - camera.transform.position;
			camera.transform.position += Time.deltaTime * 3 * new Vector3 (distance.x, distance.y, 0);
		}

		camera.transform.position = new Vector3(Mathf.Clamp(camera.transform.position.x,0,gameLogic.level.levelSize.x-20),Mathf.Clamp(camera.transform.position.y,Mathf.Min(-(gameLogic.level.levelSize.y-30),0),0),camera.transform.position.z);
	}
}
