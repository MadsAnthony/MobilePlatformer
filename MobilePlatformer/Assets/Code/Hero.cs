using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
	float speed = 10;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.RightArrow)) {
			this.transform.position += new Vector3(speed,0,0)*Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			this.transform.position -= new Vector3(speed,0,0)*Time.deltaTime;
		}
	}
}
