using System.Collections;
using DG.Tweening;
using UnityEngine;

public class FilledLineGraph : MonoBehaviour
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
	public LineGraphParser Parser; 
	public TextWipeData[] Subtitles;
	public FilledLineWipeData FilledLineWipe;
	public RulerWipeData[] RulerWipes;
	public BGWipeData BGWipe;
	public Transform Pivot;
	[HideInInspector]public RotationData Rot;
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
	
	public void Start () {
		ImportXML(1);
	}

	public void OnGUI ()
	{
		if (!ShowDebug)
			return;
		GUILayout.Label (XmlPath);
	}

	private void PlayMotion() 
	{
		DOTween.CompleteAll ();
		WipeSubtitles();
		WipeLine();
		WipeRulers();
		WipeBG();
		Pivot.rotation = Quaternion.Euler(Rot.StartRotation);
		Pivot.DORotate(Rot.EndRotation, Rot.Duration).SetEase(Rot.EaseType);
	}

	public void ImportXML (int idx)
	{
		XmlPath = ScnData.GetImportPath (idx, GraphType.Filled, LocalFile);
		StartCoroutine (ImportFile (XmlPath));
	}

	public IEnumerator ImportFile (string path)
	{
		Tools.EnableRenderers (false, _renderers, _canvas);
		var www = new WWW (path);
		while (!www.isDone)
			yield return null;
		Parser = LineGraphParser.Load (www);
		FeedXMLdata ();
		Tools.EnableRenderers (true, _renderers, _canvas);
		if (Application.isPlaying)
			PlayMotion();
		else {
			var lineMeshes = FindObjectsOfType<FilledLineMesh>();
			foreach (var lineMesh in lineMeshes) {
				lineMesh.WipeAmount = 1f;
			}
		}
	}

	/// <summary> For FilledLine we use the same LineGraphParser, considering only the first
	/// element of the <!--Lines --> tag </summary>
	public void FeedXMLdata() {

		Subtitles[0].AssignTextProperties (Parser.Subtitle1);
		Subtitles[1].AssignTextProperties (Parser.Subtitle2);
		Subtitles[2].AssignTextProperties (Parser.Subtitle3);

	    Rot.Duration = Parser.Rotation.Duration;
	    Rot.EaseType = Parser.Rotation.EaseType.ToEaseType();
	    Rot.StartRotation = Parser.Rotation.In.ToVector3();
	    Rot.EndRotation = Parser.Rotation.Out.ToVector3();

	    ParseRuler();

	    ParseBGWipe();

		FilledLineWipe.StartTime = Parser.Lines.Line[0].StartTime;
		FilledLineWipe.Duration = Parser.Lines.Line[0].Duration;
		FilledLineWipe.EaseType = Parser.Lines.Line[0].EaseType.ToEaseType ();

		FilledLineWipe.Line.LineColor = Parser.Lines.Line[0].Color.ToColor ();

		FilledLineWipe.Line.Points = new Vector3[Parser.Lines.Line[0].Points.Count];
		for (int j = 0; j < Parser.Lines.Line[0].Points.Count; j++) {
			FilledLineWipe.Line.Points[j].x = Parser.Lines.Line[0].Points[j].X;
			FilledLineWipe.Line.Points[j].y = Parser.Lines.Line[0].Points[j].Y;
		}
		FilledLineWipe.Line.DrawFilledLine ();
	}

	private void ParseBGWipe() {
		BGWipe.StartTime = Parser.BGLines.StartTime;
		BGWipe.Duration = Parser.BGLines.Duration;
		BGWipe.EaseType = Parser.BGLines.EaseType.ToEaseType();
		BGWipe.StepDelay = Parser.BGLines.StepDelay;

		BGWipe.MultiLines.StartX = Parser.BGLines.StartX;
		BGWipe.MultiLines.EndX = Parser.BGLines.EndX;
		BGWipe.MultiLines.StartY = Parser.BGLines.StartY;
		BGWipe.MultiLines.EndY = Parser.BGLines.EndY;

		BGWipe.MultiLines.LineCount = Parser.BGLines.LineCount;
		BGWipe.MultiLines.Orientation = Parser.BGLines.Orientation.ToLineOrientation();

		if (Parser.BGLines.Orientation.ToLower() == "horizontal") {
			BGWipe.MultiLines.StepY = Parser.BGLines.StepY;
		} else if (Parser.BGLines.Orientation.ToLower() == "vertical") {
			BGWipe.MultiLines.StepX = Parser.BGLines.StepX;
		}
		BGWipe.MultiLines.UpdateAndDraw();
	}

	private void ParseRuler() {
		if (Parser.Rulers[0].Along.ToLower() == "vertical") {
			ParseRulerCommon(0);
			RulerWipes[0].Ruler.StepX = Parser.Rulers[0].StepX;
		} else if (Parser.Rulers[1].Along.ToLower() == "horizontal") {
			ParseRulerCommon (1);
			RulerWipes[1].Ruler.StepY = Parser.Rulers[1].StepY;
		}
		foreach (var rulerWipe in RulerWipes) {
			rulerWipe.Ruler.CreateLines();
		}
	}

	private void ParseRulerCommon(int idx) {
		RulerWipes[idx].StartTime = Parser.Rulers	[idx].StartTime;
		RulerWipes[idx].EaseType = Parser.Rulers	[idx].EaseType.ToEaseType ();
		RulerWipes[idx].Duration = Parser.Rulers	[idx].Duration;

		RulerWipes[idx].Ruler.Along = Parser.Rulers	[idx].Along.ToLineOrientation ();
		RulerWipes[idx].Ruler.LineCount = Parser.Rulers[idx].LineCount;
		RulerWipes[idx].Ruler.StartX = Parser.Rulers[idx].StartX;
		RulerWipes[idx].Ruler.StartY = Parser.Rulers[idx].StartY;
		RulerWipes[idx].Ruler.EndY = Parser.Rulers	[idx].EndY;
	}

	private void WipeLine() 
	{
		var linesSeq = DOTween.Sequence();
		if (FilledLineWipe == null || FilledLineWipe.Line == null) 
			return;
		FilledLineWipe.Line.WipeAmount = 0f;
		FilledLineWipe.Line.DrawFilledLine ();
		SeqInsert(ref linesSeq, FilledLineWipe);
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
		BGWipe.MultiLines.DefinePoints();
		BGWipe.MultiLines.ZeroOutLinesLength();
		var BgSeq = DOTween.Sequence ();

		for (int i = 0; i < BGWipe.MultiLines.VertexCount; i++) {
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

	public void SeqInsert (ref Sequence seq, FilledLineWipeData wipeData)
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
			DOTween.To (() => wipeData.MultiLines.Points[i],
						x => wipeData.MultiLines.Points[i] = x,
						wipeData.MultiLines.FinalPoints[i], wipeData.Duration).
						SetEase (wipeData.EaseType)
						.OnUpdate(UpdateMultiLine)
						);
	}

	public void UpdateMultiLine() {
		BGWipe.MultiLines.DrawLines();
	}

}
