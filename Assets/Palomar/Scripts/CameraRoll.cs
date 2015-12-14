using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class CameraRoll : MonoBehaviour {

	public float RotationTime;
	public Vector3 StartRotation, EndRotation;
	public Ease RotationEase;
	public Text GraphTxt;
	string EndTxt;
	bool RotateToEnd = false;

	void Awake() {
		DOTween.Init ();
		transform.rotation = Quaternion.Euler(StartRotation);
		if (!GraphTxt)
			return;
        EndTxt = GraphTxt.text;
        GraphTxt.text = "";
	}

	void Start () {
		NextRotation();
		if (!GraphTxt)
			return;
		GraphTxt.DOText(EndTxt, 0.75f, false).SetEase(Ease.Linear);
	}

	/// <summary> Will alternate from rotate-to-end and rotate-to-start on each cycle
	/// </summary>
	void NextRotation() {
		RotateToEnd = !RotateToEnd;
		transform.DORotate(( RotateToEnd? EndRotation:StartRotation ), RotationTime)
			.SetEase (RotationEase)
			.OnComplete(NextRotation);
	}

}
