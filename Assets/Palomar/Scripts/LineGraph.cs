using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LineGraph : MonoBehaviour
{
	public TextWipeData Subtitle, Subtitle2, Subtitle3;

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
			Subtitle.UIText.DOText(Subtitle.finalText, Subtitle.WipeDur)
				.SetEase(Subtitle.EaseType)
		);
		SeqInsert(ref subtitleSeq, Subtitle2.StartTime, Subtitle2, Subtitle2.WipeDur, Subtitle2.EaseType);
		SeqInsert(ref subtitleSeq, Subtitle3.StartTime, Subtitle3, Subtitle3.WipeDur, Subtitle3.EaseType);

	}

	public void SeqInsert(ref Sequence seq, float startTime, TextWipeData textData, float dur, Ease ease)
	{
		seq.Insert(
			startTime,
			textData.UIText.DOText(textData.finalText, textData.WipeDur)
				.SetEase(textData.EaseType)
		);
	}

	[System.Serializable]
	public class TextWipeData
	{
		public Text UIText;
		public String finalText;
		public float StartTime;
		public float WipeDur = 0f;
		public Ease EaseType;
	}
}
