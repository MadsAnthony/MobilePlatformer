using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.SceneManagement;

public class WorldSelectView : UIView {
	public GameObject pigButton;
	public SkeletonAnimation pigCharacter;
	public SkeletonAnimation eyeCharacter;
	protected override void OnStart () {
		pigButton.GetComponent<Button> ().onClick.AddListener(() => { 
			if (isPlayingEatAnimation) return;
			StartCoroutine (PlayEatAnimation());
		});
	}

	bool isPlayingEatAnimation;
	IEnumerator PlayEatAnimation() {
		isPlayingEatAnimation = true;
		pigCharacter.state.SetAnimation (0, "eat", false);
		pigCharacter.state.AddAnimation (0, "idle", true, 1.3f);
		StartCoroutine (AnimateEyeCharacter ());
		yield return new WaitForSeconds(2);
		isPlayingEatAnimation = false;
		Director.Instance.levelIndex = 0;
		Director.TransitionManager.PlayTransition (() => {SceneManager.LoadSceneAsync ("LevelScene");},0.1f,Director.TransitionManager.FadeToBlack(),Director.TransitionManager.FadeOut(0.2f));
	}

	IEnumerator AnimateEyeCharacter() {
		yield return new WaitForSeconds(0.5f);
		float gravity = -28f;
		eyeCharacter.state.SetAnimation (0, "jump", false);
		while (true) {
			eyeCharacter.gameObject.transform.position += new Vector3 (-7, -gravity, 0) * Time.deltaTime;
			gravity += 60 * Time.deltaTime;
			if (eyeCharacter.gameObject.transform.localPosition.x<5) break;
			yield return null;
		}
		eyeCharacter.gameObject.SetActive (false);
	}
}
