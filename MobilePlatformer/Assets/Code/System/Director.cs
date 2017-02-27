using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director {
	private static Director instance;

	public LevelDatabase levelDatabase;
	public int levelIndex = -1;
	private Director() {
		Application.targetFrameRate = 60;
		levelDatabase = Resources.Load ("LevelDatabase") as LevelDatabase;
	}

	public static Director Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new Director();
			}
			return instance;
		}
	}
}
