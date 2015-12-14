using DG.Tweening;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class FilledLineWipeData
{
	public FilledLineMesh Line;
	public float StartTime;
	public float Duration = 0f;
	public Ease EaseType;
}
