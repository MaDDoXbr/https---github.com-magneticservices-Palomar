using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LineGraph : MonoBehaviour
{
	private SceneData _sceneData;
	public SceneData ScnData {
		get {
			if (_sceneData == null)
				_sceneData = Resources.Load<SceneData>("SceneData");
			return _sceneData;
		}
	}
	public LineGraphParser Parser = new LineGraphParser(); 
	public TextWipeData[] Subtitles;
	public LineWipeData[] LineWipes;
	public RulerWipeData[] RulerWipes;
	public BGWipeData BGWipe;
	public Transform Pivot;
	[HideInInspector]public RotationData Rot;
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
		_canvas = FindObjectOfType<Canvas>();
		_renderers = FindObjectsOfType<Renderer>();
		_scnManager = FindObjectOfType<SceneManager> ();
		DOTween.Init();
	}

	public void Start () 
	{
		ImportXML(1);
	}

	public void OnGUI() 
	{
		if (!ShowDebug)
			return;
		GUILayout.Label (XmlPath);		
	}

	public void ImportXML(int idx) 
	{
		XmlPath = ScnData.GetImportPath(idx, GraphType.Line, LocalFile);
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
			var lineMeshes = FindObjectsOfType<LineMesh>();
			foreach (var lineMesh in lineMeshes) {
				lineMesh.WipeAmount = 1f;
			}
		}
	}

	private void FeedXMLdata()
	{
		if (Parser == null){
			Debug.LogWarning("Parser creation error - is the URL Path correct?");
			return;
		}
		Subtitles[0].AssignTextProperties (Parser.Subtitle1);
		Subtitles[1].AssignTextProperties (Parser.Subtitle2);
		Subtitles[2].AssignTextProperties (Parser.Subtitle3);

		Rot.Duration = Parser.Rotation.Duration;
		Rot.EaseType = Parser.Rotation.EaseType.ToEaseType();
		Rot.StartRotation = Parser.Rotation.In.ToVector3();
		Rot.EndRotation = Parser.Rotation.Out.ToVector3();

		FeedRulerData();

		FeedBGWipeData();

		EnableDisableItems();

		for (int i = 0; i < Parser.Lines.Line.Count; i++) {
			LineWipes[i].StartTime = Parser.Lines.Line[i].StartTime;
			LineWipes[i].Duration = Parser.Lines.Line[i].Duration;
			LineWipes[i].EaseType = Parser.Lines.Line[i].EaseType.ToEaseType();

			LineWipes[i].Line.LineColor = Parser.Lines.Line[i].Color.ToColor();

			LineWipes[i].Line.Points = new Vector3[Parser.Lines.Line[i].Points.Count];
			int z;
			if ( int.TryParse(Parser.Lines.Line[i].z, out z) )
				LineWipes[i].Line.Offset = new Vector3(0,0,z);
			for (int j = 0; j < Parser.Lines.Line[i].Points.Count; j++) {
				LineWipes[i].Line.Points[j].x = Parser.Lines.Line[i].Points[j].X;
				LineWipes[i].Line.Points[j].y = Parser.Lines.Line[i].Points[j].Y;
			}
			LineWipes[i].Line.DrawLine();
		}
	}

    public void PlayMotion()
    {
		DOTween.CompleteAll ();
        WipeSubtitles();
        WipeLines();
        WipeRulers();
        WipeBG();
        Pivot.rotation = Quaternion.Euler(Rot.StartRotation);
        Pivot.DORotate (Rot.EndRotation, Rot.Duration).
            SetEase (Rot.EaseType);
    }

	private void EnableDisableItems() {
		for (int j = 0; j < 10; j++) {
			if (j < Parser.Lines.Line.Count)
				LineWipes[j].Line.gameObject.SetActive(true);
			else
				LineWipes[j].Line.gameObject.SetActive(false);
		}
	}

	private void FeedBGWipeData() {
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

	private void FeedRulerData() {
		if (Parser.Rulers[0].Along.ToLower() == "vertical") {
			FeedRulerDataCommon(0);
			RulerWipes[0].Ruler.StepX = Parser.Rulers[0].StepX;
		} else if (Parser.Rulers[1].Along.ToLower() == "horizontal") {
			FeedRulerDataCommon (1);
			RulerWipes[1].Ruler.StepY = Parser.Rulers[1].StepY;
		}
		foreach (var rulerWipe in RulerWipes) {
			rulerWipe.Ruler.CreateLines();
		}
	}

	private void FeedRulerDataCommon(int idx) {
		RulerWipes[idx].StartTime = Parser.Rulers	[idx].StartTime;
		RulerWipes[idx].EaseType = Parser.Rulers	[idx].EaseType.ToEaseType ();
		RulerWipes[idx].Duration = Parser.Rulers	[idx].Duration;

		RulerWipes[idx].Ruler.Along = Parser.Rulers	[idx].Along.ToLineOrientation ();
		RulerWipes[idx].Ruler.LineCount = Parser.Rulers[idx].LineCount;
		RulerWipes[idx].Ruler.StartX = Parser.Rulers[idx].StartX;
		RulerWipes[idx].Ruler.StartY = Parser.Rulers[idx].StartY;
		RulerWipes[idx].Ruler.EndY = Parser.Rulers	[idx].EndY;
	}

	private void WipeLines() 
	{
		var linesSeq = DOTween.Sequence();
		foreach (var wipe in LineWipes) {
			if (wipe == null || wipe.Line == null) 
				continue;
			wipe.Line.WipeAmount = 0f;
			wipe.Line.DrawLine();
			linesSeq.SeqInsert(wipe);
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
			rulerSeq.SeqInsert (wipe);
		}
	}

	private void WipeSubtitles() 
	{
		var subtitleSeq = DOTween.Sequence();
		foreach (var subtitle in Subtitles) {
			if (subtitle == null) 
				continue;
			subtitle.UIText.text = "";
			subtitleSeq.SeqInsert(subtitle);
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
				BgSeq.SeqInsert (BGWipe, i);
		}
	}

}
