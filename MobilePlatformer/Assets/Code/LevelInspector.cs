using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelAsset))]
public class LevelInspector : Editor {

	private Texture2D cellTexture;
	private Vector2 selectedIndex;

	private Vector2 gridStartPos = new Vector2 (100,200);
	private float cellSize = 20;
	int cellType = 1;

	public override void OnInspectorGUI()
	{
		cellTexture = AssetDatabase.LoadAssetAtPath("Assets/Textures/squareWithBorder.png", typeof(Texture2D)) as Texture2D;

		LevelAsset myTarget = (LevelAsset)target;
		myTarget.someInt = EditorGUILayout.IntField ("Experience", myTarget.someInt);
		EditorGUILayout.LabelField ("Level", myTarget.levelName);

		if (GUILayout.Button ("Play")) {
			EditorApplication.isPlaying = false;
			EditorSceneManager.OpenScene ("Assets/Scenes/LevelTest.unity");
			var LevelInit = GameObject.Find ("LevelInit").GetComponent<LevelInit> ();
			LevelInit.level = myTarget;
			EditorApplication.isPlaying = true;
		}
		if (GUILayout.Button ("Clear")) {
			myTarget.gridObjects.Clear();
		}
			
		string[] cellOptions = new string[]
		{
			"Hero", "Block"
		};
		cellType = EditorGUILayout.Popup("Label", cellType, cellOptions); 

		//Event.current.mousePosition;
		if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) {
			GetClosestCell (Event.current.mousePosition);
		}
		if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) {
			if (Event.current.button == 0) {
				if (cellType == 0) {
					myTarget.heroPos = selectedIndex;
				}
				if (cellType == 1) {
					if (!myTarget.gridObjects.Contains (selectedIndex)) {
						myTarget.gridObjects.Add (selectedIndex);
					}
				}
			}
			if (Event.current.button == 1) {
				myTarget.gridObjects.Remove(selectedIndex);
			}
			EditorUtility.SetDirty (myTarget);
		}

		DrawGrid ();

		if (GUI.changed) {
			EditorUtility.SetDirty (myTarget);
		}
	}

	Vector2 GetClosestCell(Vector2 mousePos) {
		mousePos -= gridStartPos;
		selectedIndex = new Vector2(Mathf.FloorToInt(mousePos.x/cellSize),Mathf.FloorToInt(mousePos.y/cellSize));
		return selectedIndex;
	}

	void DrawGrid() {
		for (int x = 0; x<Level.GRIDSIZE.x;x++) {
			for (int y = 0; y<Level.GRIDSIZE.y;y++) {
				var rect = new Rect (gridStartPos.x+x*cellSize, gridStartPos.y+y*cellSize, cellSize, cellSize);

				GUI.color = Color.white;
				foreach(Vector2 gridObject in ((LevelAsset)target).gridObjects) {
					if (x == gridObject.x && y == gridObject.y) {
						GUI.color = Color.black;
						break;
					}
				}
				if (x ==  ((LevelAsset)target).heroPos.x && y == ((LevelAsset)target).heroPos.y) {
					GUI.color = Color.green;
				}
				GUI.DrawTexture(rect,cellTexture,ScaleMode.ScaleToFit);
			}
		}
	}
}
#endif