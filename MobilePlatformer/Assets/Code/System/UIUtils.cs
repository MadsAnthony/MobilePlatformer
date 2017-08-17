﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIUtils : MonoBehaviour {
	public void GotoLevelSelectScene() {
		if (Director.Instance.LevelIndex == 0) {
			Director.TransitionManager.PlayTransition (() => {SceneManager.LoadScene ("WorldSelectScene");},0.1f,Director.TransitionManager.FadeToBlack(),Director.TransitionManager.FadeOut());
		} else {
			Director.Instance.LevelIndex = 0;
			Director.TransitionManager.PlayTransition (() => {SceneManager.LoadScene ("LevelScene");},0.1f,Director.TransitionManager.FadeToBlack(),Director.TransitionManager.FadeOut());
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
