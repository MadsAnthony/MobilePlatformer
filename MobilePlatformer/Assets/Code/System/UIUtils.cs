using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIUtils : MonoBehaviour {
	public void GotoLevelSelectScene() {
		if (Director.Instance.LevelIndex == 17) {
			SceneManager.LoadScene ("WorldSelectScene");
		} else {
			Director.Instance.LevelIndex = 17;
			SceneManager.LoadScene ("LevelScene");
		}
	}

	public void GotoWorldSelectScene() {
		SceneManager.LoadScene ("WorldSelectScene");
	}

	public static void GotoLevelScene(int i) {
		Director.Instance.LevelIndex = i;
		SceneManager.LoadScene ("LevelScene");
	}

	public void RestartLevel() {
		SceneManager.LoadScene ("LevelScene");
	}
}
