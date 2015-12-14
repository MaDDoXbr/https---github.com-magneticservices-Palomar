using DG.Tweening;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class DottedLineWipeData
{
	public DataText Data;
	public DottedLine DottedLine;
	public RectTransform CircleTransform;
	public float CircleEndScale;			//will be applied to X and Y
	public float StartTime;
	public float Duration = 1f;
	//		public RectTransform TextTransform;
	public Ease EaseType;
}

