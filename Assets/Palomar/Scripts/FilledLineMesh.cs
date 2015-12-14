using UnityEngine;
using System.Collections.Generic;

// TODO: Point data must be ordered in ascending order. Currently it assumes so.
[ExecuteInEditMode]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (MeshFilter))] 
public class FilledLineMesh : MonoBehaviour 
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
	public List<Vector3> PolyLine;				// Split and Line done with quads, renderable
	public List<Vector3[]> SplitQuads;
	public List<Slice2D> Slices;
	public float BaseHeight = 0f;				// Y Height where the base of the line mesh is
	public Vector3[] Points = {};
	public Color LineColor = Color.magenta;
	public bool DrawOnStart, ShowDebug;
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
			DrawFilledLine();
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

	public void Start() 
    {
		if (DrawOnStart)
			DrawFilledLine();
	}

	public void DrawFilledLine() 
	{
	    if (Points.Length < 2) return;
        ThisMesh = new Mesh();
		if (WipeAmount < 0.0001f)
			return;

		SplitQuads = new List<Vector3[]>();
		Slices = new List<Slice2D>();
        PolyLine = new List<Vector3>();

	    // Adjusted Points are trimmed (fillamount) then scaled
		var adjPoints = Points.AdjustPoints(WipeAmount, WipeMode, Xscale, Yscale, ShowDebug);
		adjPoints = adjPoints.OffsetPoints(Offset);

		Slices = InitializeQuads(adjPoints, Slices);

        // Prepare vertices
		for (int index = 0; index < Slices.Count; index++) {
			Slice2D slice = Slices[index];
			PolyLine.Add(slice.P1);
			PolyLine.Add(slice.P2);
		}
		ThisMesh.vertices = PolyLine.ToArray();
		MeshTools.AssignDefaultUvs (ThisMesh);
		CreateMeshFromSlices(ThisMesh, Slices);

		ThisMaterial.SetColor("_Color", LineColor);
	}

	/// <summary> Initialize the separate quads </summary>
	private List<Slice2D> InitializeQuads(Vector3[] points, List<Slice2D> source ) 
	{
		for (int i = 0; i < points.Length - 1; i++) {
			//var quad = MakeQuad(points[i], points[i + 1], 0);
			// quad convention used => BL, TL, TR, BR 
			var quad = new[] {
								new Vector3(points[i].x, BaseHeight),
								points[i], points[i + 1],
								new Vector3(points[i+1].x, BaseHeight),
							 };
			SplitQuads.Add(quad);
			// We use 0, 1, 3, 2 ordering to keep segments aligned relative to +Y
			source.Add(new Slice2D(quad[0], quad[1]));
			source.Add(new Slice2D(quad[3], quad[2]));
		}
		return source;
	}

    static void CreateMeshFromSlices(Mesh mesh, List<Slice2D> slices)
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
}
