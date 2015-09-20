using UnityEditor;
using EzEditor;
using UnityEngine;

[CustomEditor(typeof (ParallelLines))]
public class ParallelLinesEditor : Editor {

	private ParallelLines _target;

	public override void OnInspectorGUI() {
        if (_target == null) 
			_target = target as ParallelLines;


		_target.Type = (LineType)gui.EzEnumPopup ("Type", _target.Type, 45f);
//		gui.LookLikeControls(70f);
		base.OnInspectorGUI();
		if (gui.EzButton("Update & Draw"))
			_target.CreateLines();
	}
}
