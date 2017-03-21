using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public abstract class Piece : MonoBehaviour {
	[HideInInspector]
	public PieceType Type;
	[HideInInspector]
	public bool IsPassable;
	[HideInInspector]
	public bool IsPushable;

	public abstract void Hit (Piece hitPiece);

	public void Destroy() {
		Director.GameEventManager.Emit(new GameEvent(GameEventType.PieceDestroyed, this));
		gameObject.SetActive(false);
	}

	private float gap = 0.01f;
	private Rigidbody rb;

	void EnsureRigidBody() {
		if (rb == null) {
			if (GetComponent<Rigidbody> () != null) {
				rb = GetComponent<Rigidbody> ();
			} else {
				rb = gameObject.AddComponent<Rigidbody> ();
				rb.isKinematic = true;
			}
		}
	}

	public Vector3 Move(Vector3 dir, Action<Piece,bool> callbackInterrupted = null, Action callbackFinished = null, bool useDeltaTime = true, Piece[] excludePieces = null, bool canDestroy = false) {
		EnsureRigidBody();

		Vector3 inputDir = dir * (useDeltaTime? Time.deltaTime : 1);
		Vector3 newDir = inputDir;
		bool wasInterrupted = false;
		bool newDirHasBeenSet = false;

		int i = 0;
		Vector3 tmpDir;
		var hits = rb.SweepTestAll (inputDir, inputDir.magnitude);
		List<RaycastHit> sortedHits = hits.ToList ();
		sortedHits.Sort ((x, y) => { 
			if(x.distance == y.distance) {
				return 0;
			} else if (x.distance < y.distance){
				return -1;
			}else {
				return 1;
			};							
		});
		foreach(var hit in sortedHits) {
			var piece = hit.collider.GetComponent<Piece> ();

			// ignore pieces that are part of excludePieces.
			if (excludePieces != null && excludePieces.Contains(piece)) continue;


			tmpDir = inputDir.normalized * (hit.distance - gap);

			// check if the rest of the hits are approximately at the same distance, if they are then they should be hit.
			if (i >0 && newDirHasBeenSet && !newDir.Equals (tmpDir)) continue;

			// if pushable has set newDir, do not hit pieces that are farther away (but do hit pieces before).
			if (i > 0 && tmpDir.magnitude > newDir.magnitude) continue;

			piece.Hit(this);
			if (!piece.IsPassable && !piece.IsPushable) {
				newDirHasBeenSet = true;
				newDir = tmpDir;

				// only call callbackInterrupted on the first hit
				if (i == 0 && callbackInterrupted != null) {
					callbackInterrupted (piece,false);
					wasInterrupted = true;
				}
			}
			if (piece.IsPushable) {
				bool shouldDestroy = false;
				newDir = tmpDir+piece.Move ((inputDir-tmpDir),(Piece p, bool wasPushing) => {if (!wasPushing) shouldDestroy = true;},null,false);
				if (shouldDestroy && canDestroy) {
					newDir = inputDir;
					piece.Destroy();
					continue;
				}

				if (i == 0 && callbackInterrupted != null) {
					callbackInterrupted (piece,true);
					wasInterrupted = true;
				}
			}
			i++;
		}
		this.transform.position += newDir;
		if (callbackFinished != null && !wasInterrupted) {
			callbackFinished ();
		}
		return newDir;
	}

	protected void Check(Vector3 dir, Action callbackInterrupted = null, Action callbackFinished = null) {
		EnsureRigidBody();

		Vector3 inputDir = dir * Time.deltaTime;
		Vector3 newDir = inputDir;
		RaycastHit hit;

		if (rb.SweepTest (inputDir, out hit, inputDir.magnitude)) {
			var piece = hit.collider.GetComponent<Piece> ();
			if (PieceDatabase.IsSticky(piece.Type)) {
				newDir = inputDir.normalized * (hit.distance - gap);

				if (callbackInterrupted != null) {
					callbackInterrupted ();
				}
			}
		}
		if (callbackFinished != null) {
			callbackFinished ();
		}
	}
}
