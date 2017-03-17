using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceGroup : MonoBehaviour {
	public List<Piece> pieces = new List<Piece> ();
	public PieceGroupData pieceGroupData;

	int moveIndex = 0;
	float t = 0;
	Vector3 pos = new Vector3(0,0,0);

	void Start() {
		StartCoroutine (MoveGroup ());
	}

	IEnumerator MoveGroup () {
		while (true) {
			if (pieces.Count <= 0) yield return null;
			GroupMovement groupMovement = pieceGroupData.moves [moveIndex % pieceGroupData.moves.Count];

			Vector3 dir = groupMovement.endPoint - groupMovement.startPoint;
			dir = new Vector3 (dir.x, -dir.y, dir.z);

			if (t == 0) {
				yield return new WaitForSeconds (groupMovement.delay);
				pos = Vector3.zero;
			}

			if (groupMovement.time > 0) {
				t += (1 / ((groupMovement.time))) * Time.deltaTime;
			} else {
				t = 1;
			}


			float evalT = groupMovement.animationCurve.Evaluate (t);
			Vector3 dirEvalT = (dir * evalT);

			foreach (Piece piece  in pieces) {
				if ((DynamicBody)piece != null) {
					((DynamicBody)piece).Move ((dirEvalT - pos), null, null, false, pieces.ToArray ());
				}
			}

			pos = dirEvalT;
			if (groupMovement.maxT > 0 && t >= groupMovement.maxT) {
				t = 0;
				moveIndex++;
			}
			yield return null;
		}
	}
}
