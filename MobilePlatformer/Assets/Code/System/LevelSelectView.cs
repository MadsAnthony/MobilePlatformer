using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectView : UIView {
	public GameObject levelButton;
	protected override void OnStart () {
		var rowLength = 5;
		int i = 0;
		foreach(var level in Director.LevelDatabase.levels) {
			var levelButtonGo = Instantiate (levelButton);
			levelButtonGo.transform.parent = transform;
			levelButtonGo.transform.localScale = new Vector3 (1,1,1);
			levelButtonGo.transform.localPosition = new Vector3 (-200+i%rowLength*100,400-(i/rowLength)*100,0);

			int capturedIndex = i;
			levelButtonGo.GetComponent<Button> ().onClick.AddListener(() => { UIUtils.GotoLevelScene(capturedIndex);});
			levelButtonGo.GetComponentInChildren<Text> ().text = (i+1).ToString();
			i++;
		}
	}
}
