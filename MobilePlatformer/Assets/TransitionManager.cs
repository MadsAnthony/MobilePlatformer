using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour {
	[SerializeField] Image overlay;
	[SerializeField] Camera camera;

	void Start () {
		DontDestroyOnLoad (transform.gameObject);
		camera.transform.gameObject.SetActive(false);
	}

	public void PlayTransition(Action callInBetween = null, float waitTime = 0) {
		StartCoroutine(PlayTransitionCr(callInBetween, waitTime));
	}

	IEnumerator PlayTransitionCr(Action callInBetween, float waitTime) {
		camera.transform.gameObject.SetActive (true);
		yield return FadeToBlack();
		yield return new WaitForSeconds(waitTime);
		if (callInBetween != null) {
			callInBetween ();
		}
		yield return FadeOut();
		camera.transform.gameObject.SetActive(false);
	}

	IEnumerator FadeToBlack() {
		yield return Fade(overlay.color,new Color(0,0,0,1));
	}

	IEnumerator FadeOut() {
		yield return Fade(overlay.color,new Color(overlay.color.r,overlay.color.g,overlay.color.b,0));
	}

	IEnumerator Fade(Color startColor, Color endColor) {
		float t = 0;
		while (true) {
			t += 2f * Time.deltaTime;
			overlay.color = Color.Lerp (startColor,endColor,t);
			if (t > 1) {
				break;
			}
			yield return null;
		}
	}
}

public enum TransitionTypes {
	//Transtion in
	FadeToBlack,

	//Transition out
	FadeOut,
}
