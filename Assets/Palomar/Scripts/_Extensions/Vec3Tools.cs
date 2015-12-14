using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public static class Vec3Tools {
	public static Vector3[] OffsetPoints (this Vector3[] points, Vector3 offset)
	{
		if (offset.Equals (Vector3.zero))
			return points;
		// Make a flat clone of the original array
		var ofsPoints = Enumerable.Repeat (Vector3.zero, points.Length).ToArray ();
		Array.Copy (points, ofsPoints, points.Length);
		for (int i = 0; i < ofsPoints.Length; i++) {
			ofsPoints[i] = ofsPoints[i] + offset;
		}
		return ofsPoints;
	}

	/// <summary> Trim points according to 'completion' then Scales them </summary>
	/// <param name="points"></param>
	/// <param name="completion"></param>
	/// <param name="fillMode"></param>
	/// <returns> Adjusted Points </returns>
	public static Vector3[] AdjustPoints (this Vector3[] points, float completion, 
		LineOrientation fillMode, float xScale, float yScale, bool showDebug)
	{
		var changedScale = !(Mathf.Approximately (xScale, 1f) && Mathf.Approximately (yScale, 1f));
		var complete = Mathf.Approximately(completion, 1f);
		if (complete && !changedScale)
			return points;
		// Make a flat clone of the original array
		var adjPoints = Enumerable.Repeat (Vector3.zero, points.Length).ToArray ();
		Array.Copy (points, adjPoints, points.Length);
		if (!complete) {
			adjPoints = AdjustCompletion (points, adjPoints, completion, fillMode, 
				showDebug, xScale, yScale);
		}
		if (changedScale) {
			for (int i = 0; i < adjPoints.Length; i++) {
				adjPoints[i].Set (adjPoints[i].x * xScale,
					adjPoints[i].y * yScale,
					adjPoints[i].z);
			}
		}
		return adjPoints;
	}

	private static Vector3[] AdjustCompletion (this Vector3[] points, Vector3[] adjPoints, 
		float completion, LineOrientation fillMode, bool showDebug, float xScale, float yScale)
	{
		var first = adjPoints[0];
		var last = adjPoints[adjPoints.Length - 1];

		switch (fillMode) {
			case LineOrientation.Free:
				break;
			case LineOrientation.Horizontal:
				first = new Vector3 (first.x, 0f);
				last = new Vector3 (last.x, 0f);
				break;
			case LineOrientation.Vertical:
				first = new Vector3 (0, first.y);
				last = new Vector3 (0, last.y);
				break;
		}

		// Proportional point in an imaginary line connecting the first and last point
		var limitPoint = Vector3.Lerp (first, last, completion);
		var limitPointDistance = Vector3.Distance (limitPoint, last);

		int capIdx = 0;								// Last 'valid' Idx before the limitPoint
		bool lastPointIsExact = (limitPoint == last);

		// If last point to show == last point in the list, just assign it
		if (lastPointIsExact) {
			capIdx = adjPoints.Length;
		} else {
			for (int i = 0; i < adjPoints.Length; i++) {
				var projPoint = ProjectedPoint (adjPoints[i], first, last);
				if (showDebug)
					DebugDrawLineScaled (adjPoints[i], projPoint, 8f, xScale, yScale);
				var currPointDistance = Vector3.Distance (projPoint, last);
				if (currPointDistance < limitPointDistance) {
					capIdx = Mathf.Max (0, i - 1);	// Assures capIdx is never negative
					break;
				}
			}
		}

		// Remove remaining points, leaving room for the end cap
		// Only leaves room when the last x is not exactly the last valid x
		int trailPoints = lastPointIsExact ? 0 : 1;
		adjPoints = Enumerable.Repeat (Vector3.zero, capIdx + trailPoints + 1).ToArray ();

		// First copy all original points in range, then add the trail point if needed
		try {
			//TODO: Fix the first case, shouldn't leave a trailing Vector3.zero..
			Array.Copy (points, adjPoints, Mathf.Min (points.Length, capIdx + 1));
		} catch (System.Exception) {
			Debug.Log (" Broken capIdx: " + capIdx + " points: " + points.Length + " adjPoints: " + adjPoints.Length);
			throw;
		}

		if (!lastPointIsExact) {
			adjPoints = AddTrailPoint (adjPoints, points[capIdx], points[capIdx + 1], limitPoint, first, last);
		}

		return adjPoints;
	}

	/// <summary> Replaces the last element of the array with the proportional trailing point.
	/// We apply t in a Vector3 interpolator between current and next point. 
	/// T is calculated as the inverse interpolation between the current point (projected) distance to the first
	/// and the next point (projected) distance to the first, using dL (limit point, already projected) distance as the interpolant 
	/// </summary>
	private static Vector3[] AddTrailPoint (Vector3[] adjPoints, Vector3 current, Vector3 next,
										Vector3 limitPoint, Vector3 first, Vector3 last)
	{
		var arraysize = adjPoints.Length;
		// We need a percentage t from the current to the next point, considering limitPoint
		var pCurrentPoint = ProjectedPoint (current, first, last);
		var pNextPoint = ProjectedPoint (next, first, last);
		var d0 = Vector3.SqrMagnitude (pCurrentPoint - first);
		var d1 = Vector3.SqrMagnitude (pNextPoint - first);
		var dlimit = Vector3.SqrMagnitude (limitPoint - first);
		var t = Mathf.InverseLerp (d0, d1, dlimit);
		adjPoints[arraysize - 1] = Vector3.Lerp (current, next, t);
		return adjPoints;
	}

	/// <summary> Calculates the projected point from a source point P to a line defined by 'first' and 'last' </summary>
	public static Vector3 ProjectedPoint (Vector3 p, Vector3 first, Vector3 last)
	{
		return Vector3.Project ((p - first), (last - first)) + first;
	}

	public static void DebugDrawLineScaled (Vector3 p1, Vector3 p2, float dur, 
											float xscale, float yscale)
	{
		UnityEngine.Debug.DrawLine (Vector3.Scale (p2, new Vector3 (xscale, yscale, 1f)),
			Vector3.Scale (p1, new Vector3 (xscale, yscale, 1f)), Color.red, dur);
	}

	/// <summary>
	/// Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
	/// Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
	/// same plane, use ClosestPointsOnTwoLines() instead.
	/// </summary>
	public static bool LineLineIntersection (out Vector3 intersection, Vector3 linePoint1,
											Vector3 lineVec1, Vector3 linePoint2,
											Vector3 lineVec2, bool debug = false)
	{
		intersection = Vector3.zero;

		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross (lineVec1, lineVec2);

		float planarFactor = Vector3.Dot (lineVec3, crossVec1and2);
		//Lines are not coplanar. Take into account rounding errors.
		if ((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f)) {
			return false;
		}
		Vector3 crossVec3and2 = Vector3.Cross (lineVec3, lineVec2);

		float s = Vector3.Dot (crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
		// Opting out when s <= 1f doesn't work with open angles, nor >= 0f with closed angles
		//if((s >= 0.0f) /*&& (s <= 1.0f)*/){
		intersection = linePoint1 + (lineVec1 * s);
		if (debug)
			Debug.DrawLine (linePoint2, intersection, Color.green, 30f);
		return true;
		//} else { 
		//return false; 
		//}
	}

}
