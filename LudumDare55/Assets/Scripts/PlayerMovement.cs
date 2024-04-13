using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [FormerlySerializedAs("moveSpeed")]
    [Header("Movement")]
    public float MoveSpeed;
    

    [FormerlySerializedAs("groundDrag")]
    public float GroundDrag;

    [FormerlySerializedAs("jumpForce")]
    public float JumpForce;
    [FormerlySerializedAs("jumpCooldown")]
    public float JumpCooldown;
    [FormerlySerializedAs("airMultiplier")]
    public float AirMultiplier;
    bool _readyToJump;

    [FormerlySerializedAs("walkSpeed")]
    [HideInInspector] public float WalkSpeed;
    [FormerlySerializedAs("sprintSpeed")]
    [HideInInspector] public float SprintSpeed;

    [FormerlySerializedAs("jumpKey")]
    [Header("Keybinds")]
    public KeyCode JumpKey = KeyCode.Space;

    [FormerlySerializedAs("playerHeight")]
    [Header("Ground Check")]
    public float PlayerHeight;
    [FormerlySerializedAs("whatIsGround")]
    public LayerMask WhatIsGround;
    bool _grounded;

    [FormerlySerializedAs("orientation")]
    public Transform Orientation;

    public float HorizontalInput;
    public float VerticalInput;

    Vector3 _moveDirection;
    Rigidbody _rb;
    public CameraController CameraController;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _readyToJump = true;
    }

    private void Update()
    {
        // ground check
        _grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.3f, WhatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (_grounded)
            _rb.drag = GroundDrag;
        else
            _rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal Movement");
        VerticalInput = Input.GetAxisRaw("Vertical Movement");

        // when to jump
        if(Input.GetKey(JumpKey) && _readyToJump && _grounded)
        {
            _readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), JumpCooldown);
        }
    }

    private void MovePlayer () {
        // calculate movement direction
        _moveDirection = GetWishDirection(new Vector2(HorizontalInput, VerticalInput));


        if (_moveDirection != Vector3.zero) {
            // on ground
            if (_grounded)
                _rb.AddForce(_moveDirection.normalized*MoveSpeed*10f, ForceMode.Force);

            // in air
            else if (!_grounded)
                _rb.AddForce(_moveDirection.normalized*MoveSpeed*10f*AirMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * MoveSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        _rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }
    private void ResetJump () {
        _readyToJump = true;
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
