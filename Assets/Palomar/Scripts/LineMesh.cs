using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public enum LineType { Horizontal=0, Vertical }

[ExecuteInEditMode]
public class LineMesh : MonoBehaviour {

    #region public MeshFilter Mfilter
    MeshFilter _mFilter;
	public MeshFilter Mfilter { 
		get {
			if (_mFilter == null)
				_mFilter = GetComponent<MeshFilter>();
			if (_mFilter == null)
				_mFilter = gameObject.AddComponent<MeshFilter>();
			return _mFilter;} 
		set { _mFilter = value; }
	}
    #endregion
    #region public MeshRenderer Mrender
    MeshRenderer _mRender;
    public MeshRenderer Mrender
    {
        get
        {
            if (_mRender == null)
                _mRender = GetComponent<MeshRenderer>();
            if (_mRender == null)
                _mRender = gameObject.AddComponent<MeshRenderer>();
            return _mRender;
        }
        set { _mRender = value; }
    }
    #endregion
	public List<Vector3> PolyLine;		// Split and Line done with quads, renderable
	public List<Vector3[]> SplitQuads;
	public List<Slice2D> SourceSlices, FinalSlices;
	// TODO: Points must be ordered in ascending order. Currently it assumes so.
	public Vector3[] Points;
	public float LineWidth = 3f;
	public Color LineColor = Color.magenta;
	public bool DrawOnStart, Continuous = true, debug;
	public float Xscale = 1f, Yscale = 1f;
	public Vector3 Offset;
	public LineType FillMode = LineType.Horizontal;
	private float _fillAmount;
	public float FillAmount {
		get { return _fillAmount; }
		set {
			if (Mathf.Approximately(value, _fillAmount)) 
				return;
			_fillAmount = Mathf.Clamp01(value);
			DrawLine();
        }
    }

	public Mesh ThisMesh { 
		get { return Mfilter.sharedMesh;}
		set { Mfilter.sharedMesh = value; }
	}
    public Material ThisMaterial {
        get { return Mrender.sharedMaterial; }
        set { Mrender.sharedMaterial = value; }
    }

	public void Awake() 
    {
		Mfilter = GetComponent<MeshFilter>();
	}

//	public void OnDrawGizmos() {
//		if (SplitQuads == null) return;
//		Gizmos.color = Color.yellow;
//		foreach (var vtx in SplitQuads) {
//			//TODO: Editor command: Handles.Label(vtx, i);
//			Gizmos.color = (Gizmos.color == Color.yellow)? 
//				Color.red : Color.yellow;
//			for (int j=0; j<4; j++) {
//				Gizmos.DrawSphere(vtx[j], 0.05f);
//			}
//		}
//	}

	public void Start() 
    {
		if (DrawOnStart)
			DrawLine();
	}

	public void DrawLine() 
	{
	    if (Points.Length < 2) return;
        ThisMesh = new Mesh();
		if (FillAmount < 0.001f)
			return;

		SplitQuads = new List<Vector3[]>();
		SourceSlices = new List<Slice2D>();
        PolyLine = new List<Vector3>();

	    // Adjusted Points are trimmed (fillamount) then scaled
	    var adjPoints = AdjustPoints(Points, FillAmount, FillMode);
		adjPoints = OffsetPoints(adjPoints, Offset);

		InitializeQuads(adjPoints);
		FinalSlices = Continuous ? SetAveragedSlices (SourceSlices) : SourceSlices;

        // Prepare vertices
        foreach (Slice2D slice in FinalSlices) {
            PolyLine.Add(slice.P1);
            PolyLine.Add(slice.P2);
        }
		ThisMesh.vertices = PolyLine.ToArray();
		AssignDefaultUvs (ThisMesh);
        if (Continuous)
		    CreateConnectedMesh(ThisMesh, FinalSlices);
        else {
            CreateSplitMesh(ThisMesh, FinalSlices);
        }
        //ThisMaterial.EnableKeyword("_EMISSION");       // Unity5 Standard Material Requirement
		//"_EmissionColor" 
		ThisMaterial.SetColor("_Color", LineColor);
	}

	/// <summary> Initialize the separate quads </summary>
	/// <param name="adjPoints"></param>
	private void InitializeQuads(Vector3[] adjPoints) {
		var inc = Continuous ? 1 : 2;
		for (int i = 0; i < adjPoints.Length - 1; i = i + inc) {
			var quad = MakeQuad(adjPoints[i], adjPoints[i + 1], LineWidth);
			SplitQuads.Add(quad);
			// We use 0, 1, 3, 2 ordering to keep segments aligned relative to +Y
			SourceSlices.Add(new Slice2D(quad[0], quad[1]));
			SourceSlices.Add(new Slice2D(quad[3], quad[2]));
		}
	}

	private Vector3[] OffsetPoints(Vector3[] points, Vector3 offset) {
		if (offset.Equals(Vector3.zero)) return points;
		// Make a flat clone of the original array
		var ofsPoints = Enumerable.Repeat(Vector3.zero, points.Length).ToArray();
		Array.Copy(points, ofsPoints, points.Length);
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
	private Vector3[] AdjustPoints(Vector3[] points, float completion, LineType fillMode) {
        var changedScale = !(Mathf.Approximately(Xscale, 1f) && Mathf.Approximately(Yscale, 1f));
        var incomplete = completion < 1f;
        if (!incomplete && !changedScale) 
            return points;
        // Make a flat clone of the original array
        var adjPoints = Enumerable.Repeat(Vector3.zero, points.Length).ToArray();
        Array.Copy(points, adjPoints, points.Length);
        if (incomplete) {
            adjPoints = AdjustCompletion(points, adjPoints, completion, fillMode);
        }
        if (changedScale) {
            for (int i = 0; i < adjPoints.Length; i++) {
                adjPoints[i].Set(adjPoints[i].x*Xscale,
                    adjPoints[i].y*Yscale,
                    adjPoints[i].z);
            }
        }
      return adjPoints;
    }

	private Vector3[] AdjustCompletion(Vector3[] points, Vector3[] adjPoints, float completion, 
										LineType fillMode) 
	{
		var first = (fillMode == LineType.Horizontal) ? 
			adjPoints[0].x : adjPoints[0].y;
		var last = (fillMode == LineType.Horizontal) ?
			adjPoints[adjPoints.Length - 1].x : adjPoints[adjPoints.Length - 1].y;
		var valToShow = Mathf.Lerp(first, last, completion);
		//Debug.Log("X coord of cap point:" + valToShow);
		float prevSourceVal = first;			// only for debug purposes and to detect if the last point is exact
		int capIdx = 0;							// that's what does the magic
		bool exactLastPoint = Mathf.Approximately(valToShow, prevSourceVal);
		// If last x to show == last value in the list, just assign it
		if (exactLastPoint) {
			capIdx = adjPoints.Length;
		} else {
			for (int i = 0; i < adjPoints.Length; i++) {
				var currVal = (fillMode == LineType.Horizontal) ? 
					adjPoints[i].x : adjPoints[i].y;
				if (currVal > valToShow) {
					capIdx = i - 1;
					break;
				}
			}
		}
		// Remove remaining points, leaving room for the end cap
		// Only leaves room when the last x is not exactly the last valid x
		int trailPoints = exactLastPoint ? 0 : 1;
		adjPoints = Enumerable.Repeat (Vector3.zero, capIdx + trailPoints + 1).ToArray();
		// First copy all original "whole" points, then add the trail point if needed
		try { 
			Array.Copy (points, adjPoints, capIdx + 1);
		} catch (Exception) {
			Debug.Log(" Broken capIdx: "+capIdx);
			throw;
		}
		if (!exactLastPoint)
			adjPoints = AddTrailPoint (adjPoints, points[capIdx], 
										points[capIdx + 1], valToShow, fillMode);
		return adjPoints;
	}

	// Replaces the last element of the array with the proportional trailing point
	private Vector3[] AddTrailPoint (Vector3[] adjPoints, Vector3 currentPoint, 
		Vector3 nextPoint, float valToShow, LineType fillMode) 
	{
		var arraysize = adjPoints.Length;
		var currVal = (fillMode == LineType.Horizontal) ? 
			currentPoint.x : currentPoint.y;
		var nextVal = (fillMode == LineType.Horizontal) ?
			nextPoint.x : nextPoint.y;
		// We need a percentage t from the current to the next point, considering x (or y)
		var t = Mathf.InverseLerp (currVal, nextVal, valToShow);
		adjPoints[arraysize - 1] = Vector3.Lerp (currentPoint, nextPoint, t);
		return adjPoints;
	}

	// Initial and last slices are the same as in the split quads, others are averaged
	List<Slice2D> SetAveragedSlices (List<Slice2D> source) {
		if (source.Count < 2) {
			Debug.Log("Error: Empty list of slices");
			return null;
		}
	    var NewSlices = new List<Slice2D>();
		NewSlices.Add (source [0]);
		for (int i = 1; i < source.Count - 1; i=i+2) {
			// That's the average line in connection segments (cross-section slices)
			var a1 = (source [i].P1 + source [i+1].P1) / 2;
			var a2 = (source [i].P2 + source [i+1].P2) / 2;
			if (debug) Debug.DrawLine (a1, a2, Color.blue, 30f);
			var medianLineDir = a2 - a1; 					// point upwards

			var topLine = (source [i].P2 - source [i-1].P2);
			var botLine = (source [i].P1 - source [i-1].P1);

			//Debug.DrawLine (source [i].P2, source [i-1].P2, Color.magenta, 30f);
		
			Vector3 cornerTop, cornerBot;
			// Now we need to extend the median line to any top line (previous chosen), to get the right angled corner
			// parms: top line origin, top line direction, median line origin, median line direction
			LineLineIntersection (out cornerTop, source [i].P2, topLine, a2, medianLineDir, true);
			LineLineIntersection (out cornerBot, source [i].P1, botLine, a1, medianLineDir, true);
			//Debug.Log ("source & intersec: "+a2+ " " + cornerTop);
			//Debug.Log ("source & intersec: "+a1+ " " + cornerBot);
			NewSlices.Add (new Slice2D (cornerBot, cornerTop));
		}
		NewSlices.Add (source [source.Count - 1]);
	    return NewSlices;
	}

	// We don't use textures, so for UVs we just assign the xy 2D coords to them
	static void AssignDefaultUvs (Mesh mesh)
	{
		var vtxCount = mesh.vertices.Length;
		var uvs = new Vector2[vtxCount];
		for (var i = 0; i < vtxCount; i++) {
			uvs [i] = new Vector2 (mesh.vertices [i].x, mesh.vertices [i].y);
		}
		mesh.uv = uvs;
	}

    //TODO: Check for more elements
    private void CreateSplitMesh(Mesh mesh, List<Slice2D> slices) {
        var sliceCount = slices.Count;

        if (sliceCount < 2)
            return;
        // Eg.: | | | | | => 5 slices = 8 tris (5 - 1 => 4quads * 2tris/quad = 8), times 3 idx / tri
        var triIdx = new int[(sliceCount - 1) * 6];
        // Eg.: 1 3  5 7
        //      0 2  4 6 ... => 0,1,3, 0,3,2, 4,5,7, 4,7,6
        int i = 0;
        // "j" is the Triangle Index
        // Will repeat the number of quads (slices/2) * 6 (# of "j"s per quad)
        for (int j = 0; j < (sliceCount * 3); j = j + 6) {
            triIdx[j] = i;
            triIdx[j + 1] = i + 1;
            triIdx[j + 2] = i + 3;
            triIdx[j + 3] = i;
            triIdx[j + 4] = i + 3;
            triIdx[j + 5] = i + 2;
            //Debug.Log(" j:" + j+" "+(j+1)+" "+(j+2)+" "+(j+3)+" "+(j+4)+" "+(j+5));
            //Debug.Log(" i:" + i + " " + (i + 1) + " " + (i + 3) + " " + (i) + " " + (i + 3) + " " + (i + 2));
            i += 4;
        }
        mesh.SetTriangles(triIdx, 0);
        mesh.RecalculateNormals();
    }

    static void CreateConnectedMesh(Mesh mesh, List<Slice2D> slices)
    {
	    var sliceCount = slices.Count;

		if (sliceCount < 2) return;
		// Eg.: | | | | | => 5 slices = 8 tris (5 - 1 => 4quads * 2tris/quad = 8), times 3 idx / tri
        var triIdx = new int[(sliceCount - 1) * 6];
		// Eg.: 1 3  3 5
		//      0 2  2 4 ... => 0,1,3, 0,3,2, 2,3,4, 2,5,4
        int i = 0;
        // J is the Triangle Index
        // Will repeat the number of quads (slices-1) * 6 (2 tris)
		for (int j = 0; j < ((sliceCount - 1)*6); j=j+6) {
			triIdx[j] = i;
			triIdx[j+1] = i + 1;
			triIdx[j+2] = i + 3;
			triIdx[j+3] = i;
			triIdx[j+4] = i + 3;
			triIdx[j+5] = i + 2;
            //Debug.Log(" j:" + j+" "+(j+1)+" "+(j+2)+" "+(j+3)+" "+(j+4)+" "+(j+5));
		    i += 2;
		}
		mesh.SetTriangles (triIdx, 0);
		mesh.RecalculateNormals ();
	}
	
	Vector3[] MakeQuad(Vector3 begin, Vector3 end, float width) 
	{
		var halfwidth = width / 2;
		Vector3[] q = new Vector3[4];
		
		//var normal = Vector3.Cross(begin, end);
		// Edge case fix when beginning point is at (0,0)
		//TODO: Accept points in 3D
		//if (Mathf.Approximately(normal.z, 0f))
		var	normal = Vector3.back;
		var side = Vector3.Cross(normal, begin - end); 		//side or "corner" vector, "upwards"
		
		side.Normalize();

		q[0] = transform.InverseTransformPoint(begin + (side * -halfwidth));
		q[1] = transform.InverseTransformPoint(begin + (side * halfwidth));
		q[2] = transform.InverseTransformPoint(end + (side * halfwidth));
		q[3] = transform.InverseTransformPoint(end + (side * -halfwidth));

		return q;
	}

	[System.Serializable]
	public class Slice2D {
		public Slice2D(Vector3 p1, Vector3 p2) {
			P1 = p1;
			P2 = p2;
		}
		public Vector3 P1;
		public Vector3 P2;
	}

	// Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
	// Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
	// same plane, use ClosestPointsOnTwoLines() instead.
	public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, 
	                                        Vector3 lineVec1, Vector3 linePoint2, 
	                                        Vector3 lineVec2, bool debug = false)
	{
		intersection = Vector3.zero;
		
		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);

		float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
		//Lines are not coplanar. Take into account rounding errors.
		if((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f)){
			return false;
		}
		Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
		
		// Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
		float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
		// Opting out when s <= 1f doesn't work with open angles, nor >= 0f with closed angles
		//if((s >= 0.0f) /*&& (s <= 1.0f)*/){
			intersection = linePoint1 + (lineVec1 * s);
			if (debug) Debug.DrawLine (linePoint2, intersection, Color.green,30f);
			return true;
//		} else { 
//			return false; 
//		}
	}
}
