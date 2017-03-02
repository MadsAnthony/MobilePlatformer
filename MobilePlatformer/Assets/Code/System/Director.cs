using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour  {
	private static Director instance;

	[SerializeField] private LevelDatabase levelDatabase;
	[SerializeField] private SoundDatabase soundDatabase;

	public int levelIndex = -1;

	private GameEventManager		gameEventManager;
	private UIManager 		 		uiManager;
	private TransitionManager		transitionManager;
	private SoundManager			sounds;

	public static GameEventManager 		GameEventManager 	{get {return Instance.gameEventManager;}}
	public static LevelDatabase    		LevelDatabase 		{get {return Instance.levelDatabase;}}
	public static UIManager    	   		UIManager			{get {return Instance.uiManager;}}
	public static TransitionManager		TransitionManager 	{get {return Instance.transitionManager;}}
	public static SoundManager			Sounds 				{get {return Instance.sounds;}}

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
		gameEventManager  = new GameEventManager();
		uiManager 		  = new UIManager();
		transitionManager = SetupTransitionManager();
	}

	void Start () {
		DontDestroyOnLoad (transform.gameObject);
		Application.targetFrameRate = 60;
	}

	TransitionManager SetupTransitionManager() {
		var asset = (TransitionManager)Resources.Load ("TransitionManager", typeof(TransitionManager));
		return (TransitionManager)GameObject.Instantiate (asset);
	}

	public static void CameraShake() {
		GameObject.Find ("Main Camera").GetComponent<CameraManager>().CameraShake();
	}
}
