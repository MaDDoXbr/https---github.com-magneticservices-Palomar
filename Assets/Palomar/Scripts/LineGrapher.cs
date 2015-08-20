using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LineGrapher : MonoBehaviour {

	public WipeData BGLines, Ruler, Line1, Line2, Line3, Title1, Title2, Title3;
	public float RotationTime;
	public Vector3 EndRotation;
	public Ease RotationEase;

	void Awake() {
		DOTween.Init ();
	}

	void Start () {
		transform.DORotate(EndRotation, RotationTime).SetEase (RotationEase);

		Sequence mySeq = DOTween.Sequence();
		mySeq.Append (DOTween.To (()=>BGLines.Img.fillAmount, 
		                          x => BGLines.Img.fillAmount = x, 1, BGLines.WipeDur).
		              SetEase (BGLines.EaseType));
//		mySeq.Append (DOTween.To (()=>Ruler.Img.fillAmount, 
//		            x => Ruler.Img.fillAmount = x, 1, Ruler.WipeDur).
//		            SetEase (Ruler.EaseType));
		SeqInsert (ref mySeq, Ruler.StartTime, Ruler.Img, Ruler.WipeDur, Ruler.EaseType);

		SeqInsert (ref mySeq, Line1.StartTime, Line1.Img, Line1.WipeDur, Line1.EaseType);
		SeqInsert (ref mySeq, Line2.StartTime, Line2.Img, Line2.WipeDur, Line2.EaseType);
		SeqInsert (ref mySeq, Line3.StartTime, Line3.Img, Line3.WipeDur, Line3.EaseType);

		SeqInsert (ref mySeq, Title1.StartTime, Title1.Img, Title1.WipeDur, Title1.EaseType);
		SeqInsert (ref mySeq, Title2.StartTime, Title2.Img, Title2.WipeDur, Title2.EaseType);
		SeqInsert (ref mySeq, Title3.StartTime, Title3.Img, Title3.WipeDur, Title3.EaseType);

		// mySeq.Join <=> mySeq.Inset (0,*)
//		mySeq.Join (DOTween.To (()=>Ld.Line1.fillAmount, 
//		                        x => Ld.Line1.fillAmount = x, 1, Ld.Line1WipeDur).
//		            SetEase(Ld.LineEaseType).
//		            SetDelay(Ld.Line1Delay));
			         
//		mySeq.Join (DOTween.To (()=>Ld.Line2.fillAmount, 
//		                        x => Ld.Line2.fillAmount = x, 1, Ld.Line2WipeDur).
//		            SetEase(Ld.LineEaseType).
//		            SetDelay(Ld.Line2Delay));
//		
//		mySeq.Join (DOTween.To (()=>Ld.Line3.fillAmount, 
//		                        x => Ld.Line3.fillAmount = x, 1, Ld.Line3WipeDur).
//						SetEase(Ld.LineEaseType).
//		            SetDelay(Ld.Line3Delay));
		//Ruler.DOFillAmount(1,2);
	}

	public void SeqInsert (ref Sequence seq, float startTime, Image img, float dur, Ease ease) {
		seq.Insert (startTime, DOTween.To (()=>img.fillAmount, 
		                        x => img.fillAmount = x, 1, dur).
		            			SetEase(ease)/*.
		          				SetDelay(delay)*/);		
	}

	[System.Serializable]
	public class WipeData {
		public Image Img;
		public float StartTime;
		public float WipeDur = 0f;
		public Ease EaseType;
	}
	
//	// Update is called once per frame
//	void Update () {
//	
//	}
}
