using DG.Tweening;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class RotationData
{
	public Vector3 StartRotation;
	public Vector3 EndRotation;
	public Ease EaseType;
	public float Duration;
}
