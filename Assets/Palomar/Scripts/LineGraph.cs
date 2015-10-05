using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LineGraph : MonoBehaviour
{
	public TextAsset XmlAsset;
	public LineGraphParser Parser; 
	public TextWipeData[] Subtitles;
	public LineWipeData[] LineWipes;
	public RulerWipeData[] RulerWipes;
	public BGWipeData BGWipe;
	public Transform Pivot;
	[HideInInspector]public RotationData Rot;

	public void Awake ()
	{
		DOTween.Init();
	}
	
	public void Start () 
	{
		WipeSubtitles();
		WipeLines();
		WipeRulers();
		WipeBG();
		Pivot.rotation = Quaternion.Euler(Rot.StartRotation);
		Pivot.DORotate (Rot.EndRotation, Rot.Duration).SetEase (Rot.EaseType);
	}

	public void ImportXML() {
		Parser = LineGraphParser.Load(XmlAsset);
		Debug.Log(Parser.Subtitle1.Text);
		Debug.Log(Parser.VerticalRuler.StepCount);
	}

	private void WipeLines() 
	{
		var linesSeq = DOTween.Sequence();
		foreach (var wipe in LineWipes) {
			if (wipe == null || wipe.Line == null) 
				continue;
			wipe.Line.WipeAmount = 0f;
			wipe.Line.DrawLine();
			SeqInsert(ref linesSeq, wipe);
		}
	}

	private void WipeRulers() 
	{
		var rulerSeq = DOTween.Sequence();
		foreach (var wipe in RulerWipes) {
			if (wipe == null || wipe.Ruler == null)
				continue;
			wipe.Ruler.WipeAmount = 0f;
			//wipe.Line.DrawLine ();
			SeqInsert (ref rulerSeq, wipe);
		}
	}

	private void WipeSubtitles() 
	{
		var subtitleSeq = DOTween.Sequence();
		foreach (var subtitle in Subtitles) {
			if (subtitle == null) 
				continue;
			subtitle.UIText.text = "";
			SeqInsert(ref subtitleSeq, subtitle);
		}
	}

	private void WipeBG() 
	{
		BGWipe.Lines.DefinePoints();
		BGWipe.Lines.ZeroOutLinesLength();
		var BgSeq = DOTween.Sequence ();

		for (int i = 0; i < BGWipe.Lines.VertexCount; i++) {
			// only odd vertices (line ends) must be tweened
			if (i%2 != 0)
				SeqInsert (ref BgSeq, BGWipe, i);
		}
	}

	public void SeqInsert(ref Sequence seq, TextWipeData wipeData)
	{
		seq.Insert (
			wipeData.StartTime,
			wipeData.UIText.DOText(wipeData.FinalText, wipeData.Duration)
				.SetEase(wipeData.EaseType)
		);
	}

	public void SeqInsert (ref Sequence seq, LineWipeData wipeData)
	{
		seq.Insert (
			wipeData.StartTime,
			DOTween.To (()=>wipeData.Line.WipeAmount, 
						x => wipeData.Line.WipeAmount = x, 
						1f, wipeData.Duration).
						SetEase (wipeData.EaseType)
		);
	}

	public void SeqInsert (ref Sequence seq, RulerWipeData wipeData)
	{
		seq.Insert (
			wipeData.StartTime,
			DOTween.To (() => wipeData.Ruler.WipeAmount,
						x => wipeData.Ruler.WipeAmount = x,
						1f, wipeData.Duration).
						SetEase (wipeData.EaseType)
		);
	}

	public void SeqInsert (ref Sequence seq, BGWipeData wipeData, int i)
	{
		//Debug.Log(" inserted");
		seq.Insert (
			wipeData.StartTime + i*wipeData.StepDelay,
			DOTween.To (() => wipeData.Lines.Points[i],
						x => wipeData.Lines.Points[i] = x,
						wipeData.Lines.FinalPoints[i], wipeData.Duration).
						SetEase (wipeData.EaseType)
						.OnUpdate(UpdateMultiLine)
						);
	}

	public void UpdateMultiLine() {
		BGWipe.Lines.DrawLines();
	}

	[Serializable]
	public class TextWipeData
	{
		public Text UIText;
		public String FinalText;
		public float StartTime;
		public float Duration = 0f;
		public Ease EaseType;
	}

	[Serializable]
	public class LineWipeData
	{
		public LineMesh Line;
		public float StartTime;
		public float Duration = 0f;
		public Ease EaseType;
	}

	[Serializable]
	public class RulerWipeData
	{
		public ParallelLines Ruler;
		public float StartTime;
		public float Duration = 0f;
		public Ease EaseType;
	}

	[Serializable]
	public class BGWipeData
	{
		public MultiLines Lines;
		public float StartTime;
		public float Duration = 0f;
		public float StepDelay = 0f;
		public Ease EaseType;
	}

	[Serializable]
	public class RotationData
	{
		public Vector3 StartRotation;
		public Vector3 EndRotation;
		public Ease EaseType;
		public float Duration;
	}

}
