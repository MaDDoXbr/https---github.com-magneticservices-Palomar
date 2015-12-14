using DG.Tweening;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class LineWipeData
{
	public LineMesh Line;
	public float StartTime;
	public float Duration = 0f;
	public Ease EaseType;
}
