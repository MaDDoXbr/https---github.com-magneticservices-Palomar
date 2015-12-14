using UnityEngine;
using System.Collections;
using UnityEditor;
using EzEditor;

[CustomEditor(typeof (Constraint))]
public class ConstraintEditor : Editor
{
	private Constraint _target;

	public override void OnInspectorGUI() {
		if (_target == null) _target = target as Constraint;

		DrawDefaultInspector();

		using (gui.Horizontal()) {
			_target.FollowPosition = gui.EzToggle ("Follow Pos", _target.FollowPosition, GUILayout.Width(95f));
			if (_target.FollowPosition) {
				_target.FollowPositionX = gui.EzToggle ("x", _target.FollowPositionX);
				_target.FollowPositionY = gui.EzToggle ("y", _target.FollowPositionY);
				_target.FollowPositionZ = gui.EzToggle ("z", _target.FollowPositionZ);
				_target.PositionSnap = gui.EzFloatField ("Snap", _target.PositionSnap, 10f, GUILayout.Width (75f));
			}
		}

		using (gui.Horizontal ()) {
			_target.FollowRotation = gui.EzToggle ("Follow Rot", _target.FollowRotation, GUILayout.Width (95f));
			if (_target.FollowRotation) {
				_target.FollowRotationX = gui.EzToggle ("x", _target.FollowRotationX);
				_target.FollowRotationY = gui.EzToggle ("y", _target.FollowRotationY);
				_target.FollowRotationZ = gui.EzToggle ("z", _target.FollowRotationZ);
				_target.RotationSnap = gui.EzFloatField ("Snap", _target.RotationSnap, 10f, GUILayout.Width(75f));
			}
		}

		using (gui.Horizontal ()) {
			_target.FollowScale = gui.EzToggle ("Follow Scl", _target.FollowScale, GUILayout.Width (95f));
			if (_target.FollowScale) {
				_target.FollowScaleX = gui.EzToggle ("x", _target.FollowScaleX);
				_target.FollowScaleY = gui.EzToggle ("y", _target.FollowScaleY);
				_target.FollowScaleZ = gui.EzToggle ("z", _target.FollowScaleZ);
				_target.ScaleSnap = gui.EzFloatField ("Snap", _target.ScaleSnap, 10f, GUILayout.Width (75f));
			}
		}

		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}
}
