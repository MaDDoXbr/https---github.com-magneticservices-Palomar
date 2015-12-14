using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Draws a dotted line. Works between two points only, start and end.
/// DotLength and Gap are defined in percentage (eg. 0.01 = 1% of line length)  
/// LineMesh X and Y scale is forced to 1.
/// </summary>
[RequireComponent(typeof(LineMesh))]
public class DottedLine : MonoBehaviour
{
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
		
	[HideInInspector]public Vector3 Start, End; 
	public float DotLength, Gap;
	public Transform StartRef, EndRef;
	public Text[] MarkerTexts = new Text[]{};

	public float WipeAmount {
		get { return Line.WipeAmount; }
		set {
			if (Mathf.Approximately(Line.WipeAmount, value)) 
				return;
			Line.WipeAmount = value;
			//MarkerUpdateCheck();
		}
	}

//	/// <summary> This method expects evenly spaced, sequential 
//	/// markers. It only considers the first and last MarkerTexts </summary>
//	private void MarkerUpdateCheck() 
//	{
//		var markerCount = MarkerTexts.Length;
//		var currVal = Mathf.Lerp(float.Parse(MarkerTexts[0].text), 
//			float.Parse(MarkerTexts[markerCount - 1].text), WipeAmount);
//			
//		//Enable all markers below the current value (val)
//		foreach (var markerText in MarkerTexts) {
//			var thisVal = float.Parse(markerText.text);
//			markerText.enabled = ( thisVal <= currVal &&
//								   WipeAmount > 0.001 );
//		}
//	}

	public void CreateLines() 
	{
		DefinePoints();
		Line.Continuous = false;
		Line.Xscale = 1f;
		Line.Yscale = 1f;
		Line.DrawLine();
		//MarkerUpdateCheck ();
	}

	private void DefinePoints() 
	{
		var newPoints = new List<Vector3>();
		float lastPosT = 0f;

		if (WipeAmount > 0.0001f)
			do {
				newPoints.Add (Vector3.Lerp (Start, End, lastPosT));
				newPoints.Add (Vector3.Lerp (Start, End, lastPosT + DotLength));
				lastPosT += DotLength + Gap;
			} while (lastPosT < 1);

		Line.Points = newPoints.ToArray();
	}

}
