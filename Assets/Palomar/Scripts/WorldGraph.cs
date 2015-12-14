using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// WorldGraph Regions (Continents) data should always be ordered as such:
/// America, Europe, Asia, LatinAmerica, Africa, MiddleEast and Australia
/// - In WorldGraphs, region text tweens always start at 60% of the circle tween
/// duration, and their tween duration is 30% of the circle tween duration.
/// - Circle Scales are calculated relative to the min and max values in the XML
/// </summary>
public class WorldGraph : MonoBehaviour {

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
    public TextAsset XmlAsset;
	public DottedLineWipeData[] RegionWipes;					//Eg.: Asia, Latin America, etc;
	public Transform CameraPivot;
	public LineMesh WipeLinesMesh;
	public float WipeLinesTweenDuration = 3f;
	private const float TextStartPercentage = 0.6f;
	private const float TextTweenPercentage = 0.3f;
    private const float minCircleRadius = 0.1f;
    private const float maxCircleRadius = 1f;
    [HideInInspector]public RotationData Rot;
    public WorldGraphParser Parser;
    public TextWipeData[] Subtitles;
	[HideInInspector]public string XmlPath;
	public bool LocalFile = true;
	private SceneManager _scnManager;
	public SceneManager ScnManager
	{
		get { return _scnManager; }
	}
	private bool _showDebug = true;
	public bool ShowDebug
	{
		get
		{
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

	public void OnGUI ()
	{
		if (!ShowDebug)
			return;
		GUILayout.Label (XmlPath);
	}

	public void Start () {
		ImportXML(1);
	}

	public void ImportXML (int idx)
	{
		XmlPath = ScnData.GetImportPath (idx, GraphType.World, LocalFile);
		StartCoroutine (ImportFile (XmlPath));
	}
	
	public IEnumerator ImportFile (string path)
	{
		Tools.EnableRenderers (false, _renderers, _canvas);
		var www = new WWW (path);
		while (!www.isDone)
			yield return null;
		Parser = WorldGraphParser.Load (www);
		FeedXMLdata ();
		Tools.EnableRenderers (true, _renderers, _canvas);
		if (Application.isPlaying)
			PlayMotion ();
		else {
			var dottedLines = FindObjectsOfType<DottedLine> ();
			foreach (var dottedLine in dottedLines) {
				dottedLine.WipeAmount = 1f;
			}
		}
	}

	public void FeedXMLdata() 
	{
		Subtitles[0].AssignTextProperties (Parser.Subtitle1);
		Subtitles[1].AssignTextProperties (Parser.Subtitle2);
		Subtitles[2].AssignTextProperties (Parser.Subtitle3);

        Rot.Duration = Parser.Rotation.Duration;
        Rot.EaseType = Parser.Rotation.EaseType.ToEaseType();
        Rot.StartRotation = Parser.Rotation.In.ToVector3();
        Rot.EndRotation = Parser.Rotation.Out.ToVector3();

        float minValue = Parser.Regions.Region[0].Value;
        float maxValue = minValue;
		for (int k = 1; k < RegionWipes.Length; k++) {
            minValue = Mathf.Min(minValue, Parser.Regions.Region[k].Value);
            maxValue = Mathf.Max(maxValue, Parser.Regions.Region[k].Value);
		}

		for (int i = 0; i < RegionWipes.Length; i++) {
            var thisRegion = Parser.Regions.Region[i];
            var t = Mathf.InverseLerp (minValue, maxValue, thisRegion.Value);
            RegionWipes[i].CircleEndScale = Mathf.Lerp(minCircleRadius, maxCircleRadius, t);
            RegionWipes[i].StartTime = thisRegion.StartTime;
            RegionWipes[i].Duration = Parser.Regions.Duration;
            RegionWipes[i].Data.Value.text = thisRegion.Value + Parser.Regions.Units;
		}
    }

	private void PlayMotion() {
		DOTween.CompleteAll ();

		WipeLinesMesh.LineWidth = 5f;

		CameraPivot.rotation = Quaternion.Euler(Rot.StartRotation);
		CameraPivot.DORotate(Rot.EndRotation, Rot.Duration).SetEase(Rot.EaseType);
		Camera.main.DOFieldOfView(1f, Rot.Duration).From().SetEase(Rot.EaseType);
		DOTween.To(() => WipeLinesMesh.LineWidth,
			x => WipeLinesMesh.LineWidth = x, 0f, WipeLinesTweenDuration).
			SetEase(Ease.OutSine);

		Sequence mySeq = DOTween.Sequence();

		foreach (DottedLineWipeData wipe in RegionWipes) {
			wipe.Data.Root.localScale = Vector3.zero;
			wipe.CircleTransform.localScale = Vector3.zero;
			wipe.DottedLine.WipeAmount = 0f;
			SeqInsert(ref mySeq, wipe);
		}
	}

	public void SeqInsert (ref Sequence seq, DottedLineWipeData wipe) {
		seq.Insert (wipe.StartTime,
			wipe.CircleTransform.DOScale (wipe.CircleEndScale, wipe.Duration)
			.SetEase (wipe.EaseType));
		var textStartTime = Mathf.Lerp(wipe.StartTime, wipe.StartTime + wipe.Duration, TextStartPercentage);
		seq.Insert (textStartTime,
			wipe.Data.Root.DOScale (1f, wipe.Duration * TextTweenPercentage)
			.SetEase (wipe.EaseType));
		seq.Insert(wipe.StartTime, DOTween.To(() => wipe.DottedLine.WipeAmount,
			x => wipe.DottedLine.WipeAmount = x, 1f, wipe.Duration).
			SetEase(wipe.EaseType)
			);
	}

}
