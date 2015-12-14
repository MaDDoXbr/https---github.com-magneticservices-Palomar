using UnityEditor;
using EzEditor;
using UnityEngine;

[CustomEditor (typeof (DonutGraph))]
public class DonutGraphEditor : Editor 
{
	private DonutGraph _target;
	[SerializeField] private int _idx = 1;
	public override void OnInspectorGUI() {
        if (_target == null) _target = target as DonutGraph;
		DrawDefaultInspector();
		using (gui.Horizontal()) {
			if (gui.EzButton ("Import XML Data", GUILayout.Height (24f))) {
				_target.ImportXML (_idx);				
			}
			_idx = Mathf.Clamp ((gui.EzIntField ("Idx", _idx, 10f, GUILayout.Width (60f))), 1, 4);
		}

		EditorUtility.SetDirty (target);
	}
}
