using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LineGraph : MonoBehaviour
{
	public TextWipeData[] Subtitles;
	public LineWipeData[] LineWipes;
	public RulerWipeData[] RulerWipes;
	public BGWipeData BGWipe;

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

	private void WipeRulers() {
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

	private void WipeBG() {
		BGWipe.Lines.ZeroOutLinesLength();
		var BgSeq = DOTween.Sequence ();

		for (int i = 0; i < BGWipe.Lines.LineCount - 1; i++) {
			// only odd elements (line ends) must be tweened
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
		seq.Insert (
			wipeData.StartTime + i*wipeData.StepDelay,
			DOTween.To (() => wipeData.Lines.Points[i],
						x => wipeData.Lines.Points[i] = x,
						wipeData.Lines.FinalPoints[i], wipeData.Duration).
						SetEase (wipeData.EaseType)
			// TODO: Everytime points is set, redraw
			//DOTween.To(()=> myVector, x=> myVector = x, new Vector3(3,4,8), 1);
		);
	}

	[System.Serializable]
	public class TextWipeData
	{
		public Text UIText;
		public String FinalText;
		public float StartTime;
		public float Duration = 0f;
		public Ease EaseType;
	}

	[System.Serializable]
	public class LineWipeData
	{
		public LineMesh Line;
		public float StartTime;
		public float Duration = 0f;
		public Ease EaseType;
	}

	[System.Serializable]
	public class RulerWipeData
	{
		public ParallelLines Ruler;
		public float StartTime;
		public float Duration = 0f;
		public Ease EaseType;
	}

	[System.Serializable]
	public class BGWipeData
	{
		public MultiLines Lines;
		public float StartTime;
		public float Duration = 0f;
		public float StepDelay = 0f;
		public Ease EaseType;
	}

}
