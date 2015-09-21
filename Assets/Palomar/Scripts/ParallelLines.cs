using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineMesh))]
public class ParallelLines : MonoBehaviour
{
	public LineType Type {
		get {
			return (Line.FillMode == LineType.Horizontal) ?
			LineType.Vertical : LineType.Horizontal;
		} 
		set { Line.FillMode = (value == LineType.Horizontal) ?
			LineType.Vertical : LineType.Horizontal; }
	}

	public bool Hor { get { return Type == LineType.Horizontal; } }
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
	public bool HatLine;
	public float StartX, EndX, StartY, EndY, OffsetX, OffsetY;
	[HideInInspector]
	public float StartPad, EndPad;
	public int LineCount;

	public void CreateLines() {
		DefinePoints();
		Line.DrawLine();
	}

	private void DefinePoints() {
		var newPoints = new List<Vector3>();
		var start = Hor ? StartY : StartX;
		var offset = Hor ? OffsetY : OffsetX;

		for (int i = 0; i < LineCount; i++) {
			var coord = start + (offset*i);
			newPoints.Add(Hor
				? new Vector3(StartX, coord, 0f)
				: new Vector3(coord, StartY, 0f));
			newPoints.Add(Hor
				? new Vector3(EndX, coord, 0f)
				: new Vector3(coord, EndY, 0f));
		}
		if (HatLine) {
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
