using DG.Tweening;
using UnityEditor;
using EzEditor;
using UnityEngine;

[CustomEditor (typeof (LineGraph))]
public class LineGraphEditor : Editor 
{
	private LineGraph _target;
	[SerializeField]private int _idx = 1;
	public override void OnInspectorGUI() {
		if (_target == null) _target = target as LineGraph;
		DrawDefaultInspector();
		using (gui.Horizontal()) {
			if (gui.EzButton(gui.LoadValuesButton))
				_target.Rot.StartRotation = _target.Pivot.rotation.eulerAngles;
			_target.Rot.StartRotation = gui.EzV3Field("Rot In",
					_target.Rot.StartRotation, 10f, GUILayout.Width(180f));
		}
		using (gui.Horizontal()) {
			if (gui.EzButton(gui.LoadValuesButton))
				_target.Rot.EndRotation = _target.Pivot.rotation.eulerAngles;
			_target.Rot.EndRotation = gui.EzV3Field("Rot Out",
					_target.Rot.EndRotation, 10f, GUILayout.Width(180f));
		}
		using (gui.Horizontal()) {
			_target.Rot.Duration = gui.EzFloatField("Duration", _target.Rot.Duration, 10f);
			_target.Rot.EaseType = (Ease)gui.EzEnumPopup("Ease", _target.Rot.EaseType, 10f);
		}
		using (gui.Horizontal()) {
			if (gui.EzButton("Import XML Data", GUILayout.Height(24f),
					GUILayout.Width(180f))) {
						_target.ImportXML (	_idx );
			}
			_idx = Mathf.Clamp((gui.EzIntField("Idx",_idx, 10f, GUILayout.Width(60f))),1,4);
		}

		EditorUtility.SetDirty( target );
	}
}
