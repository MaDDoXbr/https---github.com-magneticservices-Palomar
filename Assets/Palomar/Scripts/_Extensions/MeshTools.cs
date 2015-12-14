using UnityEngine;
using System.Collections.Generic;

public static class MeshTools {

	// We don't use textures, so for UVs we just assign the xy 2D coords to them
	public static void AssignDefaultUvs (Mesh mesh)
	{
		var vtxCount = mesh.vertices.Length;
		var uvs = new Vector2[vtxCount];
		for (var i = 0; i < vtxCount; i++) {
			uvs[i] = new Vector2 (mesh.vertices[i].x, mesh.vertices[i].y);
		}
		mesh.uv = uvs;
	}

	public static void CreateSplitMesh (Mesh mesh, List<Slice2D> slices)
	{
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
		mesh.SetTriangles (triIdx, 0);
		mesh.RecalculateNormals ();
	}

	public static void CreateConnectedMesh (Mesh mesh, List<Slice2D> slices)
	{
		var sliceCount = slices.Count;

		if (sliceCount < 2)
			return;
		// Eg.: | | | | | => 5 slices = 8 tris (5 - 1 => 4quads * 2tris/quad = 8), times 3 idx / tri
		var triIdx = new int[(sliceCount - 1) * 6];
		// Eg.: 1 3  3 5
		//      0 2  2 4 ... => 0,1,3, 0,3,2, 2,3,4, 2,5,4
		int i = 0;
		// J is the Triangle Index
		// Will repeat the number of quads (slices-1) * 6 (2 tris)
		for (int j = 0; j < ((sliceCount - 1) * 6); j = j + 6) {
			triIdx[j] = i;
			triIdx[j + 1] = i + 1;
			triIdx[j + 2] = i + 3;
			triIdx[j + 3] = i;
			triIdx[j + 4] = i + 3;
			triIdx[j + 5] = i + 2;
			//Debug.Log(" j:" + j+" "+(j+1)+" "+(j+2)+" "+(j+3)+" "+(j+4)+" "+(j+5));
			i += 2;
		}
		mesh.SetTriangles (triIdx, 0);
		mesh.RecalculateNormals ();
	}

}
