using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.SceneManagement;

public class WorldSelectView : UIView {
	public CustomButton leftButton;
	public CustomButton rightButton;
	public CustomButton world1Button;
	public CustomButton world2Button;
	public SkeletonAnimation pigCharacter;
	public SkeletonAnimation penguinCharacter;
	public SkeletonAnimation eyeCharacter;
	public CustomButton labButton;
	public SkeletonAnimation labBall;
	public GameObject labBallPivot;
	public CustomButton labButtonShrink;
	public GameObject characterPivot;
	public AnimationCurve switchingCharactersCurve;
	public AnimationCurve shrinkCurve;

	protected override void OnStart () {
		Director.Instance.LevelIndex = -1;
		if (Director.Instance.PrevLevelIndex == 0) {
			eyeCharacter.gameObject.SetActive (false);
			if (Director.Instance.WorldIndex == 1) {
				StartCoroutine (PlayEatAnimation (pigCharacter, true));
			}
			if (Director.Instance.WorldIndex == 2) {
				characterPivot.transform.position -= new Vector3 (-20, 0, 0);
				StartCoroutine (PlayEatAnimation (penguinCharacter, true));
			}
		}
		Director.Instance.WorldIndex = -1;

		world1Button.OnClick += (() => { 
			Director.Instance.WorldIndex = 1;
			if (isPlayingEatAnimation) return;
			StartCoroutine (PlayEatAnimation(pigCharacter));
		});
		world2Button.OnClick += (() => { 
			Director.Instance.WorldIndex = 2;
			if (isPlayingEatAnimation) return;
			StartCoroutine (PlayEatAnimation(penguinCharacter));
		});
		leftButton.OnClick += (() => {
			if (isPlayingSwitchCharacterAnimation || isPlayingEatAnimation) return;
			StartCoroutine (SwitchCharacter(characterPivot.transform, new Vector3(20,0,0),0.5f));
		});
		rightButton.OnClick += (() => {
			if (isPlayingSwitchCharacterAnimation || isPlayingEatAnimation) return;
			StartCoroutine (SwitchCharacter(characterPivot.transform, new Vector3(-20,0,0),0.5f));
		});

		labButton.OnClick += (() => { 
			labBallOpened = !labBallOpened;
			labBall.state.SetAnimation (0, labBallOpened? "Open" : "Animation", false);
		});
		labButtonShrink.OnClick += (() => {
			if (isShrinking) return;
			StartCoroutine (ShrinkBall(3));
		});

	}
	bool labBallOpened = true;

	bool isShrinking = false;
	IEnumerator ShrinkBall(float duration) {
		isShrinking = true;
		float t = 0;
		var startScale = labBallPivot.transform.localScale;
		var endScale = Vector3.one*0.15f;
		bool sucess = false;
		while (true) {
			if (Input.GetMouseButton (0)) {
				t += (1 / duration) * Time.deltaTime;
			} else {
				t -= 2*(1 / duration) * Time.deltaTime;
			}
			if (t < 0) {
				break;
			}
			var evalT = shrinkCurve.Evaluate (Mathf.Clamp01 (t));
			labBallPivot.transform.localScale = startScale*(1-evalT) + endScale*evalT;
			if (t>=1) {
				sucess = true;
				break;
			}

			yield return null;
		}
		if (sucess) {
			labBallPivot.SetActive (false);
			eyeCharacter.transform.position = labBallPivot.transform.position;
		}
		isShrinking = false;
	}

	bool isPlayingSwitchCharacterAnimation;
	IEnumerator SwitchCharacter(Transform transform, Vector3 movingVector, float duration) {
		isPlayingSwitchCharacterAnimation = true;
		float t = 0;
		Vector3 startPos = transform.position;
		while (true) {
			t += (1 / duration) * Time.deltaTime;
			transform.position = startPos+switchingCharactersCurve.Evaluate (Mathf.Clamp01 (t))*movingVector;
			if (t>=1) {
				break;
			}
			yield return null;
		}
		isPlayingSwitchCharacterAnimation = false;
	}

	bool isPlayingEatAnimation;
	IEnumerator PlayEatAnimation(SkeletonAnimation character, bool animateEyeCharacterOut = false) {
		
		isPlayingEatAnimation = true;
		character.state.SetAnimation (0, "eat", false);
		character.state.AddAnimation (0, "idle", true, 1.3f);
		StartCoroutine (AnimateEyeCharacter (animateEyeCharacterOut));
		yield return new WaitForSeconds(2);
		isPlayingEatAnimation = false;
		if (!animateEyeCharacterOut) {
			if (Director.SaveData.GetLevelSaveDataEntry ("1") != null) {
				Director.Instance.LevelIndex = 0;
			} else {
				Director.Instance.LevelIndex = 1;
			}
			Director.TransitionManager.PlayTransition (() => { SceneManager.LoadSceneAsync ("LevelScene"); }, 0.1f, Director.TransitionManager.FadeToBlack (), Director.TransitionManager.FadeOut (0.2f));
		}
	}

	IEnumerator AnimateEyeCharacter(bool animateEyeCharacterOut) {
		yield return new WaitForSeconds(0.5f);
		float gravity = -32f;
		eyeCharacter.state.SetAnimation (0, "jump", false);
		if (animateEyeCharacterOut) {
			gravity = -22f;
			eyeCharacter.transform.position = new Vector3 (0.25f,1.1f,eyeCharacter.transform.localPosition.z);
		}
		eyeCharacter.gameObject.SetActive (true);
		while (true) {
			gravity += 60 * Time.deltaTime;
			if (!animateEyeCharacterOut) {
				eyeCharacter.gameObject.transform.position += new Vector3 (-7, -gravity, 0) * Time.deltaTime;
				if (eyeCharacter.gameObject.transform.position.x < 0.5f) break;
			} else {
				eyeCharacter.gameObject.transform.position += new Vector3 (5.5f, -gravity, 0) * Time.deltaTime;
				if (eyeCharacter.gameObject.transform.position.y < -5.9f) {
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
