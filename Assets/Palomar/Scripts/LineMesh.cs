using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public enum LineOrientation { Horizontal=0, Vertical, Free }

[ExecuteInEditMode]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (MeshFilter))] 
public class LineMesh : MonoBehaviour 
{
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
	public Vector3[] Points = {};
	[SerializeField]private float _lineWidth = 3f;
	public float LineWidth {
		get {
			return _lineWidth;
		}
		set {
			if (Mathf.Approximately(_lineWidth, value))
				return;
			_lineWidth = value < 0f ? 0f : value;
			DrawLine ();
		}
	}
	public Color LineColor = Color.magenta;
	public bool DrawOnStart, Continuous = true, ShowDebug;
	public float Xscale = 1f, Yscale = 1f;
	public Vector3 Offset;
	public LineOrientation WipeMode = LineOrientation.Horizontal;

	[SerializeField]private float _wipeAmount;
	public float WipeAmount {
		get { return _wipeAmount; }
		set {
			if (Mathf.Approximately(value, _wipeAmount)) 
				return;
			_wipeAmount = Mathf.Clamp01(value);
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
		if (WipeAmount < 0.001f)
			return;

		SplitQuads = new List<Vector3[]>();
		SourceSlices = new List<Slice2D>();
        PolyLine = new List<Vector3>();

	    // Adjusted Points are trimmed (fillamount) then scaled
		var adjPoints = Points.AdjustPoints(WipeAmount, WipeMode, Xscale, Yscale, 
											ShowDebug)
											.OffsetPoints(Offset);

		SourceSlices = InitializeQuads(adjPoints, SourceSlices);
		FinalSlices = Continuous ? SetAveragedSlices (SourceSlices) : SourceSlices;

        // Prepare vertices
		for (int index = 0; index < FinalSlices.Count; index++) {
			//if (HasHat && index == 0) continue;
			Slice2D slice = FinalSlices[index];
			PolyLine.Add(slice.P1);
			PolyLine.Add(slice.P2);
		}
		ThisMesh.vertices = PolyLine.ToArray();
		MeshTools.AssignDefaultUvs (ThisMesh);
        if (Continuous)
			MeshTools.CreateConnectedMesh (ThisMesh, FinalSlices);
        else {
			MeshTools.CreateSplitMesh (ThisMesh, FinalSlices);
        }
        //ThisMaterial.EnableKeyword("_EMISSION");       // Unity5 Standard Material Requirement
		//"_EmissionColor" 
		ThisMaterial.SetColor("_Color", LineColor);
	}

	/// <summary> Initialize the separate quads </summary>
	/// <param name="adjPoints"></param>
	private List<Slice2D> InitializeQuads(Vector3[] adjPoints, List<Slice2D> source ) 
	{
		var inc = Continuous ? 1 : 2;
		for (int i = 0; i < adjPoints.Length - 1; i = i + inc) {
			var quad = MakeQuad(adjPoints[i], adjPoints[i + 1], LineWidth);
			SplitQuads.Add(quad);
			// We use 0, 1, 3, 2 ordering to keep segments aligned relative to +Y
			source.Add(new Slice2D(quad[0], quad[1]));
			source.Add(new Slice2D(quad[3], quad[2]));
		}
		return source;
	}

	// Initial and last slices are the same as in the split quads, others are averaged
	List<Slice2D> SetAveragedSlices (List<Slice2D> source) {
		if (source.Count < 2) {
			Debug.LogError("Error: Empty list of slices");
			return source;
		}
	    var newSlices = new List<Slice2D>();
		newSlices.Add (source [0]);
		for (int i = 1; i < source.Count - 1; i=i+2) {
			// That's the average line in connection segments (cross-section slices)
			var a1 = (source [i].P1 + source [i+1].P1) / 2;
			var a2 = (source [i].P2 + source [i+1].P2) / 2;
			if (ShowDebug) Debug.DrawLine (a1, a2, Color.blue, 30f);
			var medianLineDir = a2 - a1; 					// point upwards

			var topLine = (source [i].P2 - source [i-1].P2);
			var botLine = (source [i].P1 - source [i-1].P1);

			//Debug.DrawLine (source [i].P2, source [i-1].P2, Color.magenta, 30f);
		
			Vector3 cornerTop, cornerBot;
			// Now we need to extend the median line to any top line (previous chosen), to get the right angled corner
			// parms: top line origin, top line direction, median line origin, median line direction
			Vec3Tools.LineLineIntersection (out cornerTop, source [i].P2, topLine, a2, medianLineDir, true);
			Vec3Tools.LineLineIntersection (out cornerBot, source[i].P1, botLine, a1, medianLineDir, true);
			newSlices.Add (new Slice2D (cornerBot, cornerTop));
		}
		newSlices.Add (source [source.Count - 1]);
	    return newSlices;
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

}
