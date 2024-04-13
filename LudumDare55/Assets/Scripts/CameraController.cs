#region

using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

	public class CameraController : MonoBehaviour {
		public float LookScale = 1;

		public Camera PlayerCamera;
		public static CameraController Instance { get; private set; }
		#region Properties

		public Vector3 PlanarForward => Vector3.Scale(_plane, PlayerCamera.transform.forward).normalized;
		public Vector3 PlanarRight => -Vector3.Cross(PlanarForward, Vector3.up).normalized;

		public Vector3 Up {
			get => PlayerCamera.transform.up;
			set => PlayerCamera.transform.up = value;
		}

		public Vector3 Position {
			get => PlayerCamera.transform.position;
			set => PlayerCamera.transform.position = value;
		}

		public Vector3 Forward {
			get => PlayerCamera.transform.forward;
			set => PlayerCamera.transform.forward = value;
		}

		public Vector3 Right {
			get => PlayerCamera.transform.right;
			set => PlayerCamera.transform.right = value;
		}

		public Quaternion Rotation {
			get => PlayerCamera.transform.rotation;
			set => PlayerCamera.transform.rotation = value;
		}

		public Vector3 PlanarDirection { get; set; }

		public float Yaw { get; private set; }
		public float YawFutureInterpolation { get; private set; }
		public float YawIncrease { get; private set; }
		public float Pitch { get; private set; }
		public float PitchFutureInterpolation { get; private set; }
		public float CameraRoll { get; private set; }

		#endregion

		float _cameraRotation;
		float _cameraRotationSpeed;
		float _previousYVelocity;
		float _threeSixtyCounter;
		
		readonly Vector3 _plane = new Vector3(1, 0, 1);

		#region Unity Update Loop

		void Awake () {

			Cursor.lockState = CursorLockMode.Locked;
			PlanarDirection = Vector3.forward;
			if (!Instance) {
				Instance = this;
			} else {
				Destroy(this);
			}
		}
		void Update () {
			if (Time.timeScale > 0) {
				YawIncrease = Input.GetAxis("Mouse X")*(1f/10)*LookScale;
				// this is for controller, i'm just too lazy to look up what it's rewired to by default
				// YawIncrease += Input.GetAxis("Joy 1 X 2")*1*LookScale; 

				Yaw = (Yaw + YawIncrease)%360f;


				float yawinterpolation = Mathf.Lerp(Yaw, Yaw + YawFutureInterpolation, Time.deltaTime*10) - Yaw;
				Yaw += yawinterpolation;
				YawFutureInterpolation -= yawinterpolation;

				Pitch -= Input.GetAxis("Mouse Y")*(1f/10)*LookScale;
				// this is for controller, i'm just too lazy to look up what it's rewired to by default
				// Pitch += Input.GetAxis("Joy 1 Y 2")*1f*LookScale;

				float pitchinterpolation = Mathf.Lerp(Pitch, Pitch + PitchFutureInterpolation, Time.deltaTime*10) - Pitch;
				Pitch += pitchinterpolation;
				PitchFutureInterpolation -= pitchinterpolation;

				Pitch = Mathf.Clamp(Pitch, -85, 85);
			}

			_threeSixtyCounter -= Mathf.Min(_threeSixtyCounter, Time.deltaTime*150);
			_threeSixtyCounter += Mathf.Abs(YawIncrease);
			if (_threeSixtyCounter > 240) {
				_threeSixtyCounter -= 240;
			}

			// This is where orientation is handled, the PlayerCamera is only adjusted by the pitch, and the entire player is adjusted by yaw
			PlayerCamera.transform.rotation =
				Quaternion.Euler(new Vector3(Pitch, Yaw, CameraRoll));

			// This value is used to calcuate the positions in between each fixedupdate tick

			CameraRoll -= Mathf.Sign(CameraRoll - _cameraRotation)*Mathf.Min(_cameraRotationSpeed*Time.deltaTime,
				Mathf.Abs(CameraRoll - _cameraRotation));
		}

		#endregion

		#region Wrappers

		public Ray ViewportPointToRay (Vector3 point) {
			return PlayerCamera.ViewportPointToRay(point);
		}

		public Vector3 ScreenToWorldPoint (Vector3 point) {
			return PlayerCamera.ScreenToWorldPoint(point);
		}

		public Vector3 WorldToScreenPoint (Vector3 point) {
			return PlayerCamera.WorldToScreenPoint(point);
		}

		#endregion

		#region Raycasts

		public bool EnvironmentRaycast (out RaycastHit hit, float distance) {
			return Physics.Raycast(Position, Forward, out hit, distance, LayerMask.GetMask("Environment"));
		}
		
		#endregion

}

