using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineMesh))]
public class MultiLines : MonoBehaviour
{
	public LineOrientation Orientation;

	public bool Hor { get { return Orientation == LineOrientation.Horizontal; } }
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

	private List<Vector3> _points; 
	public List<Vector3> Points { 
		get { return _points; }
		set {
			//if (_points == value) return;
			_points = value;
			DrawLines();
		} 
	}
	public List<Vector3> FinalPoints;

	public float StartX, EndX, StartY, EndY, StepX, StepY;
	public int LineCount;
	private float _wipeAmount;

//	public float WipeAmount {
//		get { return _wipeAmount; }
//		set {
//			if (Mathf.Approximately(_wipeAmount, value)) 
//				return;
//			_wipeAmount = value;
//			DefinePoints();
//			DrawLines();
//		}
//	}

	public void DefinePoints() 
	{
		Points = new List<Vector3>();
		var start = Hor ? StartY : StartX;
		var offset = Hor ? StepY : StepX;

		for (int i = 0; i < LineCount; i++) {
			var nextLine = start + (offset*i);
			var lineStart = Hor
				? new Vector3(StartX, nextLine, 0f)
				: new Vector3(nextLine, StartY, 0f);
			var lineEnd = Hor
				? new Vector3(EndX, nextLine, 0f)
				: new Vector3(nextLine, EndY, 0f);
			Points.Add(lineStart);
			Points.Add(lineEnd);
			// We need these guys for tweening purposes
			FinalPoints.Add(lineStart);
			FinalPoints.Add(lineEnd);
		}
	}

	public void ZeroOutLinesLength() {
		Points = new List<Vector3> ();
		var start = Hor ? StartY : StartX;
		var offset = Hor ? StepY : StepX;

		for (int i = 0; i < LineCount; i++) {
			var nextLine = start + (offset * i);
			var lineStart = Hor
				? new Vector3 (StartX, nextLine, 0f)
				: new Vector3 (nextLine, StartY, 0f);
			Points.Add (lineStart);
			Points.Add (lineStart);
		}
	}

	public void DrawLines() 
	{
		Line.HasHat = false;
		Line.Continuous = false;
		Line.Points = Points.ToArray();
		Line.DrawLine();
	}

}
