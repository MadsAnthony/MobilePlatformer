﻿using System.Collections;
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
	public SkeletonAnimation eyeCharacter;
	public GameObject characterPivot;
	public AnimationCurve switchingCharactersCurve;

	protected override void OnStart () {
		Director.Instance.WorldIndex = -1;
		Director.Instance.LevelIndex = -1;
		if (Director.Instance.PrevLevelIndex == 0) {
			eyeCharacter.gameObject.SetActive (false);
			StartCoroutine (PlayEatAnimation (true));
		}
		world1Button.OnClick += (() => { 
			Director.Instance.WorldIndex = 1;
			if (isPlayingEatAnimation) return;
			StartCoroutine (PlayEatAnimation());
		});
		world2Button.OnClick += (() => { 
			Director.Instance.WorldIndex = 2;
			if (isPlayingEatAnimation) return;
			StartCoroutine (PlayEatAnimation());
		});
		leftButton.OnClick += (() => {
			if (isPlayingSwitchCharacterAnimation || isPlayingEatAnimation) return;
			StartCoroutine (SwitchCharacter(characterPivot.transform, new Vector3(20,0,0),0.5f));
		});
		rightButton.OnClick += (() => {
			if (isPlayingSwitchCharacterAnimation || isPlayingEatAnimation) return;
			StartCoroutine (SwitchCharacter(characterPivot.transform, new Vector3(-20,0,0),0.5f));
		});
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
	IEnumerator PlayEatAnimation(bool animateEyeCharacterOut = false) {
		
		isPlayingEatAnimation = true;
		pigCharacter.state.SetAnimation (0, "eat", false);
		pigCharacter.state.AddAnimation (0, "idle", true, 1.3f);
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
