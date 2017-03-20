using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Piece {
	float gravity;
	void Update () {
		gravity -= 1f;
		gravity = Mathf.Clamp (gravity,-10,10);
		Move (new Vector3((Mathf.Sin (transform.eulerAngles.z*Mathf.Deg2Rad))*-2,(Mathf.Cos (transform.eulerAngles.z*Mathf.Deg2Rad))*gravity,0));
		Move (new Vector3 (0, gravity, 0));
	}

	public override void Hit (Piece hitPiece)
	{
	}

}
