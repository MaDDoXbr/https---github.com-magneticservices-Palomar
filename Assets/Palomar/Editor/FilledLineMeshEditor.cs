using UnityEditor;
using EzEditor;
using UnityEngine;

[CustomEditor (typeof (FilledLineMesh))]
public class FilledLineMeshEditor : Editor {
    public static bool showPoints;

    private FilledLineMesh _lineMesh;
    private bool _autoSetup;
    private Transform _newTransform;
    private Vector3 newValue;

    public override void OnInspectorGUI() 
	{
        if (_lineMesh == null) _lineMesh = target as FilledLineMesh;
        _lineMesh.DrawOnStart = gui.EzToggle("On Start", _lineMesh.DrawOnStart, GUILayout.Width(75f));
        using (gui.Horizontal()) {
            _lineMesh.WipeMode = (LineOrientation)gui.EzEnumPopup("Wipe Mode", _lineMesh.WipeMode, 20f);
            _lineMesh.WipeAmount = gui.EzFloatField("%", _lineMesh.WipeAmount, 13f);
        }
        using (gui.Horizontal()) {
            _lineMesh.LineColor = gui.EzColorField("Color", _lineMesh.LineColor, 10f, GUILayout.Width(85f));
	        _lineMesh.BaseHeight = gui.EzFloatField("Base Height", _lineMesh.BaseHeight, 10f);
        }
        using (gui.Horizontal()) {
            _lineMesh.Xscale = gui.EzFloatField("X Scale", _lineMesh.Xscale, 10f);
            _lineMesh.Yscale = gui.EzFloatField("Y Scale", _lineMesh.Yscale, 7f);
        }
	    _lineMesh.Offset = gui.EzV3Field("Offset", _lineMesh.Offset, 5f, GUILayout.Width(200f));
        _lineMesh.Points = gui.EzV3Array("Points", _lineMesh.Points, ref newValue, ref showPoints);
        using (gui.Horizontal()) {
            if (gui.EzButton("Draw")) {
                _lineMesh.DrawFilledLine();
            }
            _lineMesh.ShowDebug = gui.EzToggle("Debug", _lineMesh.ShowDebug, GUILayout.Width(75f));
        }

//
//        _lineMesh.JointSize = gui.EzFloatField("Joint Size", _lineMesh.JointSize);

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
