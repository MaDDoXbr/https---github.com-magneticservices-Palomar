using UnityEditor;
using EzEditor;
using UnityEngine;

[CustomEditor(typeof (ParallelLines))]
public class ParallelLinesEditor : Editor {

	private ParallelLines _target;

	public override void OnInspectorGUI() {
        if (_target == null) 
			_target = target as ParallelLines;

		base.OnInspectorGUI();
		if (gui.EzButton("Update & Draw"))
			_target.CreateLines();
	}
}
