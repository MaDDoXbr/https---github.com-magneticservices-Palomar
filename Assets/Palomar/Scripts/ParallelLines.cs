using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(LineMesh))]
public class ParallelLines : MonoBehaviour
{
	public LineOrientation Type {
		get {
			return (Line.WipeMode == LineOrientation.Horizontal) ?
			LineOrientation.Vertical : LineOrientation.Horizontal;
		} 
		set { Line.WipeMode = (value == LineOrientation.Horizontal) ?
			LineOrientation.Vertical : LineOrientation.Horizontal; }
	}

	public bool Hor { get { return Type == LineOrientation.Horizontal; } }
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

	[HideInInspector]
	public bool HasHat;
	public float StartX, EndX, StartY, EndY, StepX, StepY;
	[HideInInspector]
	public float StartPad, EndPad;
	public int LineCount;
	public Text[] MarkerTexts = new Text[]{};
	public float WipeAmount {
		get { return _line.WipeAmount; }
		set {
			if (Mathf.Approximately(Line.WipeAmount, value)) 
				return;
			Line.WipeAmount = value;
			MarkerUpdateCheck();
		}
	}

	/// <summary> This function expects evenly spaced, sequential 
	/// markers. It only considers the first and last MarkerTexts </summary>
	private void MarkerUpdateCheck() 
	{
		var markerCount = MarkerTexts.Length;
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
		Line.HasHat = HasHat;
		Line.Continuous = false;
		Line.DrawLine();
	}

	private void DefinePoints() 
	{
		var newPoints = new List<Vector3>();
		var start = Hor ? StartY : StartX;
		var offset = Hor ? StepY : StepX;

		for (int i = 0; i < LineCount; i++) {
			var coord = start + (offset*i);
			newPoints.Add(Hor
				? new Vector3(StartX, coord, 0f)
				: new Vector3(coord, StartY, 0f));
			newPoints.Add(Hor
				? new Vector3(EndX, coord, 0f)
				: new Vector3(coord, EndY, 0f));
		}
		if (HasHat) {
			if (!Hor) {
				newPoints.Add(new Vector3(StartX - StartPad, StartY, 0f));
				newPoints.Add(new Vector3(StartX+(offset * (LineCount-1))+EndPad,
					StartY, 0f));
			} else {
				newPoints.Add (new Vector3 (EndX, StartY - StartPad, 0f));
				newPoints.Add (new Vector3 (EndX, 
					StartY + (offset * (LineCount-1)) + EndPad, 0f));				
			}
		}
		Line.Points = newPoints.ToArray();
	}

}
