using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class DonutGraph : MonoBehaviour
{
	private SceneData _sceneData;
	public SceneData ScnData
	{
		get
		{
			if (_sceneData == null)
				_sceneData = Resources.Load<SceneData> ("SceneData");
			return _sceneData;
		}
	}
    //public TextAsset XmlAsset;
    public DonutGraphParser Parser;
    public float SliceWipeDuration;
	public RadialWipeData[] DonutSlices = new RadialWipeData[] {};	//Donut1, Donut2, Donut3, Donut4;
	public float RotationTime;
	public Vector3 StartRotation, EndRotation;
	public Ease CameraRotEase;
	public Transform CameraPivot;
	public TextWipeData[] Subtitles;
	[HideInInspector]public string XmlPath;
	public bool LocalFile = true;
	private SceneManager _scnManager;
	public SceneManager ScnManager {
		get { return _scnManager; }
	}
	private bool _showDebug = true;
	public bool ShowDebug {
		get {
			if (ScnManager)
				return (ScnManager.ShowDebug);
			return _showDebug;
		}
	}
	[SerializeField]private Renderer[] _renderers;
	[SerializeField]private Canvas _canvas;

	public void Awake ()
	{
		_canvas = FindObjectOfType<Canvas> ();
		_renderers = FindObjectsOfType<Renderer> ();
		_scnManager = FindObjectOfType<SceneManager> ();
		DOTween.Init ();
	}

	public void Start() 
	{
		ImportXML(1);
	}

	public void OnGUI ()
	{
		if (!ShowDebug)
			return;
		GUILayout.Label (XmlPath);
	}

	public void SeqInsert(ref Sequence seq, TextWipeData wipe) {
		seq.Insert(wipe.StartTime,
			wipe.UIText.DOColor(wipe.FinalColor, wipe.Duration)
			.SetEase(wipe.EaseType)
			);
	}

	public void SeqInsert (ref Sequence seq, RadialWipeData wipe, float nextRotation) {
		seq.Insert(wipe.StartTime, DOTween.To(() => wipe.Img.fillAmount,
			x => wipe.Img.fillAmount = x, wipe.PercentageFinal*0.01f, SliceWipeDuration).
			SetEase(wipe.EaseType)
			);
		seq.Insert(wipe.StartTime,
			wipe.Img.transform.DORotate (wipe.EndRotation, SliceWipeDuration, RotateMode.FastBeyond360)
			.SetEase (wipe.EaseType));

		// Rotate Dotted-Line to the avg angle between current and previous slices
		var dottedLineRot = Mathf.Lerp(wipe.EndRotation.z, nextRotation, 0.5f);
		//var dataTextTransf = wipe.DataText.Root.transform;
		seq.Insert (wipe.StartTime,
			wipe.DottedLine.transform.DORotate (new Vector3 (0, 0, dottedLineRot), 
			SliceWipeDuration, RotateMode.FastBeyond360)
			.SetEase(wipe.EaseType));

		int count = 0;
		seq.Insert(wipe.StartTime,
			DOTween.To(() => count, x => count = x, wipe.PercentageFinal,
						SliceWipeDuration * 1.5f)
						.SetEase(wipe.EaseType)
						.OnUpdate(()=>wipe.DataText.Value.text = count+"%")
					);
	}

	public void ImportXML (int idx)
	{
		XmlPath = ScnData.GetImportPath (idx, GraphType.Donut, LocalFile);
		StartCoroutine (ImportFile (XmlPath));
	}

	public IEnumerator ImportFile (string path)
	{
		Tools.EnableRenderers (false, _renderers, _canvas);
		var www = new WWW (path);
		while (!www.isDone)
			yield return null;
		Parser = DonutGraphParser.Load (www);
		FeedXMLdata ();
		Tools.EnableRenderers (true, _renderers, _canvas);
		if (Application.isPlaying) {
			PlayMotion ();
			yield break;
		}
		var dottedLines = FindObjectsOfType<DottedLine> ();
		foreach (var dottedLine in dottedLines) {
			dottedLine.WipeAmount = 1f;
		}
		SetSlicesFill(null);
	}

	public void FeedXMLdata() 
	{
	    RotationTime = Parser.Rotation.Duration;
	    StartRotation = Parser.Rotation.In.ToVector3();
	    EndRotation = Parser.Rotation.Out.ToVector3();
	    CameraRotEase = Parser.Rotation.EaseType.ToEaseType();
		Subtitles[0].AssignTextProperties (Parser.Subtitle1);
		Subtitles[1].AssignTextProperties (Parser.Subtitle2);
		Subtitles[2].AssignTextProperties (Parser.Subtitle3);

	    SliceWipeDuration = Parser.Slices.Duration;

		// We use 359f to avoid Quaternion-to-Euler rounding issues
		float previousRotation = 359f;
	    int accumulatedPercent = 0;
	    for (int i = 0; i < Parser.Slices.Slices.Count; i++) {
	        previousRotation = SetupSlice(i, accumulatedPercent, previousRotation);
	        accumulatedPercent += Parser.Slices.Slices[i].FinalPercent;
	    }
	}

	public void PlayMotion() {
		DOTween.CompleteAll();
		CameraPivot.rotation = Quaternion.Euler(StartRotation);
		CameraPivot.DORotate(EndRotation, RotationTime).SetEase (CameraRotEase);
		Camera.main.DOFieldOfView (1f, RotationTime).From().SetEase (CameraRotEase);

		var mySeq = DOTween.Sequence();

		// Starts fully transparent
		foreach (TextWipeData subtitle in Subtitles) {
			subtitle.UIText.text = subtitle.FinalText;
			subtitle.UIText.color = new Color(subtitle.FinalColor.r,
				subtitle.FinalColor.g, subtitle.FinalColor.b, 0f);
			SeqInsert(ref mySeq, subtitle);
		}

		SetSlicesFill(mySeq);
	}

	private void SetSlicesFill(Sequence mySeq) {
		for (int i = 0; i < DonutSlices.Length; i++) {
			RadialWipeData wipe = DonutSlices[i];
			wipe.DataText.Value.text = "0%";
			wipe.Img.transform.rotation = Quaternion.Euler(0f, 0f, -1);
			wipe.DottedLine.transform.rotation = Quaternion.Euler(0f, 0f, -1);
			wipe.Img.fillAmount = 0.03f;
			var nextRotation = (i + 1 < DonutSlices.Length)
				? DonutSlices[i + 1].EndRotation.z
				: 0.5f;
			if (mySeq != null)
				SeqInsert(ref mySeq, wipe, nextRotation);
			// Places the DataText root 
//			var dataTextTransf = wipe.DataText.Root.transform;
//			dataTextTransf.position = wipe.DottedLine.EndRef.position;
		}
	}

	public float SetupSlice(int idx, int acumulatedPercent, float prevRotation)
    {
	    RadialWipeData thisWipe = DonutSlices[idx];
	    thisWipe.StartTime = Parser.Slices.Slices[idx].StartTime;
        thisWipe.EaseType = Parser.Slices.Slices[idx].EaseType.ToEaseType();
        thisWipe.PercentageFinal = Parser.Slices.Slices[idx].FinalPercent;

	    float thisRotation = 359.0f - (360.0f*(acumulatedPercent/100.0f));
	    thisWipe.EndRotation.z = thisRotation;

	    return thisRotation;
    }

}
