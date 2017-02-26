using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
	public void GotoMenuScene() {
		SceneManager.LoadScene ("IntroScene");
	}

	public void GotoLevelScene(int i) {
		Director.Instance.levelIndex = i;
		SceneManager.LoadScene (0);
	}
}
