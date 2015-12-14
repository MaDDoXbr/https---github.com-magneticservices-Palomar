using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(LineMesh))]
public class ParallelLines : MonoBehaviour
{
	// Currently doesn't support "free" wipe mode
	public LineOrientation Along {
		get {
			return (Line.WipeMode == LineOrientation.Horizontal) ?
			LineOrientation.Horizontal : LineOrientation.Vertical;
		} 
		set { Line.WipeMode = (value == LineOrientation.Horizontal) ?
			LineOrientation.Horizontal : LineOrientation.Vertical; } 
	}

	public bool HasHat	{ get { return HatLine != null; } }
	public bool Vert	{ get { return Along == LineOrientation.Vertical; } }
	private LineMesh _line;
	public LineMesh Line {
		get {
			if (_line == null)
				_line = GetComponent<LineMesh>();
			return _line;
		}
		set {
			_line = value;
		}
	}
	public LineMesh HatLine;

	public float StartX, EndX, StartY, EndY, StepX, StepY;
	public float StartPad, EndPad;
	public int LineCount;
	public Text[] MarkerTexts = new Text[]{};
	private const float HatSpeedUpFactor = 0.75f;

	public float WipeAmount {
		get { return Line.WipeAmount; }
		set {
			if (Mathf.Approximately(Line.WipeAmount, value)) 
				return;
			Line.WipeAmount = value;
			//Debug.Log (1 + HatSpeedUpFactor * (1 - value));
			if (HasHat)
				HatLine.WipeAmount = value * (1 + HatSpeedUpFactor*(1-value));
			MarkerUpdateCheck();
		}
	}

	/// <summary> This method expects evenly spaced, sequential 
	/// markers. It only considers the first and last MarkerTexts </summary>
	private void MarkerUpdateCheck() 
	{
		var markerCount = MarkerTexts.Length;
		if (MarkerTexts.Length == 0) return;
		var currVal = Mathf.Lerp(float.Parse(MarkerTexts[0].text), 
			float.Parse(MarkerTexts[markerCount - 1].text), WipeAmount);
			
		//Enable all markers below the current value (val)
		foreach (var markerText in MarkerTexts) {
			var thisVal = float.Parse(markerText.text);
			markerText.enabled = ( thisVal <= currVal &&
								   WipeAmount > 0.001 );
		}
	}

	public void CreateLines() 
	{
		DefinePoints();
		Line.Continuous = false;
		Line.DrawLine();
		if (HasHat) {
			HatLine.WipeAmount = Line.WipeAmount *
				(1 + HatSpeedUpFactor * (1 - Line.WipeAmount));
		}
		MarkerUpdateCheck ();
	}

	private void DefinePoints() 
	{
		var newPoints = new List<Vector3>();
		var start = Vert ? StartY : StartX;
		var offset = Vert ? StepY : StepX;

		for (int i = 0; i < LineCount; i++) {
			var coord = start + (offset*i);
			newPoints.Add(Vert
				? new Vector3(StartX, coord, 0f)
				: new Vector3(coord, StartY, 0f));
			newPoints.Add(Vert
				? new Vector3(EndX, coord, 0f)
				: new Vector3(coord, EndY, 0f));
		}
		Line.Points = newPoints.ToArray();
		if (!HasHat)
			return;

		newPoints = new List<Vector3> ();
		if (Vert) {
			newPoints.Add (new Vector3 (EndX, StartY - StartPad, 0f));
			newPoints.Add (new Vector3 (EndX,
				StartY + (offset * (LineCount - 1)) + EndPad, 0f));	
		} else {
			newPoints.Add (new Vector3 (StartX - StartPad, StartY, 0f));
			newPoints.Add (new Vector3 (StartX + (offset * (LineCount - 1)) + EndPad,
				StartY, 0f));			
		}
		HatLine.Points = newPoints.ToArray();
	}

}
