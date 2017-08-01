﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIUtils : MonoBehaviour {
	public void GotoLevelSelectScene() {
		SceneManager.LoadScene ("LevelSelectScene");
	}

	public void GotoWorldSelectScene() {
		SceneManager.LoadScene ("WorldSelectScene");
	}

	public static void GotoLevelScene(int i) {
		Director.Instance.levelIndex = i;
		SceneManager.LoadScene ("LevelScene");
	}

	public void RestartLevel() {
		SceneManager.LoadScene ("LevelScene");
	}
}
