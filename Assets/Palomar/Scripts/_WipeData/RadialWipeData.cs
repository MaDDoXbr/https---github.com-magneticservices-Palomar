using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RadialWipeData {
	public Image Img;
	public Vector3 EndRotation;
	public float StartTime;
	public DataText DataText;
	public int PercentageFinal;
	public DottedLine DottedLine;
	public Ease EaseType;
}
