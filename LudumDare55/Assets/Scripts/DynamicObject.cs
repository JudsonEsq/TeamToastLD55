using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DynamicObject : MonoBehaviour {

	[Serializable]
	public class OnCollision : UnityEvent {
	}

	public OnCollision OnCollisionHit;
	
	[Tooltip("The Rigidbody component attached to this object")]
	public Rigidbody RigidBody;

	[Tooltip("The optimal rotation when this object is being held")]
	public Quaternion OptimalHoldRotation;

	[Tooltip("The direction in which this object will be launched")]
	public Vector3 LaunchDirection;

	[Tooltip("The additional distance from the holder when this object is being held")]
	public float AdditionalHoldDistance;

	public bool Launched;

	public bool IsGrabbed { get; private set; }
	public bool CanBeGrabbed => !IsGrabbed;
	public bool IsGrounded { get; private set; }

	
	float _lastSpeed;
	Vector3 _lastVelocity;
	bool _touching;
	
	GameObject _grabber;


	#region Grabbing

	// these are virtual methods so that we can override them in a child class
	public virtual void StartGrab (GameObject grabber) {
		_grabber = grabber;
		
		Launched = false;
		IsGrabbed = true;
	}
	public virtual void StopGrab () {
		_grabber = null;
		IsGrabbed = false;
	}

	#endregion

	public void FixedUpdate () {
		if (!_touching) {
			_lastVelocity = RigidBody.velocity;
			_lastSpeed = _lastVelocity.magnitude;
		}

		// this is a hacky ground check
		// if we're grounded, we have no speed
		if (Mathf.Round(RigidBody.velocity.y) == 0 && RigidBody.velocity.magnitude < 0.5f) {
			_lastSpeed = 0;
			_lastVelocity = Vector3.zero;
			IsGrounded = true;
		} else {
			IsGrounded = false;
		}
	}
	public void OnCollisionEnter (Collision other) {
		_touching = true;

		// if we hit something and we've just been launched
		if (Launched && !IsGrabbed) {
			OnCollisionHit.Invoke();
		}
	}
	
	public void OnCollisionExit (Collision other) {
		_touching = false;
	}
}