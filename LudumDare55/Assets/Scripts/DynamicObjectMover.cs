#region

using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

#endregion

public class DynamicObjectMover : MonoBehaviour {

	public float ThrowForce = 10;
	
	#region Components/GameObjects

	[FormerlySerializedAs("_grabLayer")]
	[Header("Main")]
	[Tooltip("The layer which the gun can grab objects from")]
	public LayerMask GrabLayerMask;

	[FormerlySerializedAs("cameraController")]
	public CameraController CameraController;

	public Handimator Handimator;
	
	#endregion

	#region Distance

	[Header("Distance")]
	[FormerlySerializedAs("MinObjectDistance")]
	[FormerlySerializedAs("_minObjectDistance")]
	[SerializeField, Tooltip("The min distance the object can be from the player")]
	public float MinimumObjectDistance = 2.5f;
	[FormerlySerializedAs("MinObjectDistanceMoving")]
	[SerializeField, Tooltip("The min distance the object can be from the player while moving")]
	public float MinimumObjectDistanceMoving = 6.5f;


	/// <summary>The maximum distance at which a new object can be picked up</summary>
	[FormerlySerializedAs("MaxGrabDistance")]
	[FormerlySerializedAs("_maxGrabDistance")]
	[Tooltip("The maximum distance at which a new object can be picked up")]
	public float MaximumGrabDistance = 50f;

	bool _distanceChanged;

	#endregion

	public float Multiplier = 1f;
	public float DeltaTimeMultiplier = 0.3f;

	#region Properties

	public Vector3 StartPoint { get; private set; }
	public Vector3 MidPoint { get; private set; }
	public Vector3 EndPoint { get; private set; }
	public bool HasObject => _grabbedRigidbody;

	#endregion

	#region ObjectInfo

	Rigidbody _grabbedRigidbody;
	Transform _grabbedTransform;
	Vector3 _hitOffsetLocal;
	float _currentGrabDistance;
	RigidbodyInterpolation _initialInterpolationSetting;

	#endregion

	//Vector3.Zero and Vector2.zero create a new Vector3 each time they are called so these simply save that process and a small amount of cpu runtime.
	readonly Vector3 _zeroVector3 = Vector3.zero;
	readonly Vector3 _oneVector3 = Vector3.one;


	bool _justReleased;
	bool _wasKinematic;
	Collider _collider;
	float _drainTimer;

	DynamicObject _teleObject;
	bool _hadObject;
	int _frames;
	public ObjectGrabbed OnObjectGrabbed;
	public ObjectReleased OnObjectReleased;
	PlayerMovement _movement;
	public delegate void ObjectGrabbed (GameObject obj);

	public delegate void ObjectReleased (GameObject obj);


	#region Unity Update Loop

	void Start () {
		_movement = GetComponent<PlayerMovement>();
		// we sub the before refresh event so that we can release the object and then
		// have it set its position and velocity and have that velocity and position be unmodified
		CameraController = CameraController.Instance;

		if (!CameraController) {
			Debug.LogError($"{nameof(DynamicObjectMover)} missing Camera", this);
			return;
		}

		// PLay idle animation when game starts, this might need to be changed based on the scene, 
		// since there are different animations for the menu scene
		if (Handimator!= null)
		{
			Handimator.PlayIdleAnimation();
		}
	}
	void FixedUpdate () {
		if (_grabbedRigidbody) {

			Ray ray = CenterRay(true);
			// Apply any intentional rotation input made by the player & clear tracked input
			// Remove all torque, reset rotation input & store the rotation difference for next FixedUpdate call
			_grabbedRigidbody.angularVelocity = _zeroVector3;

			// Calculate object's center position based on the offset we stored
			// NOTE: We need to convert the local-space point back to world coordinates
			// Get the destination point for the point on the object we grabbed
			float actualGrabDistance = _currentGrabDistance;
			
			// get the forward vector of the player
			float forward = _movement.VerticalInput;
			if (forward == 1) {
				actualGrabDistance = MinimumObjectDistanceMoving;
			}
			Vector3 holdPoint = ray.GetPoint(actualGrabDistance);
			Vector3 centerDestination = holdPoint; // - _grabbedTransform.TransformVector(_hitOffsetLocal);
			centerDestination.y -= _collider.bounds.size.y/2;

			// Find vector from current position to destination
			Vector3 toDestination = centerDestination - _grabbedTransform.position;
			float mass = _grabbedRigidbody.mass;
			mass = Mathf.Clamp(Mathf.Round(mass/4f), 2.5f, 10f);

			// Calculate force
			Vector3 force = toDestination/Time.fixedDeltaTime*DeltaTimeMultiplier/mass*Multiplier;

			//lerp towards optimal rotation
			if (_teleObject) {
				_grabbedRigidbody.rotation = Quaternion.Lerp(_grabbedRigidbody.rotation, _teleObject.OptimalHoldRotation, 10*Time.fixedDeltaTime);
			}

			// Remove any existing velocity and add force to move to final position
			_grabbedRigidbody.velocity = _zeroVector3;
			_grabbedRigidbody.AddForce(force, ForceMode.VelocityChange);

			//We need to recalculte the grabbed distance as the object distance from the player has been changed
			if (_distanceChanged) {
				_distanceChanged = false;
				_currentGrabDistance = Vector3.Distance(ray.origin, holdPoint);
			}

			//Update public properties
			StartPoint = CameraController.Position;
			MidPoint = holdPoint;
			EndPoint = _grabbedTransform.TransformPoint(_hitOffsetLocal);
		}
	}

	void Update () {

		if (!_grabbedRigidbody && _hadObject) {
			ReleaseObject();
			_justReleased = true;
		}
		// replace this with a input call
		if (!Input.GetKey(KeyCode.Mouse0)) {
			_justReleased = false;
		}

		if (Input.GetKey(KeyCode.Mouse0) && !_grabbedRigidbody && !_justReleased) {
			Debug.Log("GrabObject");
			GrabObject();
		} else if (_grabbedRigidbody) {
			if (Input.GetKeyDown(KeyCode.Mouse1) || !IsObjectInRange()) {
				ReleaseObject();
				_justReleased = true;
				return;
			}

			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				ReleaseObject(true);
				_justReleased = true;
			}
		}
	}

	#endregion

	#region Other

	/// <returns>Ray from center of the main camera's viewport forward</returns>
	Ray CenterRay (bool offset = false) {
		if (offset) {

			return CameraController.ViewportPointToRay(_oneVector3*0.5f + new Vector3(0, 0.07f, 0));

		}
		return CameraController.ViewportPointToRay(_oneVector3*0.5f);
	}

	//Check distance is within range when moving object with the scroll wheel
	bool IsObjectInRange () {
		Vector3 pointA = transform.position;
		Vector3 pointB = _grabbedRigidbody.position;

		float distance = Vector3.Distance(pointA, pointB);
		float additionalDistance = _teleObject ? _teleObject.AdditionalHoldDistance : 0;
		additionalDistance += MinimumObjectDistanceMoving;
		return distance <= MaximumGrabDistance + additionalDistance;
	}

	#endregion

	#region Grabbing

	void GrabObject () {
		// We are not holding an object, look for one to pick up

		Ray ray = CenterRay();

		// single, pixel perfect raycast
		if (Physics.Raycast(ray, out RaycastHit hit, MaximumGrabDistance, GrabLayerMask | LayerMask.GetMask("Environment"), QueryTriggerInteraction.Ignore)) {
			// Don't pick up kinematic rigidbodies (they can't move)
			if (hit.rigidbody && !hit.collider.isTrigger && !hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Environment"))) {
				SetObject(hit.collider);
				_hitOffsetLocal = hit.transform.InverseTransformVector(hit.point - hit.transform.position);
				return;
			}

		}
		// if the raycast didn't hit, do an overlap sphere to find a grabbable object
		if (CameraController.EnvironmentRaycast(out RaycastHit envHit, MaximumGrabDistance)) {
			Collider[] colliders = Physics.OverlapSphere(envHit.point, 2.5f, GrabLayerMask);
			int count = colliders.Length;
			Debug.Log(count);
			if (count > 0) {
				// find the closest collider
				Vector2[] points = new Vector2[count];
				for (int i = 0; i < count; i++) {
					points[i] = CameraController.WorldToScreenPoint(colliders[i].transform.position);
				}
				// find the closest point to the center of the screen
				Vector2 center = CameraController.WorldToScreenPoint(envHit.point);
				// draw a 2d line from the center of the screen to the closest point
				
				Collider closest = null;
				float distance = float.MaxValue;
				for (int i = 0; i < count; i++) {
					float dist = Vector2.Distance(center, points[i]);
					if (dist < distance) {
						distance = dist;
						closest = colliders[i];
					}
				}

				if (closest) {
					Debug.DrawLine(envHit.point, closest.transform.position, Color.green, 5);
					SetObject(closest);
					_hitOffsetLocal = envHit.transform.InverseTransformVector(envHit.point - envHit.transform.position);
				}
			}
		}
	}

	void SetObject (Collider objCollider) {
		Debug.Log("SetObject: " + objCollider.name);

		// check for a telekinesis object
		_teleObject = objCollider.GetComponent<DynamicObject>();
		if (_teleObject && _teleObject.CanBeGrabbed) {
			_teleObject.StartGrab(gameObject);
		} else if (_teleObject && !_teleObject.CanBeGrabbed) {
			return;
		}

		// Play pickup animation
		Handimator.PlayPickupAnimation();

		_collider = objCollider;

		// Track rigidbody's initial information
		_grabbedRigidbody = objCollider.GetComponent<Rigidbody>();
		if (!_grabbedRigidbody) return;
		_wasKinematic = _grabbedRigidbody.isKinematic;
		_grabbedRigidbody.isKinematic = false;
		_grabbedRigidbody.freezeRotation = true;

		_initialInterpolationSetting = _grabbedRigidbody.interpolation;
		_currentGrabDistance = MinimumObjectDistance; // Vector3.Distance(ray.origin, hit.point);

		_grabbedTransform = _grabbedRigidbody.transform;
		_grabbedRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

		//  HasObject = true;
		OnObjectGrabbed?.Invoke(_grabbedRigidbody.gameObject);
		_hadObject = true;
		
		// pickup should transition to hold on the animator graph
		//Handimator.PlayHoldAnimation();
	
		//Physics.IgnoreCollision(_collider, GetComponent<Collider>(), true);

	}

	void ReleaseObject (bool forceRelease = false) {

		if (!_grabbedRigidbody) {
			_grabbedRigidbody = null;
			_grabbedTransform = null;
			StartPoint = _zeroVector3;
			MidPoint = _zeroVector3;
			EndPoint = _zeroVector3;
			_collider = null;
			OnObjectReleased?.Invoke(null);
			_hadObject = false;
			return;
		}


		Vector3 GrabPlayerReleaseDirection (Vector3 worldCenter) {
			// This code caused some weird issues, but I know I wrote it for a reason.
			// I can't remember why, though.
			/*if (CameraController.EnvRaycast(out RaycastHit hit, 20f) && _motor.Velocity.magnitude > 5.4f) {
				return (hit.point - _grabbedRigidbody.transform.position).normalized;
			}*/
			Vector3 point = worldCenter + CameraController.Forward*20;
			return (point - _grabbedRigidbody.transform.position).normalized;
		}

		//Move rotation to desired rotation in case the lerp hasn't finished
		// Reset the rigidbody to how it was before we grabbed it
		if (_teleObject) {
			_teleObject.StopGrab();
		}

		_grabbedRigidbody.isKinematic = _wasKinematic;
		_grabbedRigidbody.interpolation = _initialInterpolationSetting;
		_grabbedRigidbody.freezeRotation = false;

		Vector3 additionalForce = Vector3.zero;

		Vector3 screenCenter = new Vector3(Screen.width/2F, Screen.height/2F, 0);
		Vector3 worldCenter = CameraController.ScreenToWorldPoint(screenCenter);
		Vector3 direction = GrabPlayerReleaseDirection(worldCenter);

		if (forceRelease) {
			//	Debug.Log(direction);
			//bool useTarget = false;



			_grabbedRigidbody.velocity = _zeroVector3;
			_grabbedRigidbody.angularVelocity = _zeroVector3;

			if (_teleObject) {
				_teleObject.LaunchDirection = direction;
				_teleObject.Launched = forceRelease;
			}

//					Debug.Log(direction);
			Vector3 velocity = direction*(ThrowForce) + additionalForce;
			_grabbedRigidbody.AddForce(velocity, ForceMode.VelocityChange);
			Handimator.PlayThrowAnimation();

		} else {
			Handimator.PlayDropAnimation();
			_grabbedRigidbody.velocity *= 0.45f;
		}
		//Physics.IgnoreCollision(_collider, GetComponent<Collider>(), false);

		_grabbedRigidbody = null;
		_grabbedTransform = null;
		StartPoint = _zeroVector3;
		MidPoint = _zeroVector3;
		EndPoint = _zeroVector3;
		_collider = null;
		// HasObject = false;
		OnObjectReleased?.Invoke(null);
	}

	#endregion
}