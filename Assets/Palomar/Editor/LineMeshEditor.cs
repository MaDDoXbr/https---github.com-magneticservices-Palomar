using UnityEditor;
using EzEditor;
using UnityEngine;

[CustomEditor(typeof (LineMesh))]
public class LineMeshEditor : Editor {
    public static bool showPoints;

    private LineMesh _lineMesh;
    private bool _autoSetup;
    private Transform _newTransform;
    private Vector3 newValue;


    public override void OnInspectorGUI() 
	{
        if (_lineMesh == null) _lineMesh = target as LineMesh;
        _lineMesh.DrawOnStart = gui.EzToggle("On Start", _lineMesh.DrawOnStart, GUILayout.Width(75f));
        using (gui.Horizontal()) {
            _lineMesh.FillMode = (LineType)gui.EzEnumPopup("Fill Mode", _lineMesh.FillMode);
            _lineMesh.FillAmount = gui.EzFloatField("Amount", _lineMesh.FillAmount, 13f);
        }
        using (gui.Horizontal()) {
            _lineMesh.LineColor = gui.EzColorField("Color", _lineMesh.LineColor, 10f, GUILayout.Width(85f));
            gui.EzSpacer(10f);
            _lineMesh.LineWidth = gui.EzFloatField("Line Width", _lineMesh.LineWidth, 8f);
        }
        using (gui.Horizontal()) {
            _lineMesh.Xscale = gui.EzFloatField("X Scale", _lineMesh.Xscale, 10f);
            _lineMesh.Yscale = gui.EzFloatField("Y Scale", _lineMesh.Yscale, 7f);
        }
	    _lineMesh.Offset = gui.EzV3Field("Offset", _lineMesh.Offset, 5f, GUILayout.Width(200f));
        _lineMesh.Points = gui.EzV3Array("Points", _lineMesh.Points, ref newValue, ref showPoints);
        using (gui.Horizontal()) {
            if (gui.EzButton("Draw")) {
                _lineMesh.DrawLine();
            }
            _lineMesh.Continuous = gui.EzToggle("Continuous", _lineMesh.Continuous, GUILayout.Width(85f));
            _lineMesh.debug = gui.EzToggle("Debug", _lineMesh.debug, GUILayout.Width(75f));
        }

//
//        _lineMesh.JointSize = gui.EzFloatField("Joint Size", _lineMesh.JointSize);

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
