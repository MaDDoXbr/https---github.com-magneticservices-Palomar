using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineMesh))]
public class ParallelLines : MonoBehaviour
{
	public enum LineType { Horizontal=0, Vertical}
	public LineType Type;

	private LineMesh _line;
	public LineMesh Line {
		get {
			if (_line == null)
				_line = GetComponent<LineMesh>();
			return _line;
		}
		set { _line = value; }
	}
	public float StartX, EndX, StartY, EndY, OffsetX, OffsetY;
	public int LineCount;

	public void CreateLines() {
		if (Type == LineType.Horizontal)
			HorizontalLines();
		else {
			VerticalLines();
		}
		Line.DrawLine();
	}

	private void HorizontalLines() {
		var newPoints = new List<Vector3>();
		for (int i = 0; i < LineCount; i++) {
			var thisY = StartY + (OffsetY*i);
			newPoints.Add(new Vector3(StartX, thisY, 0f));
			newPoints.Add(new Vector3(EndX, thisY, 0f));
		}
		Line.Points = newPoints.ToArray();
	}
	private void VerticalLines() {
		var newPoints = new List<Vector3>();
		for (int i = 0; i < LineCount; i++) {
			var thisX = StartX + (OffsetX * i);
			newPoints.Add(new Vector3(thisX, StartY, 0f));
			newPoints.Add(new Vector3(thisX, EndY, 0f));
		}
		Line.Points = newPoints.ToArray();
	}

}
