using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineMesh))]
public class MultiLines : MonoBehaviour
{
	public LineOrientation Orientation;

	public bool Hor { get { return Orientation == LineOrientation.Horizontal; } }
	
	[SerializeField]private LineMesh _line;
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

//	[SerializeField]private List<Vector3> _points; 
//	public List<Vector3> Points { 
//		get { return _points; }
//		set {
//			if (_points == value) return;
//			_points = value;
//			DrawLines();
//		} 
//	}
	public List<Vector3> FinalPoints, Points;

	public float StartX, EndX, StartY, EndY, StepX, StepY;
	public int LineCount;
	public int VertexCount { get { return LineCount*2; } }

	public void DefinePoints() 
	{
		Points = new List<Vector3>();
		FinalPoints = new List<Vector3>();
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
			// We need to store these guys for tweening purposes
			FinalPoints.Add(lineStart);
			FinalPoints.Add(lineEnd);
		}
	}

	public void ZeroOutLinesLength() 
	{
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
		DrawLines();
	}

	public void DrawLines() 
	{
		Line.Continuous = false;
		Line.Points = Points.ToArray();
		Line.WipeAmount = 1f;	 
		Line.DrawLine();
	}

	public void UpdateAndDraw() 
	{
		DefinePoints ();
		DrawLines ();
	}
}
