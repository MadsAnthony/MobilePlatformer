using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMoving : DynamicBody {

	void Update () {
		Move (new Vector3((Mathf.Sin (transform.eulerAngles.z*Mathf.Deg2Rad))*-2,(Mathf.Cos (transform.eulerAngles.z*Mathf.Deg2Rad))*2,0));
	}

	public override void Hit (Piece hitPiece)
	{
	}

}
