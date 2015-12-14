using UnityEngine;

public enum GraphType { Line, Donut, World, Filled }

public class SceneData : ScriptableObject {
	public string LinePath, DonutPath, WorldPath, FilledPath;
	public int LineIdx, DonutIdx, WorldIdx, FilledIdx;

	/// <summary> This changes for each graph script and idx ; eg.: LineGraph1.xml </summary>
	public string GetImportPath (int idx, GraphType type, bool localFile) 
	{
		string tp = "";
		switch (type) {
			case GraphType.Line:
				tp = LinePath; break;
			case GraphType.Donut:
				tp = DonutPath; break;
			case GraphType.World: 
				tp = WorldPath; break;
			case GraphType.Filled: 
				tp = FilledPath; break;
			default: break;
		}
		var urlPath = tp + idx + ".xml";
		string path;

		if (localFile) {
			if (Application.isEditor)
				path = "file://" + Application.dataPath + "/Palomar/" + urlPath;
			else
				path = "file://" + Application.dataPath + '/' + urlPath;
		} else
			path = urlPath;

		return path;
	}

}
