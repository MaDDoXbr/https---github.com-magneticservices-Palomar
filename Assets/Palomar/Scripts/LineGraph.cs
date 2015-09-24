using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LineGraph : MonoBehaviour
{
	public TextWipeData Subtitle, Subtitle2, Subtitle3;
	public LineWipeData[] LineWipes;
	//public LineMesh[] Lines;

	void Awake()
	{
		DOTween.Init();
	}
	
	void Start () 
	{
		if (!Subtitle.UIText) return;

		Subtitle.UIText.text = "";
		Subtitle2.UIText.text = "";
		Subtitle3.UIText.text = "";

		Sequence subtitleSeq = DOTween.Sequence();
		subtitleSeq.Append(
			Subtitle.UIText.DOText(Subtitle.finalText, Subtitle.Duration)
				.SetEase(Subtitle.EaseType)
		);
		SeqInsert(ref subtitleSeq, Subtitle2);
		SeqInsert(ref subtitleSeq, Subtitle3);

		// Lines

		Sequence linesSeq = DOTween.Sequence ();
		LineWipes[0].Line.WipeAmount = 0f;
		LineWipes[1].Line.WipeAmount = 0f;
		LineWipes[2].Line.WipeAmount = 0f;

		SeqInsert(ref linesSeq, LineWipes[0]);
		SeqInsert (ref linesSeq, LineWipes[1]);
		SeqInsert (ref linesSeq, LineWipes[2]);


	}

	public void SeqInsert(ref Sequence seq, TextWipeData wipeData)
	{
		seq.Insert(
			wipeData.StartTime,
			wipeData.UIText.DOText(wipeData.finalText, wipeData.Duration)
				.SetEase(wipeData.EaseType)
		);
	}

	public void SeqInsert (ref Sequence seq, LineWipeData wipeData)
	{
		seq.Insert (
			wipeData.StartTime,
			DOTween.To (()=>wipeData.Line.WipeAmount, x => wipeData.Line.WipeAmount = x, 
						1f, wipeData.Duration).
						SetEase (wipeData.EaseType)
		);
	}

	[System.Serializable]
	public class TextWipeData
	{
		public Text UIText;
		public String finalText;
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
}
