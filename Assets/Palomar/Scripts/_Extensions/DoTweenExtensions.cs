using DG.Tweening;
using UnityEngine;
using System.Collections;

public static class DoTweenExtensions {

	public static Sequence SeqInsert (this Sequence seq, TextWipeData wipeData)
	{
		wipeData.UIText.fontSize = wipeData.FontSize;
		seq.Insert (
			wipeData.StartTime,
			wipeData.UIText.DOText (wipeData.FinalText, wipeData.Duration)
				.SetEase (wipeData.EaseType)
		);
		return seq;
	}

	public static Sequence SeqInsert (this Sequence seq, LineWipeData wipeData)
	{
		seq.Insert (
			wipeData.StartTime,
			DOTween.To (() => wipeData.Line.WipeAmount,
						x => wipeData.Line.WipeAmount = x,
						1f, wipeData.Duration).
						SetEase (wipeData.EaseType)
		);
		return seq;
	}

	public static Sequence SeqInsert (this Sequence seq, RulerWipeData wipeData)
	{
		seq.Insert (
			wipeData.StartTime,
			DOTween.To (() => wipeData.Ruler.WipeAmount,
						x => wipeData.Ruler.WipeAmount = x,
						1f, wipeData.Duration).
						SetEase (wipeData.EaseType)
		);
		return seq;
	}

	public static Sequence SeqInsert (this Sequence seq, BGWipeData wipeData, int i)
	{
		seq.Insert (
			wipeData.StartTime + (i * wipeData.StepDelay),
			DOTween.To (() => wipeData.MultiLines.Points[i],
						x => wipeData.MultiLines.Points[i] = x,
						wipeData.MultiLines.FinalPoints[i], wipeData.Duration).
						SetEase (wipeData.EaseType)
						.OnUpdate (() => wipeData.MultiLines.DrawLines ())
						);
		return seq;
	}

}
