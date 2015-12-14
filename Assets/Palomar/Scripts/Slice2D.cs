using UnityEngine;
using System.Collections;

[System.Serializable]
public class Slice2D
{
	public Slice2D (Vector3 p1, Vector3 p2) {
		P1 = p1;
		P2 = p2;
	}
	public Vector3 P1;
	public Vector3 P2;
}
