using UnityEngine;

//[AddComponentMenu("RageTools/Rage Constraint")]
/// <summary> Constrains (follows) a certain object transform position, rotation and scale, selectively </summary>
//[ExecuteInEditMode]
public class Constraint : MonoBehaviour {

	[SerializeField]private GameObject _follower;
	public GameObject Follower {
		get { return _follower; }
		set {
			if (_follower != null && _follower == value) return;
			_follower = value;
		}
	}
    [HideInInspector][SerializeField]private Transform _followerTransform;
	public Transform FollowerTransform {
		get {	if (_followerTransform == null) 
					_followerTransform = Follower.transform;
				return _followerTransform; }
		set {	_followerTransform = value; }
	}
	[HideInInspector]public bool FollowPosition, FollowPositionX = true, FollowPositionY = true, FollowPositionZ = true;
	[HideInInspector]public bool FollowRotation, FollowRotationX = true, FollowRotationY = true, FollowRotationZ = true;
	[HideInInspector]public bool FollowScale, FollowScaleX = true, FollowScaleY = true, FollowScaleZ = true;
	public bool Local;

	[SerializeField]private bool _visible;
	public bool Visible {
		get { return _visible; }
		set {
			_visible = value;
//            if (FollowerIsGroup) { FollowerGroup.Visible = value; return; }
//            if (FollowerIsSpline) FollowerSpline.Visible = value;
		}
	}

	[HideInInspector]
	public float RotationSnap;
	[HideInInspector]
	public float PositionSnap;
	[HideInInspector]
	public float ScaleSnap;

    public void Update() {
		if (Follower == null) return;
		UpdateFollower();
	}

	private void UpdateFollower( ) {
		if (FollowPosition) CopyPosition();

		if (FollowRotation) CopyRotation();

		if (FollowScale) CopyScale();
	}

	private void CopyScale( ) {
		if (!FollowerTransform.localScale.Equals (transform.localScale))
			FollowerTransform.localScale = Mathf.Approximately(ScaleSnap, 0f) ? transform.localScale
											: Vector3.Lerp(Follower.transform.localScale, transform.localScale, ScaleSnap * Time.deltaTime);
		//TODO: .lossyScale
	}

	private void CopyPosition( ) {
		if (Local) {
			if (!FollowerTransform.localPosition.Equals(transform.localPosition))
				FollowerTransform.localPosition = transform.localPosition;
			return;
		}
		CopyTransformPosition();
	}

    private void CopyTransformPosition( ) {
		if (FollowerTransform.position.Equals(transform.position)) return;
		Vector3 targetPositon = (FollowPositionX && FollowPositionY && FollowPositionZ)? 
								transform.position
		                        : new Vector3 (	FollowPositionX? transform.position.x : FollowerTransform.position.x,
												FollowPositionY? transform.position.y : FollowerTransform.position.y,
												FollowPositionZ? transform.position.z : FollowerTransform.position.z);
		FollowerTransform.position = Mathf.Approximately(PositionSnap, 0f)
										? targetPositon
										: Vector3.Lerp(FollowerTransform.position, targetPositon, PositionSnap * Time.deltaTime);
	}

	private void CopyRotation( ) {
		if (Local) {
			if (!FollowerTransform.localRotation.Equals(transform.localRotation))
				FollowerTransform.localRotation = transform.localRotation;
			return;
		}
		CopyTransformRotation();
	}

    private void CopyTransformRotation() {
        if (!FollowerTransform.rotation.Equals(transform.rotation))
            FollowerTransform.rotation = Mathf.Approximately(RotationSnap, 0f)
                                             ? transform.rotation
                                             : Quaternion.Slerp(FollowerTransform.rotation, transform.rotation,
                                                                RotationSnap*Time.deltaTime);
    }

    // v^v^v^v^v^
	// SWITCHSETS 
	// v^v^v^v^v^
	// (Pass-through methods)

	/// <summary> Switches the first occurrence of an Item Id found in the switchsets </summary>
//	public void SwitchsetItem (string switchItem) {
//		if (!FollowerIsGroup) return;
//		FollowerGroup.SwitchsetItem(switchItem);
//	}

}