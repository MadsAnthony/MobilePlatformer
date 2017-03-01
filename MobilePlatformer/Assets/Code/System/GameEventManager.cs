using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager {

	public delegate void GameEventHandler(GameEvent e);
	public GameEventHandler OnGameEvent;
	private GameEvent reusableEvent = new GameEvent();

	public void Emit(GameEvent e) {
		if (OnGameEvent != null) {
			OnGameEvent(e);
		}
	}

	public void Emit(GameEventType type) {
		reusableEvent.type = type;
		Emit(reusableEvent);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class GameEvent {
	public GameEventType type;
}

public enum GameEventType {
	BlockColored,
	LevelCompleted
}
