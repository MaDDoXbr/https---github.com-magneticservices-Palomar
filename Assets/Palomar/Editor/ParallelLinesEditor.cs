using UnityEditor;
using EzEditor;
using UnityEngine;

[CustomEditor(typeof (ParallelLines))]
public class ParallelLinesEditor : Editor {

	private ParallelLines _target;

	public override void OnInspectorGUI() {
        if (_target == null) 
			_target = target as ParallelLines;
		if (_target == null) 
			return;

		using (gui.Horizontal()) {
			gui.LookLikeControls (40f, 60f);
			_target.Type = (LineType)gui.EzEnumPopup ("Type", _target.Type, 15f);
			_target.LineCount = gui.EzIntField ("Line Count", _target.LineCount, 10f);
		}
		using (gui.Horizontal ()) {
			_target.HasHat = gui.EzToggle ("Hat Line", _target.HasHat);
			if (_target.HasHat) {
				gui.LookLikeControls(40f,10f);
				_target.StartPad = gui.EzFloatField("Pad In", _target.StartPad, 10f);
				_target.EndPad = gui.EzFloatField ("Pad Out", _target.EndPad, 10f);
			}
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

		//base.OnInspectorGUI();
		if (gui.EzButton("Update & Draw"))
			_target.CreateLines();
	}
}
