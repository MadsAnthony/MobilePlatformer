using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour  {
	private static Director instance;

	[SerializeField] private LevelDatabase levelDatabase;
	public int levelIndex = -1;

	private GameEventManager gameEventManager;

	public static GameEventManager GameEventManager {get {return Instance.gameEventManager;}}
	public static LevelDatabase    LevelDatabase 	{get {return Instance.levelDatabase;}}

	public static Director Instance
	{
		get 
		{
			if (instance == null)
			{
				var asset = (Director)Resources.Load ("Director", typeof(Director));
				instance = (Director)GameObject.Instantiate (asset);
				instance.Load();
			}
			return instance;
		}
	}
		
	void Load() {
		gameEventManager = new GameEventManager();
	}

	void Start () {
		DontDestroyOnLoad (transform.gameObject);
		Application.targetFrameRate = 60;
	}
	public static void CameraShake() {
		GameObject.Find ("Main Camera").GetComponent<CameraManager>().CameraShake();
	}
}
