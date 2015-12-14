using UnityEditor;
using EzEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor (typeof (DottedLine))]
public class DottedLineEditor: Editor
{
	private DottedLine _target;
//	private Text _newMarker = null;
//	private bool _showMarker;

	public override void OnInspectorGUI () {
		if (_target == null)
			_target = target as DottedLine;
		if (_target == null)
			return;

		using (gui.Horizontal ()) {
			_target.WipeAmount = gui.EzFloatField ("Wipe %", _target.WipeAmount, 15f, GUILayout.Width (100f));
		}
		using (gui.Horizontal ()) {
			if (gui.EzButton (gui.LoadValuesButton)) {
				if (!_target.StartRef || !_target.EndRef )
					Debug.LogWarning("Start and/or End References not assigned.");
				else {
					_target.Start = _target.StartRef.position;
					_target.End = _target.EndRef.position;
				}
			}
			_target.Start = gui.EzV3Field ("Start",
				_target.Start, 10f, GUILayout.Width (180f));
		}
		using (gui.Horizontal ()) {
			_target.End = gui.EzV3Field ("End",
				_target.End, 10f, GUILayout.Width (180f));
		}
		gui.LookLikeControls ();

		base.OnInspectorGUI ();

		if (gui.EzButton ("Update & Draw"))
			_target.CreateLines ();
	}
}

