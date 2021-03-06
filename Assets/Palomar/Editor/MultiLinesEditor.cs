﻿using UnityEditor;
using EzEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof (MultiLines))]
public class MultiLinesEditor : Editor {

	private MultiLines _target;

	public override void OnInspectorGUI() {
        if (_target == null) 
			_target = target as MultiLines;
		if (_target == null) 
			return;

//		using (gui.Horizontal()) {
//			gui.LookLikeControls (40f, 60f);
//			_target.Along = (LineOrientation)gui.EzEnumPopup ("Along", _target.Along, 15f);
//			_target.LineCount = gui.EzIntField ("Line Count", _target.LineCount, 10f, GUILayout.Width(100f));
//			_target.WipeAmount = gui.EzFloatField ("%", _target.WipeAmount, 10f, GUILayout.Width (40f));
//		}
//		using (gui.Horizontal ()) {
//			_target.HasHat = gui.EzToggle ("Hat Line", _target.HasHat);
//			if (_target.HasHat) {
//				gui.LookLikeControls(40f,10f);
//				_target.StartPad = gui.EzFloatField("Pad In", _target.StartPad, 10f);
//				_target.EndPad = gui.EzFloatField ("Pad Out", _target.EndPad, 10f);
//			}
//		}
//		using (gui.Horizontal()) {
//			_target.StartY = gui.EzFloatField ("Start Y", _target.StartY, 10f);
//			_target.EndY = gui.EzFloatField ("End Y", _target.EndY, 10f);			
//		}
//		using (gui.Horizontal ()) {
//			_target.StepX = gui.EzFloatField ("Step X", _target.StepX, 10f);
//			_target.StepY = gui.EzFloatField ("Step Y", _target.StepY, 10f);
//		}
//		_target.MarkerTexts = gui.EzObjectArray ("Marker Texts", _target.MarkerTexts, ref _newMarker, ref _showMarker);
		DrawDefaultInspector();

//		using (gui.Horizontal()) {
//			_target.WipeAmount = gui.EzFloatField ("Wipe %", _target.WipeAmount, 10f);
//		}
		if (gui.EzButton ("Update & Draw")) {
			_target.DefinePoints();
			_target.DrawLines ();
		}
	}
}
