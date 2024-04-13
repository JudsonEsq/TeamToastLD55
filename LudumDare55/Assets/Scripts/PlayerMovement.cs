using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float MoveSpeed;
	public float MaxSpeed;
	public float Acceleration;
	public float Deceleration;
	public float AirMultiplier;
	public float Gravity;
	public float HorizontalInput;
	public float VerticalInput;
	public float LerpSpeed = 5f; // Speed of interpolation
	Vector2 _currentRawMove;   
	Vector2 _targetRawMove;
	float _currentMoveSpeed; 
	float _targetMoveSpeed;
	
	Vector3 _moveDirection;
	Vector3 _velocity; // New velocity variable
	CharacterController _controller;
	public CameraController CameraController;

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
	}

	private void Update () {
		HorizontalInput = Input.GetAxisRaw("Horizontal Movement");
		VerticalInput = Input.GetAxisRaw("Vertical Movement");

		// Set target raw movement
		_targetRawMove = new Vector2(HorizontalInput, VerticalInput);

		// Smoothly interpolate current raw movement towards target raw movement
		_currentRawMove = Vector2.Lerp(_currentRawMove, _targetRawMove, LerpSpeed * Time.deltaTime);
		_moveDirection = GetWishDirection(_currentRawMove);

		// handle acceleration and deceleration
		if (HorizontalInput != 0 || VerticalInput != 0)
		{
			_targetMoveSpeed = Mathf.Min(_targetMoveSpeed + Acceleration * Time.deltaTime, MaxSpeed);
		}
		else
		{
			_targetMoveSpeed = Mathf.Max(_targetMoveSpeed - Deceleration * Time.deltaTime, 0);
		}

		// Smoothly interpolate current move speed towards target move speed
		_currentMoveSpeed = Mathf.Lerp(_currentMoveSpeed, _targetMoveSpeed, LerpSpeed * Time.deltaTime);

		// Update velocity
		_velocity = _moveDirection * _currentMoveSpeed;

		// handle drag
		if (_controller.isGrounded)
			_controller.Move(_velocity * Time.deltaTime);
		else {
			// add gravity
			_controller.Move((_velocity * AirMultiplier + Vector3.down * Gravity) * Time.deltaTime);
		}
	}
	
	Vector3 GetWishDirection(Vector2 rawDirectionalMove)
	{
		return Vector3
			.Cross(
				(rawDirectionalMove.x * -CameraController.PlanarForward +
					rawDirectionalMove.y * CameraController.PlanarRight).normalized,
				new Vector3(0, 1, 0)).normalized;
	}
}
