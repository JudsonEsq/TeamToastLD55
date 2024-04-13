using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float MoveSpeed;
	public float JumpForce;
	public float AirMultiplier;
	public float Gravity;
	public float HorizontalInput;
	public float VerticalInput;

	Vector3 _moveDirection;
	CharacterController _controller;
	public CameraController CameraController;

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
	}

	private void Update () {
		MyInput();
		_moveDirection = GetWishDirection(new Vector2(HorizontalInput, VerticalInput));

		Debug.Log(_moveDirection*MoveSpeed*Time.deltaTime);
		// handle drag
		if (_controller.isGrounded)
			_controller.Move(_moveDirection*MoveSpeed*Time.deltaTime);
		else {
			// add gravity
			_controller.Move(_moveDirection*(MoveSpeed*AirMultiplier*Time.deltaTime) + Vector3.down*(Gravity*Time.deltaTime));
		}
	}

	private void MyInput()
	{
		HorizontalInput = Input.GetAxisRaw("Horizontal Movement");
		VerticalInput = Input.GetAxisRaw("Vertical Movement");
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
