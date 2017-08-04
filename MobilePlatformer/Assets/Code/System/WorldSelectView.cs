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
		Director.Instance.WorldIndex = -1;
		Director.Instance.LevelIndex = -1;
		if (Director.Instance.PrevLevelIndex == 17) {
			eyeCharacter.gameObject.SetActive (false);
			StartCoroutine (PlayEatAnimation (true));
		}
		pigButton.GetComponent<Button> ().onClick.AddListener(() => { 
			if (isPlayingEatAnimation) return;
			StartCoroutine (PlayEatAnimation());
		});
	}

	bool isPlayingEatAnimation;
	IEnumerator PlayEatAnimation(bool animateEyeCharacterOut = false) {
		
		isPlayingEatAnimation = true;
		pigCharacter.state.SetAnimation (0, "eat", false);
		pigCharacter.state.AddAnimation (0, "idle", true, 1.3f);
		StartCoroutine (AnimateEyeCharacter (animateEyeCharacterOut));
		yield return new WaitForSeconds(2);
		isPlayingEatAnimation = false;
		if (!animateEyeCharacterOut) {
			Director.Instance.WorldIndex = 1;
			Director.Instance.LevelIndex = 17;
			Director.TransitionManager.PlayTransition (() => { SceneManager.LoadSceneAsync ("LevelScene"); }, 0.1f, Director.TransitionManager.FadeToBlack (), Director.TransitionManager.FadeOut (0.2f));
		}
	}

	IEnumerator AnimateEyeCharacter(bool animateEyeCharacterOut) {
		yield return new WaitForSeconds(0.5f);
		float gravity = -28f;
		eyeCharacter.state.SetAnimation (0, "jump", false);
		if (animateEyeCharacterOut) {
			gravity = -22f;
			eyeCharacter.transform.localPosition = new Vector3 (5,130,eyeCharacter.transform.localPosition.z);
		}
		eyeCharacter.gameObject.SetActive (true);
		while (true) {
			gravity += 60 * Time.deltaTime;
			if (!animateEyeCharacterOut) {
				eyeCharacter.gameObject.transform.position += new Vector3 (-7, -gravity, 0) * Time.deltaTime;
				if (eyeCharacter.gameObject.transform.localPosition.x < 5) break;
			} else {
				eyeCharacter.gameObject.transform.position += new Vector3 (5, -gravity, 0) * Time.deltaTime;
				if (eyeCharacter.gameObject.transform.localPosition.y < -70) {
					eyeCharacter.state.SetAnimation (0, "idle", true);
					break;
				}
			}
			yield return null;
		}
		if (!animateEyeCharacterOut) {
			eyeCharacter.gameObject.SetActive (false);
		}
	}
}
