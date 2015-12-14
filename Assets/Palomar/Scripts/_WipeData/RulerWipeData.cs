using DG.Tweening;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class RulerWipeData
{
	public ParallelLines Ruler;
	public float StartTime;
	public float Duration = 0f;
	public Ease EaseType;
}
