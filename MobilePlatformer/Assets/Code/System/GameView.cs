using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameView : MonoBehaviour {
	public GameLogic gameLogic;
	public Text goalText;
	public Image overlay;
	// Use this for initialization
	void Start () {
		StartCoroutine (FadeFromBlack ());
		Director.GameEventManager.OnGameEvent += HandleGameEvent;
	}

	void OnDestroy() {
		Director.GameEventManager.OnGameEvent -= HandleGameEvent;
	}

	void HandleGameEvent(GameEvent e) {
		switch (e.type) {
		case GameEventType.LevelCompleted:
			Director.Instance.levelIndex += 1;
			if (Director.Instance.levelIndex >= Director.LevelDatabase.levels.Count) {
				StartCoroutine (LoadNextScene("IntroScene"));
			} else {
				StartCoroutine (LoadNextScene("LevelScene"));
			}
			break;
		}
	}

	IEnumerator FadeToBlack() {
		overlay.transform.position = new Vector3 (0,0,0);
		float alpha = 0;
		while (true) {
			alpha += 2f * Time.deltaTime;
			overlay.color = new Color(overlay.color.r,overlay.color.g,overlay.color.b,alpha);
			if (alpha >= 1) {
				break;
			}
			yield return null;
		}
	}
	IEnumerator FadeFromBlack() {
		overlay.transform.position = new Vector3 (0,0,0);
		float alpha = 1;
		while (true) {
			alpha -= 2f * Time.deltaTime;
			overlay.color = new Color(overlay.color.r,overlay.color.g,overlay.color.b,alpha);
			if (alpha <= 0) {
				break;
			}
			yield return null;
		}
		overlay.transform.position = new Vector3 (1000,0,0);
	}

	IEnumerator LoadNextScene(string sceneName) {
		yield return FadeToBlack ();
		yield return new WaitForSeconds(0.1f);
		SceneManager.LoadScene (sceneName);
	}

	// Update is called once per frame
	void Update () {
		goalText.text = gameLogic.CurrentColoredBlocks+"/"+gameLogic.coloredBlocksGoal;
	}
}
