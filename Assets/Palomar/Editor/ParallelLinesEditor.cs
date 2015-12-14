using UnityEditor;
using EzEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof (ParallelLines))]
public class ParallelLinesEditor : Editor {

	private ParallelLines _target;
	private Text _newMarker = null;
	private bool _showMarker;

	public override void OnInspectorGUI() {
        if (_target == null) 
			_target = target as ParallelLines;
		if (_target == null) 
			return;

		using (gui.Horizontal()) {
			gui.LookLikeControls (40f, 60f);
			_target.Along = (LineOrientation)gui.EzEnumPopup ("Along", _target.Along, 10f);
			_target.LineCount = gui.EzIntField ("Line Count", _target.LineCount, 10f, GUILayout.Width(100f));
			_target.WipeAmount = gui.EzFloatField ("%", _target.WipeAmount, 10f, GUILayout.Width (40f));
		}
		using (gui.Horizontal()) {
			_target.StartX = gui.EzFloatField ("Start X", _target.StartX, 10f);
			_target.EndX = gui.EzFloatField ("End X", _target.EndX, 10f);
		}
		using (gui.Horizontal()) {
			_target.StartY = gui.EzFloatField ("Start Y", _target.StartY, 10f);
			_target.EndY = gui.EzFloatField ("End Y", _target.EndY, 10f);			
		}
		using (gui.Horizontal ()) {
			_target.StepX = gui.EzFloatField ("Step X", _target.StepX, 10f);
			_target.StepY = gui.EzFloatField ("Step Y", _target.StepY, 10f);
		}
		_target.HatLine = gui.EzObjectField("Hat Line", _target.HatLine, 10f) as LineMesh;
		if (_target.HasHat) {
			using (gui.Horizontal ()) {
					gui.LookLikeControls(40f,10f);
					_target.StartPad = gui.EzFloatField("Pad In", _target.StartPad, 10f);
					_target.EndPad = gui.EzFloatField ("Pad Out", _target.EndPad, 10f);
			}
		}
		_target.MarkerTexts = gui.EzObjectArray ("Marker Texts", _target.MarkerTexts, ref _newMarker, ref _showMarker);

		if (gui.EzButton("Update & Draw"))
			_target.CreateLines();
	}
}
