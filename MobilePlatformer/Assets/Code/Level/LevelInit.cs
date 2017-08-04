using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInit : MonoBehaviour {
	public LevelAsset level;

	public static Vector3 LevelStartPos = new Vector3(-9.5f,16,0);

	private GameLogic gameLogic;

	private Dictionary<string, Piece> pieces = new Dictionary<string, Piece>();

	private Dictionary<int, Piece> levelDoors = new Dictionary<int,Piece>();
	// Use this for initialization
	void Start () {
		gameLogic = GetComponent<GameLogic>();
		if (Director.Instance.LevelIndex >= 0) {
			level = Director.LevelDatabase.levels[Director.Instance.LevelIndex];
		}
		gameLogic.level = level;

		InitializeLevel();
	}

	void InitializeLevel() {
		int i = 0;

		foreach (var piece in level.pieces) {
			PieceData pieceData = Director.PieceDatabase.GetPieceData (piece.type);
			var tmpPiece = Instantiate(pieceData.prefab);
			tmpPiece.transform.eulerAngles = new Vector3(tmpPiece.transform.eulerAngles.x,tmpPiece.transform.eulerAngles.y,((int)piece.dir)*-90);
			tmpPiece.transform.position = new Vector3(piece.pos.x,-piece.pos.y,0)+LevelInit.LevelStartPos;

			tmpPiece.name = "Piece"+i;

			tmpPiece.Init(piece, gameLogic);

			if (piece.type == PieceType.Collectable) {
				gameLogic.collectablesGoal++;
			}
			if (piece.type == PieceType.Hero) {
				gameLogic.hero = tmpPiece.GetComponent<Hero> ();
			}
			if (piece.type == PieceType.LevelDoor) {
				int levelIndex = piece.GetSpecificData<LevelDoorPieceLevelData> ().levelIndex;
				levelDoors.Add (levelIndex,tmpPiece);
			}

			// Remove this condition at some time.
			if (!string.IsNullOrEmpty(piece.id)) {
				pieces.Add (piece.id, tmpPiece);
			}
			i++;
		}

		if (Director.Instance.LevelIndex == 17 && Director.Instance.PrevLevelIndex != -1) {
			Piece levelDoorPiece;
			if (levelDoors.TryGetValue (Director.Instance.PrevLevelIndex, out levelDoorPiece)) {
				gameLogic.hero.transform.position = levelDoors [Director.Instance.PrevLevelIndex].transform.position;
			}
		}
		var camera = ((GameView)Director.UIManager.ActiveView).camera;
		camera.transform.position = new Vector3(gameLogic.hero.transform.position.x,gameLogic.hero.transform.position.y,camera.transform.position.z);


		foreach (PieceGroupData pieceGroup in level.pieceGroups) {
			var go = new GameObject();
			PieceGroup goPieceGroup = go.AddComponent<PieceGroup> ();
			goPieceGroup.pieceGroupData = pieceGroup;
			foreach (var pieceId in pieceGroup.pieceIds) {
				goPieceGroup.pieces.Add(pieces [pieceId]);
			}
		}

		var mesh = new Mesh ();

		var verticesList  = new List<Vector3> ();
		var trianglesList = new List<int> ();
		var uvsList = new List<Vector2> ();
		int index = 0;
		var bgUVSize = 8;
		foreach (Vector2 bgPos in level.backgroundList) {
			int id = index * 4;
			var vertices = new Vector3[] { new Vector3 (-0.5f+bgPos.x, -0.5f-bgPos.y, 0), new Vector3 (0.5f+bgPos.x, -0.5f-bgPos.y, 0), new Vector3 (0.5f+bgPos.x, 0.5f-bgPos.y, 0), new Vector3 (-0.5f+bgPos.x, 0.5f-bgPos.y, 0)  };
			var triangles = new int[] { id, id+1, id+2, id+2, id+3, id };


			float posXMod = bgPos.x % bgUVSize;
			float posYMod = -bgPos.y % bgUVSize;
			var uvs = new Vector2[] { new Vector2(posXMod/bgUVSize,posYMod/bgUVSize), new Vector2((posXMod+1)/bgUVSize,posYMod/bgUVSize), new Vector2((posXMod+1)/bgUVSize,(posYMod+1)/bgUVSize), new Vector2(posXMod/bgUVSize,(posYMod+1)/bgUVSize)};

			verticesList.AddRange (vertices);
			trianglesList.AddRange (triangles);
			uvsList.AddRange (uvs);
			index++;
		}
		mesh.vertices = verticesList.ToArray();
		mesh.triangles = trianglesList.ToArray();
		mesh.uv = uvsList.ToArray();

		var background = new GameObject ("background");
		background.transform.position = new Vector3(LevelInit.LevelStartPos.x,LevelInit.LevelStartPos.y,10);
		background.transform.eulerAngles = new Vector3 (0,180,0);
		background.transform.localScale  = new Vector3 (-1,1,0);
		background.AddComponent<MeshRenderer> ();
		background.AddComponent<MeshFilter> ();

		background.GetComponent<MeshFilter>().mesh = mesh;
		Material material = (Material)Resources.Load("BackgroundMaterial");
		background.GetComponent<MeshRenderer> ().material = material;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.R)) {
			SceneManager.LoadScene ("LevelScene");
		}
	}
}
