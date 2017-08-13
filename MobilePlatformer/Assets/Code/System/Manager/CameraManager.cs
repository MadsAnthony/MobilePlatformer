using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	Camera camera;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CameraShake() {
		camera = GameObject.Find ("BaseCamera").GetComponent<Camera>();

		StartCoroutine(Shake(camera));
	}

	IEnumerator Shake(Camera camera) {
		float x = 0;
		while (true) {
			x += 0.4f;
			transform.eulerAngles = new Vector3 (0,0,4*Mathf.Sin(x));
			if (x > 4) {
				break;
			}
			yield return null;
		}
		transform.eulerAngles = new Vector3 (0,0,0);
	}
}
